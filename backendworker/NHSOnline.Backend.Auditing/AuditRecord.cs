using System;
using MongoDB.Bson.Serialization.Attributes;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.Auditing
{
    [Serializable]
    public class AuditRecord : MongoRecord
    {
        public AuditRecord(
            DateTime timestamp, 
            string nhsLoginSubject,
            string nhsNumber, 
            bool isActingOnBehalfOfAnother,
            Supplier supplier, 
            string operation, 
            string details, 
            VersionTag version)
        {
            Timestamp = timestamp;
            NhsLoginSubject = nhsLoginSubject;
            NhsNumber = nhsNumber;
            IsActingOnBehalfOfAnother = isActingOnBehalfOfAnother;
            Supplier = supplier.ToString();
            Operation = operation;
            Details = details;

            if (version == null) return;
            
            ApiVersion = version.Api;
            WebVersion = version.Web;
            NativeVersion = version.Native;
        }
        
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
    }
}
