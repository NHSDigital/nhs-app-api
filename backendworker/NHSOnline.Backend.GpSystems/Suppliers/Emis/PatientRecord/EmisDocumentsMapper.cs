using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisDocumentsMapper
    {

      public PatientDocuments Map(MedicationRootObject documentsGetResponse)
      {
          if (documentsGetResponse == null)
          {
              throw new ArgumentNullException(nameof(documentsGetResponse));
          }

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
                : null;

                if (document.Observation.AssociatedText != null)
                {
                    documentItem.Notes = new List<String>();
                    documentItem.Notes = document.Observation.AssociatedText.Select(x => x.Text).ToList();                            
                }

                documentItem.IsAvailable = document.Available;
                documentItem.Size = document.Size;
                documentItem.Extension = document.Extension;

                documentItem.Term = document.Observation.Term;
                documentItem.DocumentGuid = document.DocumentGuid;

                documentData.Add(documentItem);
              }

              documents.Data = documentData;
              
            }
          }

          return documents;
      }
    }
}