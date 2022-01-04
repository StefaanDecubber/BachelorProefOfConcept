using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using BachelorQnABot.Models;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BachelorQnABot
{
    public class BotServices : IBotServices
    {
        public BotServices(IConfiguration configuration, OrchestratorRecognizer dispatcher)
        {
            #region JsonReaderCognitiveModels
            using (StreamReader r = new StreamReader("cognitivemodels.json"))
            {
                string json = r.ReadToEnd();
                Mapper = JsonConvert.DeserializeObject<Mapper>(json);
            }
            #endregion

            Dispatch = dispatcher;

            #region Textanalytics projects
            //Create http client to acces projects
            TextAnalyticsClient = new TextAnalyticsClient(new Uri(Mapper.TextanalyticModel.EndpointUri), new AzureKeyCredential(Mapper.TextanalyticModel.Credential));
            QuestionAnsweringClient = new QuestionAnsweringClient(new Uri(Mapper.TextanalyticModel.EndpointUri), new AzureKeyCredential(Mapper.TextanalyticModel.Credential));
            Projects = new Dictionary<string, QuestionAnsweringProject>();

            //Create projects and store in dictionary
            foreach (var item in Mapper.TextanalyticModel.Projects)
            {
                Projects.Add(item.Name, new QuestionAnsweringProject(item.Name, item.DeploymentName));
            }
            #endregion
        }

        public Mapper Mapper { get; set; }
        public Dictionary<string, QuestionAnsweringProject> Projects { get ; set ; }
        public TextAnalyticsClient TextAnalyticsClient { get ; set ; }
        public QuestionAnsweringClient QuestionAnsweringClient { get ; set ; }
        public OrchestratorRecognizer Dispatch { get; set; }
    }
}
