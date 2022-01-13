using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BachelorBotAndSemanticSearch
{
    public static class QuestionHelper
    {
        public static List<string> GetQuestions()
        {
            #region JsonReader
            var questions = new List<string>();
            using (StreamReader r = new StreamReader(""))
            {
                string json = r.ReadToEnd();
                var questionMapper = JsonConvert.DeserializeObject<QuestionsMapper>(json);
                questions = questionMapper.Questions;
            }
            #endregion
            return questions;
        }

        public static List<string> GetQuestionsSelf()
        {
            #region JsonReader
            var questions = new List<string>();
            using (StreamReader r = new StreamReader(""))
            {
                string json = r.ReadToEnd();
                var questionMapper = JsonConvert.DeserializeObject<QuestionsMapper>(json);
                questions = questionMapper.QuestionsSelf;
            }
            #endregion
            return questions;
        }
    }
}
