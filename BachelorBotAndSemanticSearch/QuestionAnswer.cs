using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorBotAndSemanticSearch
{
    public class QuestionAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string ShortAnswer { get; set; }
        public bool HasShortAnswer => !string.IsNullOrEmpty(ShortAnswer);
        public string Intent { get; set; }
        public double Score { get; set; }
    }
}
