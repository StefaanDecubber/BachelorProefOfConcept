using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleSemanticSearch.Models
{
    public class QuestionAnswerResult
    {
        [JsonProperty("@search.score")]
        public double SearchScore { get; set; }

        [JsonProperty("@search.rerankerScore")]
        public double SearchRerankerScore { get; set; }

        public string kbId { get; set; }
        public string key { get; set; }
        public string id { get; set; }
        public List<string> questions { get; set; }
        public string answer { get; set; }
        public string source { get; set; }
        public List<object> keywords { get; set; }
        public string changeStatus { get; set; }
        public object alternateQuestions { get; set; }
        public string isContextOnly { get; set; }
        public List<object> parentIds { get; set; }
        public string prompts { get; set; }
        public DateTime lastUpdatedTimestampUTC { get; set; }

    }
}
