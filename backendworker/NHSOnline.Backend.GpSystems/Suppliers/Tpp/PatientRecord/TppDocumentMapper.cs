using System;
using System.Globalization;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDocumentMapper
    {
        PatientDocument Map(RequestBinaryDataReply requestBinaryDataReply);
    }

    public class TppDocumentMapper : ITppDocumentMapper
    {

        public PatientDocument Map(RequestBinaryDataReply requestBinaryDataReply)
        {
            if (requestBinaryDataReply == null)
            {
                throw new ArgumentNullException(nameof(requestBinaryDataReply));
            }

            var type = MapFileTypeToDownloadType(requestBinaryDataReply.BinaryData.FileType);
            if (!Constants.FileConstants.FileTypes.TppWhiteListTypes.Contains(type))
            {
                return new PatientDocument
                {
                    HasErrored = true,
                };
            }

            var mimeType = Constants.FileConstants.FileTypes.DocumentMimeTypes[type];

            var htmlAddedToBinary = string.Format(
                CultureInfo.InvariantCulture,
                Constants.FileConstants.ImageHtmlFormat,
                mimeType,
                requestBinaryDataReply.BinaryData.BinaryDataPage.BinaryData);

            return new PatientDocument
            {
                Content = htmlAddedToBinary,
                Type = requestBinaryDataReply.BinaryData.FileType,
                HasErrored = false,
            };
        }

        private static string MapFileTypeToDownloadType(string fileType)
        {
            switch (fileType)
            {
                case Constants.FileConstants.FileTypes.ImageType.Jfif:
                    return Constants.FileConstants.FileTypes.ImageType.Jpg;
                default:
                    return fileType;
            }
        }
    }
}