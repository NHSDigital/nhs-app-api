using System;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.BinaryData;

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

            return new PatientDocument
            {
                Type = requestBinaryDataReply.BinaryData.FileType,
                HasErrored = false,
            };
        }
    }
}