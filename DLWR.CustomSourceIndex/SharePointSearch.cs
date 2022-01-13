using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;

namespace DlWR.CustomSourceIndex
{
    public static class SharePointSearch
    {
        public static List<SearchResult> Search(string query, List<string> properties, ClientContext ctx, List<string> sortList, int rowLimit, bool cleanHtml, bool parseImages)
        {
            var keywordQuery = new KeywordQuery(ctx)
            {
                QueryText = query
            };
            keywordQuery.SelectProperties.Add("ListItemID");
            keywordQuery.SelectProperties.Add("Title");
            keywordQuery.SelectProperties.Add("Path");
            keywordQuery.SelectProperties.Add("Created");
            keywordQuery.SelectProperties.Add("ContentType");
            keywordQuery.SelectProperties.Add("SPWebUrl");
            keywordQuery.SelectProperties.Add("Author");
            keywordQuery.SelectProperties.Add("SPSiteUrl");
            foreach (var property in properties)
            {
                keywordQuery.SelectProperties.Add(property);
            }
            keywordQuery.RowLimit = rowLimit;
            keywordQuery.TrimDuplicates = false;
            foreach (var property in sortList)
            {
                keywordQuery.SortList.Add(property, SortDirection.Descending);
            }
            //keywordQuery.SortList.Add("Created", SortDirection.Descending);
            keywordQuery.SourceId = new Guid("8413cd39-2156-4e00-b54d-11efd9abdb89");

            var results = GetSearchResults(keywordQuery, ctx);

            return results.ResultRows.Select(resultRow => new SearchResult(resultRow, properties, ctx, cleanHtml, parseImages)).ToList();
        }

        internal static IEnumerable<PeopleSearchResult> SearchPeople(string query, ClientContext ctx, int rowLimit)
        {
            var keywordQuery = new KeywordQuery(ctx)
            {
                QueryText = query
            };

            keywordQuery.SelectProperties.Add("AccountName");
            keywordQuery.SelectProperties.Add("UserProfile_GUID");
            keywordQuery.SelectProperties.Add("PreferredName");
            keywordQuery.SelectProperties.Add("WorkEmail");
            keywordQuery.SelectProperties.Add("OfficeNumber");
            keywordQuery.SelectProperties.Add("PictureURL");
            keywordQuery.SelectProperties.Add("MobilePhone");

            keywordQuery.RowLimit = rowLimit;
            keywordQuery.TrimDuplicates = false;
            keywordQuery.SourceId = new Guid("b09a7990-05ea-4af9-81ef-edfab16c4e31");

            var results = GetSearchResults(keywordQuery, ctx);

            return results.ResultRows.Select(resultRow => new PeopleSearchResult(resultRow)).ToList();
        }

        private static ResultTable GetSearchResults(KeywordQuery qry, ClientContext clientContext)
        {
            var searchExecutor = new SearchExecutor(clientContext);
            var results = searchExecutor.ExecuteQuery(qry);
            clientContext.ExecuteQuery();


            return results.Value[0];
        }
    }
}
