using Azure;
using Azure.Search.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleSemanticSearch.Models
{

    public class HttpClient
    {
        private string indexName = "testkb";

        // Get the service endpoint and API key from the cognitive search service
        private Uri endpoint = new Uri("");
        private string key = "";
        private AzureKeyCredential credential;

        public SearchClient SearchClient { get; set; }

        // Create a client
        public HttpClient()
        {
            credential = new AzureKeyCredential(key);
            SearchClient = new SearchClient(endpoint, indexName, credential);
        }
    }
}
