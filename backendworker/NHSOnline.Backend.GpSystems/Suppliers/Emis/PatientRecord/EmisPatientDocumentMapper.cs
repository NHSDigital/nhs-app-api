using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Sanitization;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisPatientDocumentMapper
    {
        private readonly ILogger<EmisPatientDocumentMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public EmisPatientDocumentMapper(ILogger<EmisPatientDocumentMapper> logger, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }

        public PatientDocument Map(IndividualDocument documentGetResponse, string documentType, string documentName)
        {
            _logger.LogInformation("Mapping document response");
            if (documentGetResponse == null)
            {
                throw new ArgumentNullException(nameof(documentGetResponse));
            }

            var document = new PatientDocument();

            if (documentGetResponse.CompressedEncodedDocumentContent != null)
            {
                _logger.LogInformation("Decompressing retrieved document content");

                var documentContent = DecompressGzip(documentGetResponse.CompressedEncodedDocumentContent);

                if (!string.IsNullOrEmpty(documentType)
                    && !string.IsNullOrEmpty(documentName)
                    && (Constants.FileConstants.FileTypes.ImageTypes.Contains(documentType) || 
                    documentType.Equals(Constants.FileConstants.FileTypes.DocumentType.Pdf, StringComparison.Ordinal)))
                {
                    documentContent = AddAltTextToImage(documentContent, documentName);
                }
                
                
                document.Content = _htmlSanitizer.GetBodyContent(documentContent);
            }

            return document;
        }

        private static string DecompressGzip(string str)
        {
            var gzipBuffer = Convert.FromBase64String(str);

            using (var memoryStream = new MemoryStream(gzipBuffer))
            using (var memoryStreamOut = new MemoryStream())
            {
                using (var zip = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    zip.CopyTo(memoryStreamOut);
                }
                return Encoding.UTF8.GetString(memoryStreamOut.ToArray());
            }
        }

        private string AddAltTextToImage(string content, string altText)
        {
            _logger.LogEnter();
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    return string.Empty;
                }

                var doc = new HtmlDocument
                {
                    OptionWriteEmptyNodes = true
                };

                doc.LoadHtml(content);

                var imgNodes = doc.DocumentNode.SelectNodes(".//img");

                if (imgNodes == null)
                {
                    _logger.LogInformation("Document contains no img tag");
                    return content;
                }

                if (imgNodes.Count > 1)
                {
                    // if a pdf has multiple images we want to
                    // set the alt text to distinguish pages
                    //eg altText="test.pdf page 1"
                    foreach (var imgNode in imgNodes)
                    {
                        if (!imgNode.Attributes.Contains("alt"))
                        {
                            imgNode.SetAttributeValue("alt", altText + " page " 
                                                                     + (imgNodes.IndexOf(imgNode) + 1));
                        }
                    }
                }
                else
                {
                    if (!imgNodes[0].Attributes.Contains("alt"))
                    {
                        _logger.LogInformation("Setting alt text on img tag");
                        imgNodes[0].SetAttributeValue("alt", altText);
                    }    
                }
                
                return doc.DocumentNode.InnerHtml;
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}