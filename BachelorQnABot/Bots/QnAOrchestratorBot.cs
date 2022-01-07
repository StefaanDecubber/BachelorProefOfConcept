// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.15.0

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BachelorQnABot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace BachelorQnABot.Bots
{
    public class QnAOrchestratorBot : ActivityHandler
    {
        private ILogger<QnAOrchestratorBot> _logger;
        private IBotServices _botServices;

        public QnAOrchestratorBot(IBotServices botServices, ILogger<QnAOrchestratorBot> logger)
        {
            _logger = logger;
            _botServices = botServices;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var dc = new DialogContext(new DialogSet(), turnContext, new DialogState());
            if (turnContext.Activity.Text == "Voer test uit")
            {
                //Get questions from json file
                var questions = GetQuestions();
                //Create questionanswer objects with all questions answers intents score etc.
                var qas = await GetQuestionAnswersAsync(questions, dc, cancellationToken);
                //Write qas in excel
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var file = new FileInfo(@"C:\Users\stefa\Documents\informatica\bachelorproef\Bestanden\DataTest.xlsx");
                await SaveExcelFile(qas, file);

            }
            else
            {
                // Top intent tell us which cognitive service to use.
                var allScores = await _botServices.Dispatch.RecognizeAsync(dc, (Activity)turnContext.Activity, cancellationToken);
                var topIntent = allScores.Intents.First().Key;

                // Next, we call the dispatcher with the top intent.
                await DispatchToTopIntentAsync(turnContext, topIntent, cancellationToken);
            }
        }

        private async Task SaveExcelFile(List<QuestionAnswer> qas, FileInfo file)
        {
            DeleteIfExists(file);

            using var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add("MainReport");

            var range = ws.Cells["A2"].LoadFromCollection(qas, true);
            range.AutoFitColumns();
            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }

        private async Task<List<QuestionAnswer>> GetQuestionAnswersAsync(List<string> questions, DialogContext dc, CancellationToken cancellationToken)
        {
            var qas = new List<QuestionAnswer>();
            foreach (var question in questions)
            {
                Activity act = new Activity();
                act.Text = question;
                var scores = await _botServices.Dispatch.RecognizeAsync(dc, act, cancellationToken);
                var topIntentForEach = scores.Intents.First().Key;

                var qa = new QuestionAnswer();
                qa.Question = question;
                qa.Intent = topIntentForEach;
                qa.Score = (double)scores.Intents.First().Value.Score;

                if (topIntentForEach == "None")
                {
                    var result = await _botServices.QuestionAnsweringClient.GetAnswersAsync(question, _botServices.Projects["CasualConversation"]);
                    qa.Answer = result.Value.Answers.First().Answer;
                    qa.ShortAnswer = result.Value.Answers.First().ShortAnswer?.Text;
                }
                else
                {
                    var result = await _botServices.QuestionAnsweringClient.GetAnswersAsync(question, _botServices.Projects[topIntentForEach]);
                    qa.Answer = result.Value.Answers.First().Answer;
                    qa.ShortAnswer = result.Value.Answers.First().ShortAnswer?.Text;
                }
                qas.Add(qa);
            }
            return qas;
        }

        private List<string> GetQuestions()
        {
            #region JsonReader
            var questions = new List<string>();
            using (StreamReader r = new StreamReader("questions.json"))
            {
                string json = r.ReadToEnd();
                var questionMapper = JsonConvert.DeserializeObject<QuestionsMapper>(json);
                questions = questionMapper.Questions;
            }
            #endregion
            return questions;
        }
        
        //If the question doesn't belong to one of the projects in orchestrator.blu than the question will be answered by the chitchat project with name CasualConversation
        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, CancellationToken cancellationToken)
        {
            if(intent == "None")
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
            }
            else
            {
                await ProcessQnaRequest(turnContext, intent, cancellationToken);
            }
        }
        

        //Get answer in textanalytics project with the given name
        //If a short answer is available than return the short answer
        private async Task ProcessQnaRequest(ITurnContext<IMessageActivity> turnContext, string name, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessQnaRequest");

            //using textanalytics
            var result = await _botServices.QuestionAnsweringClient.GetAnswersAsync(turnContext.Activity.Text, _botServices.Projects[name]);
            if (result.Value.Answers.Any())
            {
                if (result.Value.Answers.First().ShortAnswer == null)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(result.Value.Answers.First().Answer), cancellationToken);
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(result.Value.Answers.First().ShortAnswer.Text), cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
