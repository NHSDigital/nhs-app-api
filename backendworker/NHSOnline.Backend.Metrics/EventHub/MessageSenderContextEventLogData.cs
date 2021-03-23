using System;
using System.Collections.Generic;
using NHSOnline.Backend.Metrics.Extensions;

namespace NHSOnline.Backend.Metrics.EventHub
{
    public class MessageSenderContextEventLogData : IEventLogData
    {
        private readonly string _supplierId;
        private readonly string _communicationId;
        private readonly string _transmissionId;
        private readonly DateTime? _communicationCreatedDateTime;
        private readonly string _requestReference;
        private readonly string _campaignId;
        private readonly string _odsCode;
        private readonly string _nhsNumber;
        private readonly string _nhsLoginId;

        public MessageSenderContextEventLogData(string supplierId, string communicationId,
            string transmissionId, DateTime? communicationCreatedDateTime, string requestReference, string campaignId,
            string odsCode, string nhsNumber, string nhsLoginId)
        {
            _supplierId = supplierId;
            _communicationId = communicationId;
            _transmissionId = transmissionId;
            _communicationCreatedDateTime = communicationCreatedDateTime;
            _requestReference = requestReference;
            _campaignId = campaignId;
            _odsCode = odsCode;
            _nhsNumber = nhsNumber;
            _nhsLoginId = nhsLoginId;
        }

        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(bool pidAllowed)
        {
            yield return new KeyValuePair<string, string>("SupplierId", _supplierId);
            yield return new KeyValuePair<string, string>("CommunicationId", _communicationId);
            yield return new KeyValuePair<string, string>("TransmissionId", _transmissionId);
            yield return new KeyValuePair<string, string>("CommunicationCreatedDateTime", _communicationCreatedDateTime?.ToSplunkString());
            yield return new KeyValuePair<string, string>("RequestReference", _requestReference);
            yield return new KeyValuePair<string, string>("CampaignId", _campaignId);
            yield return new KeyValuePair<string, string>("OdsCode", _odsCode);
            yield return new KeyValuePair<string, string>("NhsLoginId", _nhsLoginId);

            if (pidAllowed)
            {
                yield return new KeyValuePair<string, string>("NhsNumber", _nhsNumber);
            }
        }
    }
}