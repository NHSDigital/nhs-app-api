using System;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    [Serializable]
    public class AuditRecord
    {
        public AuditRecord(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details, VersionTag version)
        {
            Timestamp = timestamp;
            NhsNumber = nhsNumber;
            Supplier = supplier.ToString();
            Operation = operation;
            Details = details;

            if (version == null) return;
            
            ApiVersion = version.Api;
            WebVersion = version.Web;
            NativeVersion = version.Native;
        }
        
        public DateTime Timestamp { get; private set; }
        public string NhsNumber { get; private set; }
        public string Supplier { get; private set; }
        public string Operation { get; private set; }
        public string Details { get; private set; }
        
        public string ApiVersion { get; private set; }
        public string WebVersion { get; private set; }
        public string NativeVersion { get; private set; }
    }
}
