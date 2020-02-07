using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisDocumentsMapper
    {

      public PatientDocuments Map(MedicationRootObject documentsGetResponse)
      {
        var documents = new PatientDocuments();

          if (documentsGetResponse.MedicalRecord != null)
          {
            var medicalRecord = documentsGetResponse.MedicalRecord;
            var documentData = new List<DocumentItem>();

            if (medicalRecord.Documents != null)
            {

              foreach(var document in medicalRecord.Documents)
              {

                var documentItem = new DocumentItem();

                documentItem.EffectiveDate = document.Observation.EffectiveDate != null
                ? new MyRecordDate
                {
                    Value = document.Observation.EffectiveDate.Value,
                    DatePart = document.Observation.EffectiveDate.DatePart
                }
                : new MyRecordDate();

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

                documentData.Add(documentItem);
              }

              documents.Data = documentData;
            }
          }

          return documents;
      }
    }
}