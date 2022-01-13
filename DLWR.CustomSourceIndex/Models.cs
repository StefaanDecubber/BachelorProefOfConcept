using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DlWR.CustomSourceIndex
{
    public class SearchPropertyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class ParsedImage
    {
        public string Key { get; set; }
        public string Base64String { get; set; }
        public string Url { get; set; }
    }

    public class PeopleSearchResult
    {
        

        public PeopleSearchResult(IDictionary<string, object> result)
        {
            AccountName = result["AccountName"].ToString();
            //UserProfile_GUID = result["UserProfile_GUID"].ToString();
            PreferredName = result["PreferredName"].ToString();
            //WorkEmail = result["WorkEmail"]?.ToString();
            WorkEmail = result["AccountName"].ToString().Split('|')[2];
            PictureURL = result["PictureURL"]?.ToString();
            //OfficeNumber = result["OfficeNumber"].ToString();
            //MobilePhone = result["MobilePhone"].ToString();
        }

        public string AccountName { get; set; }
        public string UserProfile_GUID { get; set; }
        public string PreferredName { get; set; }
        public string WorkEmail { get; set; }
        public string PictureURL { get; set; }
        public string OfficeNumber { get; set; }
        public string MobilePhone { get; set; }

    }

    public class ElasticDocument
    {
        [JsonProperty("_allow_permissions")]
        public List<object> AllowPermissions { get; set; }

        [JsonProperty("_deny_permissions")]
        public List<object> DenyPermissions { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("entity")]
        public string Entity { get; set; }
    }
}
