using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    public interface ITppDcrEventsDocumentsMapper
    {
        PatientDocuments Map(RequestPatientRecordReply requestPatientRecordReply);
    }

    public class TppDcrEventsDocumentsMapper : ITppDcrEventsDocumentsMapper
    {

        public PatientDocuments Map(RequestPatientRecordReply requestPatientRecordReply)
        {
            if (requestPatientRecordReply == null)
            {
                throw new ArgumentNullException(nameof(requestPatientRecordReply));
            }

            return new PatientDocuments
            {
                Data = MapEvents(requestPatientRecordReply.Events),
                HasAccess = true,
                HasErrored = false,
            };
        }

        private IEnumerable<DocumentItem> MapEvents(List<Event> events)
        {
            if (events == null)
            {
                return new List<DocumentItem>();
            }

            return events?.ToDictionary(
                    dcrEvent => dcrEvent,
                    dcrEvent => dcrEvent.Items.Where(eventItem => eventItem.Type.Equals("Letter", StringComparison.Ordinal)
                                             || eventItem.Type.Equals("Attachment", StringComparison.Ordinal))
                )
                .SelectMany(eventItemPair =>
                    eventItemPair.Value.Select(item =>
                        MapDocument(
                            item,
                            new MyRecordDate()
                            {
                                Value = eventItemPair.Key.Date.SafeParseToNullableDateTimeOffset(),
                            }
                        )
                    )
                )
                .ToList();
        }

        private DocumentItem MapDocument(RequestPatientRecordItem inputEventItem, MyRecordDate date)
        {
            return new DocumentItem
            {
                EffectiveDate = date,
                DocumentIdentifier = inputEventItem.BinaryDataId,
                IsAvailable = true,
                Type = inputEventItem.Type.Equals("Attachment", StringComparison.Ordinal) ? "Document" : "Letter"
            };
        }
    }
}