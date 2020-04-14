using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class EmisDocumentsMapper
    {
        internal PatientDocuments Map(MedicationRootObject documentsGetResponse)
        {
            return new PatientDocuments()
            {
                Data = documentsGetResponse?.MedicalRecord
                    ?.Documents
                    ?.Select(Map)
                    .ToList() ?? new List<DocumentItem>()
            };
        }

        private static DocumentItem Map(Document document)
        {
            var name = document.Observation
                .AssociatedText
                ?.Select(x => x.Text)
                .FirstOrDefault();

            var effectiveDate = new MyRecordDate
            {
                Value = document.Observation.EffectiveDate?.Value,
                DatePart = document.Observation.EffectiveDate?.DatePart
            };

            var isValid = document.Size < Constants.FileConstants.EmisSizeLimit &&
                          Constants.FileConstants.FileTypes.WhiteListTypes.Contains(
                              document.Extension, StringComparer.OrdinalIgnoreCase);

            return new DocumentItem
            {
                DocumentIdentifier = document.DocumentGuid,
                EventGuid = document.Observation.EventGuid,
                Name = name,
                Term = document.Observation.Term,
                Extension = document.Extension,
                Size = document.Size,
                EffectiveDate = effectiveDate,
                CodeId = document.Observation.CodeId,
                IsAvailable = document.Available,
                IsValidFile = isValid
            };
        }
    }
}