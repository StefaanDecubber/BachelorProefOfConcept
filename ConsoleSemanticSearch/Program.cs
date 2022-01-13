using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using BachelorBotAndSemanticSearch;
using ConsoleSemanticSearch.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleSemanticSearch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Create HttpClient with SearchClient
            HttpClient http = new HttpClient();

            //Create file path for excel
            var file = new FileInfo(@"");

            //Get questions from json file
            //var questions = QuestionHelper.GetQuestions();
            var questionsSelf = QuestionHelper.GetQuestionsSelf();

            //Create list of questionanswers to create excel file
            List<QuestionAnswer> qas = new List<QuestionAnswer>();

            //Create semantic queryoption with required options for semantic
            SearchOptions searchOptions = new SearchOptions();
            searchOptions.SemanticConfigurationName = "searchquestionsanswers";
            searchOptions.QueryLanguage = "en-us";
            searchOptions.QueryType = SearchQueryType.Semantic;
            searchOptions.QueryCaption = QueryCaptionType.Extractive;
            searchOptions.QueryAnswer = QueryAnswerType.Extractive;
            searchOptions.QueryAnswerCount = 1;

            //Choose questionlist: questions or questionsself
            foreach (var question in questionsSelf)
            {
                //Query cognitive service with searchClient
                SearchResults<QuestionAnswerResult> queryResults = http.SearchClient.Search<QuestionAnswerResult>(question, searchOptions);

                var results = queryResults.GetResults();
                QuestionAnswer qa = new QuestionAnswer();
                var result = results.OrderByDescending(x => x.Score).FirstOrDefault();
                qa.Answer = result.Document.answer;
                qa.Question = question;
                if (result.Score.HasValue)
                    qa.Score = (double)result.Score;
                qas.Add(qa);
                
            }
            await ExcelHelper.SaveDataInNewExcelAsync(qas, file, "questionSelf");
        }
    }
}
