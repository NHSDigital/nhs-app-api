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

            var binaryData = requestBinaryDataReply.BinaryData.BinaryDataPage.BinaryData;
            var fileType = requestBinaryDataReply.BinaryData.FileType;
            var type = MapFileTypeToDownloadType(fileType);

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
                binaryData);

            return new PatientDocument
            {
                Content = htmlAddedToBinary,
                Type = fileType,
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