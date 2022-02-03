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
            string sessionId,
            string proofLevel,
            string ods,
            string referrer)
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
            SessionId = sessionId;
            ProofLevel = proofLevel;
            ODS = ods;
            Referrer = referrer;

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
        public string SessionId { get; private set; }

        [BsonElement]
        public string ProofLevel { get; private set; }

        [BsonElement]
        public string ODS { get; private set; }

        [BsonElement]
        public string Referrer { get; private set; }
   }
}
