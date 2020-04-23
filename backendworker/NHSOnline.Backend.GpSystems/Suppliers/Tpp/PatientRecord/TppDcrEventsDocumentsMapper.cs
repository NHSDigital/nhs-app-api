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

        private static string _documentDetailsComments;
        private static string _tppDocumentDetailsRegexWithoutComments;
        private static string _tppDocumentDetailsRegexWithComments;
        private static string _documentDetailsExtensionConstants;

        public TppDcrEventsDocumentsMapper(ILogger<TppDcrEventsDocumentsMapper> logger)
        {
            _logger = logger;
            _documentDetailsComments = Constants.TppDocumentConstants.DocumentDetailsComments;
            _tppDocumentDetailsRegexWithoutComments = Constants.Regex.TppDocumentDetailsRegexWithoutComments;
            _tppDocumentDetailsRegexWithComments = Constants.Regex.TppDocumentDetailsRegexWithComments;
            _documentDetailsExtensionConstants = Constants.TppDocumentConstants.DocumentDetailsExtension;
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
            _logger.LogEnter();
            if (events == null)
            {
                return new List<DocumentItem>();
            }

            _logger.LogExit();
            return events.ToDictionary(
                    dcrEvent => dcrEvent,
                    dcrEvent => dcrEvent.Items.Where(eventItem =>
                        "Letter".Equals(eventItem.Type, StringComparison.Ordinal)
                        || "Attachment".Equals(eventItem.Type, StringComparison.Ordinal)))
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
            _logger.LogEnter();
            var match = GetRegexMatch(inputEventItem.Details);

            var extension = match.Groups[_documentDetailsExtensionConstants].ToString();
            var comments = match.Groups[_documentDetailsComments].ToString();
            var type = "Attachment".Equals(inputEventItem.Type, StringComparison.Ordinal) ? "Document" : "Letter";

            _logger.LogExit();
            return new DocumentItem
            {
                EffectiveDate = date,
                DocumentIdentifier = inputEventItem.BinaryDataId,
                IsAvailable = true,
                Type = type,
                IsValidFile = Constants.FileConstants.FileTypes.WhiteListTypes.Contains(extension, StringComparer.OrdinalIgnoreCase),
                Comments = string.IsNullOrEmpty(comments) ? null : comments,
                Extension = extension,
                NeedMoreInformation = true
            };
        }

        private static Match GetRegexMatch(string inputEventItemDetails)
        {
            var pattern = new Regex(_tppDocumentDetailsRegexWithComments);
            var match = pattern.Match(inputEventItemDetails);

            if (!string.IsNullOrEmpty(match.Groups[_documentDetailsComments].ToString()))
            {
                return match;
            }

            _logger.LogInformation("Regex failed to match with comments, using regex without comments");
            var patternNoComments = new Regex(_tppDocumentDetailsRegexWithoutComments);
            return patternNoComments.Match(inputEventItemDetails);
        }
    }
}