using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using HtmlToOpenXml;
using Microsoft.Extensions.Logging;
using Wkhtmltopdf.NetCore;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisDocumentDownloadConverter
    {
        byte[] ConvertToText(string content);
        byte[] ConvertToImage(string content);
        byte[] ConvertToWordDocument(string content);
        byte[] ConvertToPdf(string content);
    }

    public class EmisDocumentDownloadConverter: IEmisDocumentDownloadConverter
    {
        private readonly ILogger<IEmisDocumentDownloadConverter> _logger;
        private readonly IGeneratePdf _generatePdf;
        
        public EmisDocumentDownloadConverter(ILogger<IEmisDocumentDownloadConverter> logger, IGeneratePdf generatePdf)
        {
            _logger = logger;
            _generatePdf = generatePdf;
        }

        public byte[] ConvertToText(string content)
        {
            var htmlDocument = CreateHtmlDocument(content);

            _logger.LogInformation("Converting document content to text");
            return Encoding.UTF8.GetBytes(System.Net.WebUtility.HtmlDecode(htmlDocument.DocumentNode.InnerText));
        }
        public byte[] ConvertToImage(string content)
        {
            var htmlDocument = CreateHtmlDocument(content);

            _logger.LogInformation("Converting HTML document to an image. Extracting the base64 src from the first img element");
            var imgNodes = htmlDocument.DocumentNode.SelectNodes(".//img");

            if (imgNodes != null)
            {
                return Convert.FromBase64String(
                    imgNodes[0]
                        .GetAttributeValue("src", "")
                        .Split("base64,")[1]);
            }

            _logger.LogInformation("HTML document contains no img tags");
            return null;
        }

        public byte[] ConvertToWordDocument(string content)
        {
            _logger.LogInformation("Converting document content to a Word Document");
            using (var outputStream = new MemoryStream())
            {
                using (var document = WordprocessingDocument.Create(outputStream, WordprocessingDocumentType.Document))
                {
                    var mainPart = document.MainDocumentPart;
                    
                    if (mainPart == null)
                    {
                        mainPart = document.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    _logger.LogInformation("Parsing document content into WordDocument");
                    new HtmlConverter(mainPart).ParseHtml(content);

                    mainPart.Document.Save();
                }

                return outputStream.ToArray();
            }
        }

        public byte[] ConvertToPdf(string content)
        {
            var htmlDocument = CreateHtmlDocument(content);

            _logger.LogInformation("Converting document content to a PDF");
            var imgNodes = htmlDocument.DocumentNode.SelectNodes(".//img");

            foreach (var imageNode in imgNodes)
            {
                imageNode.Attributes.Add("width", "100%");
            }

            return _generatePdf.GetPDF($"<html><body>${htmlDocument.DocumentNode.InnerHtml}</body></html>");
        }

        private static HtmlDocument CreateHtmlDocument(string content)
        {
            var htmlDocument = new HtmlDocument
            {
                OptionWriteEmptyNodes = true
            };

            htmlDocument.LoadHtml(content);

            return htmlDocument;
        }
    }
}