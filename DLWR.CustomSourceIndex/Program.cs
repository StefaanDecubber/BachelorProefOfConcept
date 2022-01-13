using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PnP.Framework;


namespace DlWR.CustomSourceIndex
{
    class Program
    {
        private static string _clientId, _clientSecret, _defaultUrl, _apiKey;
        private static AuthenticationManager _authenticationManager;
        private const string ApiKeyHeader = "SPOMetadataMapper-Api-Key";

        static async Task Main(string[] args)
        {
            _apiKey = "264235BF-80BA-4150-8ECD-F03F1503FC0F";
            _clientId = "91a388cb-0f7c-4b7a-b930-8afa9232f198";
            _clientSecret = "g62iEvwj1Y5nz7Xf-BOu.DoAg42l.Ebfo-";
            _defaultUrl = "https://delawareconsulting.sharepoint.com";
            _authenticationManager = new AuthenticationManager();

            var elasticDocs = new List<ElasticDocument>();

            var questions = QuestionsHelper.GetQuestions();

            using (var context = _authenticationManager.GetACSAppOnlyContext(_defaultUrl, _clientId, _clientSecret))
            {
                var searchService = new SearchService(_defaultUrl, context);
                foreach(var question in questions)
                {
                    var results = await searchService.Search(question,
                    new List<string>() { "owstaxIdInsideSearchKeywords", "InsideKeywords" }, new List<string>(), 100, true, false);

                    //context.Load(context.Web, w => w.Title);
                    //context.ExecuteQuery();
                    var counter = 1;
                    foreach (var webResults in results)
                    {
                        using (var webContext = _authenticationManager.GetACSAppOnlyContext(webResults.WebUrl, _clientId, _clientSecret))
                        {
                            webContext.Load(webContext.Web, w => w.Title);
                            try
                            {
                                webContext.ExecuteQuery();
                            }
                            catch
                            {
                                //log  (or whatever you want to do with it)
                            }

                            var html = HtmlHelpers.GetHtmlFromSearchResult(webResults, webContext);
                            if (html != "<html><head></head><body></body></html>")
                            {
                                html = html.Replace("&#160;", " ");
                                html = html.Replace("&#58;", ":");
                                html = html.Replace("<i>", "");
                                html = html.Replace("</i>", "");
                                html = html.Replace("’", "'");
                                html = html.Replace("<p>​The", "<p>The");
                                html = html.Replace("“", "'");
                                html = html.Replace("”", "'");
                                html = html.Replace("…", "...");
                                html = html.Replace("–", "-");
                                html = html.Replace("<p></p>", "");
                                html = html.Replace("<span></span>", "");
                                html = html.Replace("<b></b>", "");
                                html = html.Replace("<i></i>", "");
                                html = HtmlHelpers.CleanUp(html);
                                var wordDoc = WordHelpers.CreateWordByHtml(html);
                                var questionTrim = question.Trim().Replace(" ", "").Replace("?", "");
                                System.IO.File.WriteAllBytes($"C:\\Code\\Research\\IccBot\\SearchResults\\{questionTrim}{counter}.docx", wordDoc);
                                break;
                            }
                            counter++;
                        }
                    }
                }
            }
        }
    }

    public static class QuestionsHelper
    {
        public static List<string> GetQuestions()
        {
            #region JsonReader
            var questions = new List<string>();
            //C:\Users\stefa\Documents\informatica\bachelorproef\DLWR.CustomSourceIndex\DLWR.CustomSourceIndex\DLWR.CustomSourceIndex\questions.json
            using (StreamReader r = new StreamReader("C:\\Users\\stefa\\Documents\\informatica\\bachelorproef\\DLWR.CustomSourceIndex\\DLWR.CustomSourceIndex\\DLWR.CustomSourceIndex\\questions.json"))
            {
                string json = r.ReadToEnd();
                var questionMapper = JsonConvert.DeserializeObject<QuestionsMapper>(json);
                questions = questionMapper.QuestionsSelf;
            }
            #endregion
            return questions;
        }
    }

    public static class HtmlHelpers
    {
        public static string GetHtmlFromSearchResult(SearchResult result, ClientContext context)
        {
            var html = "<html><head></head><body>";

                try
                {
                    var page = context.Web.GetFileByUrl(result.Path);
                    context.Load(page, p => p.Title);
                    context.Load(page.ListItemAllFields, srcItm => srcItm["CanvasContent1"],
                        srcItm => srcItm["LayoutWebpartsContent"]);
                    context.ExecuteQuery();
                    var canvasContent = page.ListItemAllFields["CanvasContent1"].ToString();

                    html = $"{html}<div>{RemoveAllImageNodes(canvasContent)}</div>";

                }
                catch (Exception ex)
                {

                }
          
            html = html + "</body></html>";

            return html;
        }

        public static string GetHtmlFromSearchResults(IEnumerable<SearchResult> searchResults, ClientContext context)
        {
            var html = "<html><head></head><body>";
            foreach (SearchResult result in searchResults)
            {
                try
                {
                    var page = context.Web.GetFileByUrl(result.Path);
                    context.Load(page, p => p.Title);
                    context.Load(page.ListItemAllFields, srcItm => srcItm["CanvasContent1"],
                        srcItm => srcItm["LayoutWebpartsContent"]);
                    context.ExecuteQuery();
                    var canvasContent = page.ListItemAllFields["CanvasContent1"].ToString();

                    html = $"{html}<div>{RemoveAllImageNodes(canvasContent)}</div>";

                }
                catch (Exception ex)
                {

                }
            }
            html = html + "</body></html>";

            return html;
        }

        public static string RemoveAllImageNodes(string html)
        {
            try
            {
                //html = HttpUtility.HtmlDecode(html);

                //html = Regex.Replace(html,
                //    @"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$",
                //    string.Empty);

                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(html);

                var nodes = document.DocumentNode.SelectNodes("//img");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        node.Remove();
                        //node.Attributes.Remove("src"); //This only removes the src Attribute from <img> tag
                    }

                    html = document.DocumentNode.OuterHtml;
                }

                //var divs = document.DocumentNode.SelectNodes("//div");
                //if (divs != null)
                //{
                //    foreach (var tag in divs)
                //    {
                //        if (tag.Attributes["data-sp-webpartdata"] != null)
                //        {
                //            tag.Remove();
                //        }
                //    }

                //    html = document.DocumentNode.OuterHtml;
                //}
            }
            catch (Exception ex)
            {
            }

            return html;
        }

        public static string CleanUp(string html)
        {
            var htmlToKeep = "<html><head></head><body>";
            html = HttpUtility.HtmlDecode(html);
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);

            var divs = document.DocumentNode.SelectNodes("//div");
            if (divs != null)
            {
                foreach (var tag in divs)
                {
                    if (tag.Attributes["data-sp-rte"] != null)
                    {
                        htmlToKeep = htmlToKeep + tag.InnerHtml;
                    }
                }
            }

            return htmlToKeep + "</body></html>";
        }
    }
}
