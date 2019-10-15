using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
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
                    && Constants.EmisConstants.ImgDocumentTypes.Contains(documentType))
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

            using (var memoryStream = new MemoryStream())
            {
                var msgLength = BitConverter.ToInt32(gzipBuffer, 0);
                memoryStream.Write(gzipBuffer, 0, gzipBuffer.Length);

                var buffer = new byte[msgLength];

                memoryStream.Position = 0;
                int length;
                using (var zip = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    length = zip.Read(buffer, 0, buffer.Length);
                }

                var data = new byte[length];
                Array.Copy(buffer, data, length);
                return Encoding.UTF8.GetString(data);

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

                var imgNode = doc.DocumentNode.SelectSingleNode(".//img");

                if (imgNode == null)
                {
                    _logger.LogInformation("Document contains no img tag");
                    return content;
                }
                
                if (!imgNode.Attributes.Contains("alt"))
                {
                    _logger.LogInformation("Setting alt text on img tag");
                    imgNode.SetAttributeValue("alt", altText);
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