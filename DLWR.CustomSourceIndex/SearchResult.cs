using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SharePoint.Client;

namespace DlWR.CustomSourceIndex
{
    public class SearchResult
    {
        List<ParsedImage> _parsedImages;
        private string _webUrl, _siteUrl;
        public SearchResult(IDictionary<string, object> result, List<string> properties, ClientContext ctx, bool cleanHtml, bool parseImages)
        {
            _parsedImages = new List<ParsedImage>();
            ListItemId = result["ListItemID"].ToString();
            ContentType = result["ContentType"].ToString();
            Path = result["Path"].ToString();
            Title = result["Title"].ToString();
            _webUrl = result["SPWebUrl"].ToString();
            _siteUrl = result["SPSiteUrl"].ToString();
            WebUrl = _webUrl;
            SearchProperties = BuildPropertyValuesList(result, properties, cleanHtml, parseImages, ctx);
            if (parseImages)
            {
                ParsedImages = _parsedImages;
            }
        }

        private IEnumerable<SearchPropertyValue> BuildPropertyValuesList(IDictionary<string, object> result, List<string> properties, bool cleanHtml, bool parseImages, ClientContext ctx)
        {
            return properties.Select(property => new SearchPropertyValue()
            {
                Key = property,
                Value = PrepareValue(result[property]?.ToString(), cleanHtml, parseImages, ctx)
            }).ToList();
        }

        private string PrepareValue(string value, bool cleanHtml, bool parseImages, ClientContext ctx)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s*\1\s*>");
            var isHtml = tagRegex.IsMatch(value);

            if (!isHtml) return value;

            if (parseImages)
            {
                var imageUrls = FetchLinksFromSource(value);
                foreach (var imageUrl in imageUrls)
                {
                    _parsedImages.Add(new ParsedImage()
                    {
                        Key = imageUrl,
                        Url = _siteUrl + imageUrl,
                        Base64String = ParseImage(ctx, _webUrl, imageUrl)
                    });
                }
            }

            if (!cleanHtml) return value;

            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        private string ParseImage(ClientContext ctx, string webUrl, string url)
        {
            //This doesn't work because of images being on premise (Hybrid Search)
            return "";

            //Uri filename = new Uri(webUrl + url);
            //string server = filename.AbsoluteUri.Replace(filename.AbsolutePath, "");
            //string serverrelative = filename.AbsolutePath;

            //FileInformation f = File.OpenBinaryDirect(ctx, serverrelative);
            //ctx.ExecuteQuery();

            //using (var memoryStream = new MemoryStream())
            //{
            //    f.Stream.CopyTo(memoryStream);
            //    return "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
            //}
        }

        public List<string> FetchLinksFromSource(string htmlSource)
        {
            List<string> links = new List<string>();
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(htmlSource, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                string href = m.Groups[1].Value;
                links.Add(href);
            }
            return links;
        }

        public string ListItemId { get; set; }
        public string ContentType { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string WebUrl { get; set; }
        public IEnumerable<SearchPropertyValue> SearchProperties { get; set; }
        public IEnumerable<ParsedImage> ParsedImages { get; set; }

    }
}
