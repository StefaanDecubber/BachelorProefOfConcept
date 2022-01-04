using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BachelorQnABot.Models
{
    public class TextanalyticModel
    {
        public string EndpointUri { get; set; }
        public string Credential { get; set; }
        public List<Project> Projects { get; set; }
    }
}
