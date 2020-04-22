using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDocumentMapper
    {
        PatientDocument Map(RequestBinaryDataReply requestBinaryDataReply);
        FileContentResult MapForDownload(RequestBinaryDataReply requestBinaryDataReply, string documentName);
    }

    [SuppressMessage("Microsoft.Naming", "CA1308", Justification = "Required for matching file extensions")]
    public class TppDocumentMapper : ITppDocumentMapper
    {
        private readonly ILogger<ITppDocumentMapper> _logger;

        public TppDocumentMapper(ILogger<ITppDocumentMapper> logger)
        {
            _logger = logger;
        }

        public PatientDocument Map(RequestBinaryDataReply requestBinaryDataReply)
        {
            if (requestBinaryDataReply is null)
            {
                throw new ArgumentNullException(nameof(requestBinaryDataReply));
            }

            var type = MapFileTypeToDownloadType(requestBinaryDataReply.BinaryData.FileType);

            var isViewable = Constants.FileConstants.FileTypes.TppViewableWhiteListTypes.Contains(
                type, StringComparer.OrdinalIgnoreCase);
            var isDownloadable = Constants.FileConstants.FileTypes.WhiteListTypes.Contains(
                type, StringComparer.OrdinalIgnoreCase);

            if (!isViewable && !isDownloadable)
            {
                _logger.LogWarning($"Unsupported file type: {type}");
            }

            string content = null;

            if (isViewable)
            {
                content = string.Format(
                    CultureInfo.InvariantCulture,
                    Constants.FileConstants.ImageHtmlFormat,
                    Constants.FileConstants.FileTypes.DocumentMimeTypes[type],
                    requestBinaryDataReply.BinaryData.BinaryDataPage.BinaryData);
            }

            return new PatientDocument
            {
                Content = content,
                Type = requestBinaryDataReply.BinaryData.FileType,
                HasErrored = false,
                IsViewable = isViewable,
                IsDownloadable = isDownloadable
            };
        }

        public FileContentResult MapForDownload(RequestBinaryDataReply requestBinaryDataReply, string documentName)
        {
            if (requestBinaryDataReply is null)
            {
                throw new ArgumentNullException(nameof(requestBinaryDataReply));
            }

            var type = MapFileTypeToDownloadType(requestBinaryDataReply.BinaryData.FileType);
            var documentAsBase64 = requestBinaryDataReply.BinaryData.BinaryDataPage.BinaryData;

            if (!Constants.FileConstants.FileTypes.WhiteListTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogError($"Unsupported file type: {type}");
                return null;
            }

            if (string.IsNullOrWhiteSpace(documentAsBase64))
            {
                _logger.LogError("Null or blank base64 binary data returned in response");
                return null;
            }

            var documentAsBytes = Convert.FromBase64String(documentAsBase64);
            var mimeType = Constants.FileConstants.FileTypes.DocumentMimeTypes[type];
            var fileName = $"{documentName}.{type}";

            return new FileContentResult(documentAsBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }

        private string MapFileTypeToDownloadType(string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileType))
            {
                throw new ArgumentException("Must not be null or blank", nameof(fileType));
            }

            var mappedFileType = fileType.ToLowerInvariant();

            if (mappedFileType == Constants.FileConstants.FileTypes.ImageType.Jfif)
            {
                mappedFileType = Constants.FileConstants.FileTypes.ImageType.Jpg;
            }

            _logger.LogInformation($"Mapped actual document type {fileType} to {mappedFileType} for download");
            return mappedFileType;
        }
    }
}