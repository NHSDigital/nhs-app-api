using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Sanitization;
using FileTypes = NHSOnline.Backend.Support.Constants.FileConstants.FileTypes;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisPatientDocumentMapper
    {
        PatientDocument Map(IndividualDocument documentGetResponse, string documentType, string documentName);
        FileContentResult MapForDownload(IndividualDocument documentGetResponse, string documentType, string documentName);
    }

    [SuppressMessage("Microsoft.Naming", "CA1308", Justification = "Required for matching file extensions")]
    public class EmisPatientDocumentMapper: IEmisPatientDocumentMapper
    {
        private readonly ILogger<IEmisPatientDocumentMapper> _logger;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly IEmisDocumentDownloadConverter _emisDocumentDownloadConverter;

        public EmisPatientDocumentMapper(
            ILogger<IEmisPatientDocumentMapper> logger,
            IHtmlSanitizer htmlSanitizer,
            IEmisDocumentDownloadConverter emisDocumentDownloadConverter)
        {
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
            _emisDocumentDownloadConverter = emisDocumentDownloadConverter;
        }

        public PatientDocument Map(IndividualDocument documentGetResponse, string documentType, string documentName)
        {
            try
            {
                _logger.LogEnter();

                if (documentGetResponse?.CompressedEncodedDocumentContent == null)
                {
                    _logger.LogError("Response contained no content");
                    return new PatientDocument
                    {
                        HasErrored = true
                    };
                }

                _logger.LogInformation("Decompressing retrieved document content");

                var documentContent = DecompressGzip(documentGetResponse.CompressedEncodedDocumentContent);

                if (ShouldAddAltTextToImage(documentType, documentName))
                {
                    documentContent = AddAltTextToImage(documentContent, documentName);
                }

                return new PatientDocument
                {
                    Content = _htmlSanitizer.GetBodyContent(documentContent),
                    IsViewable = true,
                    IsDownloadable = true
                };
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public FileContentResult MapForDownload(IndividualDocument documentGetResponse, string documentType, string documentName)
        {
            try
            {
                _logger.LogEnter();

                var patientDocument = Map(documentGetResponse, documentType, documentName);

                if (patientDocument?.HasErrored ?? true)
                {
                    _logger.LogError("Mapped patient document is null or has errored. Returning null FileContentResult");
                    return null;
                }

                var documentAsBytes = ConvertHtmlDocumentToBytes(documentType, patientDocument.Content);

                if (documentAsBytes == null)
                {
                    _logger.LogError($"{nameof(ConvertHtmlDocumentToBytes)} returned null. Returning null FileContentResult");
                    return null;
                }

                var mappedType = MapFileTypeToDownloadType(documentType);
                var mimeType = FileTypes.DocumentMimeTypes[mappedType];
                var fileName = $"{documentName}.{mappedType}";

                return new FileContentResult(documentAsBytes, mimeType)
                {
                    FileDownloadName = fileName
                };
            }
            finally
            {
                _logger.LogExit();
            }
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

                _logger.LogInformation("Adding alt text to images in document");

                doc.DocumentNode
                    .SelectNodes(".//img")
                    ?.Where(n => !n.Attributes.Contains("alt"))
                    .ForEach((node, index) => node.SetAttributeValue("alt", $"{altText} page {index + 1}"));

                return doc.DocumentNode.InnerHtml;
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private byte[] ConvertHtmlDocumentToBytes(string type, string content)
        {
            if (FileTypes.TextTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                return _emisDocumentDownloadConverter.ConvertToText(content);
            }

            if (FileTypes.ImageTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                return _emisDocumentDownloadConverter.ConvertToImage(content);
            }

            if (FileTypes.DocumentTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                return _emisDocumentDownloadConverter.ConvertToWordDocument(content);
            }

            return !string.IsNullOrEmpty(type) && FileTypes.DocumentType.Pdf.Equals(type.ToLowerInvariant(), StringComparison.Ordinal)
                ? _emisDocumentDownloadConverter.ConvertToPdf(content)
                : null;
        }

        private static bool ShouldAddAltTextToImage(string documentType, string documentName) =>
            !string.IsNullOrEmpty(documentType) &&
            !string.IsNullOrEmpty(documentName) &&
            (FileTypes.ImageTypes.Contains(documentType, StringComparer.OrdinalIgnoreCase) ||
             FileTypes.DocumentType.Pdf.Equals(documentType, StringComparison.Ordinal));

        private string MapFileTypeToDownloadType(string fileType)
        {
            var mappedFileType = fileType.ToLowerInvariant();

            switch (mappedFileType)
            {
                case FileTypes.DocumentType.Docm:
                    mappedFileType = FileTypes.DocumentType.Doc;
                    break;
                case FileTypes.TextType.Rtf:
                    mappedFileType = FileTypes.TextType.Txt;
                    break;
                case FileTypes.ImageType.Jfif:
                    mappedFileType = FileTypes.ImageType.Jpg;
                    break;
            }

            _logger.LogInformation($"Mapped actual document type {fileType} to {mappedFileType} for download");
            return mappedFileType;
        }
    }
}