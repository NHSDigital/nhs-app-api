using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord
{
    internal interface ITppDcrEventsDocumentsMapper
    {
        PatientDocuments Map(RequestPatientRecordReply requestPatientRecordReply);
    }

    internal sealed class TppDcrEventsDocumentsMapper : ITppDcrEventsDocumentsMapper
    {

        private static ILogger<TppDcrEventsDocumentsMapper> _logger;

        public TppDcrEventsDocumentsMapper(ILogger<TppDcrEventsDocumentsMapper> logger)
        {
            _logger = logger;
        }

        public PatientDocuments Map(RequestPatientRecordReply requestPatientRecordReply)
        {
            _logger.LogEnter();
            if (requestPatientRecordReply == null)
            {
                _logger.LogError("RequestPatientRecordReply is null, throwing null argument exception");
                _logger.LogExit();
                throw new ArgumentNullException(nameof(requestPatientRecordReply));
            }

            _logger.LogExit();
            return new PatientDocuments
            {
                Data = MapEvents(requestPatientRecordReply.Events),
                HasAccess = true,
                HasErrored = false,
            };
        }

        private static IEnumerable<DocumentItem> MapEvents(List<Event> events)
        {
            if (events == null)
            {
                return new List<DocumentItem>();
            }

            return events.ToDictionary(
                    dcrEvent => dcrEvent,
                    dcrEvent => dcrEvent.Items.Where(eventItem =>
                        eventItem.Type.Equals("Letter", StringComparison.Ordinal)
                        || eventItem.Type.Equals("Attachment", StringComparison.Ordinal)))
                .SelectMany(eventItemPair =>
                    eventItemPair.Value.Select(item =>
                        MapDocument(
                            item,
                            new MyRecordDate
                            {
                                Value = eventItemPair.Key.Date.SafeParseToNullableDateTimeOffset(),
                            }
                        )
                    )
                )
                .ToList();
        }

        private static DocumentItem MapDocument(RequestPatientRecordItem inputEventItem, MyRecordDate date)
        {
            var match = GetRegexMatch(inputEventItem.Details);

            return new DocumentItem
            {
                EffectiveDate = date,
                DocumentIdentifier = inputEventItem.BinaryDataId,
                IsAvailable = true,
                Type = inputEventItem.Type.Equals("Attachment", StringComparison.Ordinal) ? "Document" : "Letter",
                IsValidFile = true,
                Comments =
                    string.IsNullOrEmpty(match.Groups[Constants.TppDocumentConstants.DocumentDetailsComments].ToString())
                    ? null
                    : match.Groups[Constants.TppDocumentConstants.DocumentDetailsComments].ToString(),
                Extension = match.Groups[Constants.TppDocumentConstants.DocumentDetailsExtension].ToString()
            };
        }

        private static Match GetRegexMatch(string inputEventItemDetails)
        {
            var pattern = new Regex(Constants.Regex.TppDocumentDetailsRegexWithComments);
            var match = pattern.Match(inputEventItemDetails);

            if (string.IsNullOrEmpty(match.Groups[Constants.TppDocumentConstants.DocumentDetailsComments].ToString()))
            {
                _logger.LogInformation("Regex failed to match with comments, using regex without comments");
                var patternNoComments = new Regex(Constants.Regex.TppDocumentDetailsRegexWithoutComments);
                return patternNoComments.Match(inputEventItemDetails);
            }

            return match;

        }
    }
}