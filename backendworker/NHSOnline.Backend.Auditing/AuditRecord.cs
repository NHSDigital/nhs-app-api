using System;
using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Auditing
{
    [Serializable]
    public class AuditRecord : RepositoryRecord
    {
        public AuditRecord(
            DateTime timestamp,
            string nhsLoginSubject,
            string nhsNumber,
            bool isActingOnBehalfOfAnother,
            Supplier supplier,
            string operation,
            string details,
            VersionTag version,
            string environment,
            string integrationReferrer,
            string referrerOrigin,
            string sessionId,
            string proofLevel,
            string ods,
            string referrer,
            string providerId,
            string providerName,
            string jumpOffId)
        {
            Timestamp = timestamp;
            AuditId = Guid.NewGuid().ToString();
            NhsLoginSubject = nhsLoginSubject;
            NhsNumber = nhsNumber;
            IsActingOnBehalfOfAnother = isActingOnBehalfOfAnother;
            Supplier = supplier.ToString();
            Operation = operation;
            Details = details;
            IntegrationReferrer = integrationReferrer;
            ReferrerOrigin = referrerOrigin;
            SessionId = sessionId;
            ProofLevel = proofLevel;
            ODS = ods;
            Referrer = referrer;
            ProviderId = providerId;
            ProviderName = providerName;
            JumpOffId = jumpOffId;

            if (version == null)
            {
                return;
            }

            ApiVersion = version.Api;
            WebVersion = version.Web;
            NativeVersion = version.Native;
            Environment = environment;
        }

        [BsonElement]
        public string AuditId { get; private set; }

        [BsonElement]
        public string NhsLoginSubject { get; private set; }

        [BsonElement]
        public string NhsNumber { get; private set; }

        [BsonElement]
        public bool IsActingOnBehalfOfAnother { get; private set; }

        [BsonElement]
        public string Supplier { get; private set; }

        [BsonElement]
        public string Operation { get; private set; }

        [BsonElement]
        public string Details { get; private set; }

        [BsonElement]
        public string ApiVersion { get; private set; }

        [BsonElement]
        public string WebVersion { get; private set; }

        [BsonElement]
        public string NativeVersion { get; private set; }

        [BsonElement]
        public string Environment { get; private set; }

        [BsonElement]
        public string IntegrationReferrer { get; private set; }

        [BsonElement]
        public string ReferrerOrigin { get; private set; }

        [BsonElement]
        public string SessionId { get; private set; }

        [BsonElement]
        public string ProofLevel { get; private set; }

        [BsonElement]
        public string ODS { get; private set; }

        [BsonElement]
        public string Referrer { get; private set; }

        [BsonElement]
        public string ProviderId { get; private set; }

        [BsonElement]
        public string ProviderName { get; private set; }

        [BsonElement]
        public string JumpOffId { get; private set; }
    }
}
