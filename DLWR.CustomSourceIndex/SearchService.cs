using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using PnP.Framework;

namespace DlWR.CustomSourceIndex
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResult>> Search(string query, List<string> properties, List<string> sortList, int rowLimit, bool cleanHtml, bool parseImages);
        IEnumerable<PeopleSearchResult> SearchPeople(string query, int rowLimit);
    }

    public class SearchService : ISearchService
    {
        private readonly string _accessToken;
        private readonly Uri _siteCollectionUri;
        private readonly string _clientId, _clientSecret, _spUrl, _thumbPrint, _tenantId;
        private readonly ClientContext _ctx;

        public SearchService(string url, ClientContext ctx)
        {
            _spUrl = url;
            _ctx = ctx;
        }

        public SearchService(string url, string clientId, string clientSecret)
        {
            _spUrl = url;
            _clientId = clientId;
            _clientSecret = clientSecret;
            AuthenticationManager a = new AuthenticationManager();
            _ctx = a.GetACSAppOnlyContext(_spUrl, _clientId, _clientSecret);
        }
        public async Task<IEnumerable<SearchResult>> Search(string query, List<string> properties, List<string> sortList, int rowLimit, bool cleanHtml, bool parseImages)
        {
            return SharePointSearch.Search(query, properties, _ctx, sortList, rowLimit, cleanHtml, parseImages);
        }

        public IEnumerable<PeopleSearchResult> SearchPeople(string query, int rowLimit)
        {
            //using (var ctx = _authenticationManager.GetAppOnlyAuthenticatedContext(_spUrl, _clientId, _clientSecret))
            //{
            //    return SharePointSearch.SearchPeople(query, ctx, rowLimit);
            //}
            return null;
        }
    }
}
