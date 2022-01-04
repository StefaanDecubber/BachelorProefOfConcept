using Azure.AI.Language.QuestionAnswering;
using Azure.AI.TextAnalytics;
using Microsoft.Bot.Builder.AI.Orchestrator;
using System;
using System.Collections.Generic;

namespace BachelorQnABot
{
    public interface IBotServices
    {
        public Dictionary<string, QuestionAnsweringProject> Projects { get; set; }
        public TextAnalyticsClient TextAnalyticsClient { get; set; }
        public QuestionAnsweringClient QuestionAnsweringClient { get; set; }
        public OrchestratorRecognizer Dispatch { get; }
    }
}
