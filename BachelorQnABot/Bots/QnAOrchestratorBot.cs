// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.15.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

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
            // Top intent tell us which cognitive service to use.
            var allScores = await _botServices.Dispatch.RecognizeAsync(dc, (Activity)turnContext.Activity, cancellationToken);
            var topIntent = allScores.Intents.First().Key;

            // Next, we call the dispatcher with the top intent.
            await DispatchToTopIntentAsync(turnContext, topIntent, cancellationToken);
        }

        private async Task DispatchToTopIntentAsync(ITurnContext<IMessageActivity> turnContext, string intent, CancellationToken cancellationToken)
        {
            if(intent == "None")
            {
                await ProcessQnaRequest(turnContext, "CasualConversation", cancellationToken);
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
