using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DlWR.CustomSourceIndex
{
    public static class WordHelpers
    {
        public static byte[] CreateWordByHtml(string html)
        {
            using (var stream = new MemoryStream())
            {
                //1. Create Document
                using (var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = document.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = document.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    //var decoded = System.Net.WebUtility.UrlDecode(html).Replace("\r\n", "");
                    ////Re-Encode Special letters to ascii
                    //decoded = decoded.Replace("é", "&eacute;");
                    //decoded = decoded.Replace("à", "&agrave;");
                    //decoded = decoded.Replace("ë", "&euml;");

                    string altChunkId = "myId";
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(html));


                    // Create alternative format import part.
                    AlternativeFormatImportPart formatImportPart =
                        mainPart.AddAlternativeFormatImportPart(
                            AlternativeFormatImportPartType.Html, altChunkId);

                    // Feed HTML data into format import part (chunk).
                    formatImportPart.FeedData(ms);
                    AltChunk altChunk = new AltChunk();
                    altChunk.Id = altChunkId;

                    mainPart.Document.Body.Append(altChunk);
                }

                return stream.ToArray();

            }
        }
    }
}