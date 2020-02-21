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
            var documents = new PatientDocuments();

            var medicalRecord = documentsGetResponse.MedicalRecord;

            if (medicalRecord?.Documents != null)
            {
                documents.Data = medicalRecord.Documents.Select(Map).ToList();
            }

            return documents;
        }

        private static DocumentItem Map(Document document)
        {
            var documentItem = new DocumentItem
            {
                EffectiveDate = document.Observation.EffectiveDate != null
                    ? new MyRecordDate
                    {
                        Value = document.Observation.EffectiveDate.Value,
                        DatePart = document.Observation.EffectiveDate.DatePart
                    }
                    : new MyRecordDate()
            };

            if (document.Observation.AssociatedText != null)
            {
                documentItem.Name = document.Observation.AssociatedText.Select(x => x.Text).FirstOrDefault();
            }

            documentItem.IsAvailable = document.Available;
            documentItem.Size = document.Size;
            documentItem.Extension = document.Extension;

            documentItem.Term = document.Observation.Term;
            documentItem.EventGuid = document.Observation.EventGuid;
            documentItem.CodeId = document.Observation.CodeId;
            documentItem.DocumentIdentifier = document.DocumentGuid;

            documentItem.IsValidFile = documentItem.Size != null && document.Size < 4000000 &&
                                       Constants.FileConstants.FileTypes.WhiteListTypes.Contains(document.Extension);
            return documentItem;
        }
    }
}