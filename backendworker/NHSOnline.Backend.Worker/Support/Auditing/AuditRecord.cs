using System;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    [Serializable]
    public class AuditRecord
    {
        public AuditRecord(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details)
        {
            Timestamp = timestamp;
            NhsNumber = nhsNumber;
            Supplier = supplier.ToString();
            Operation = operation;
            Details = details;
        }
        
        public DateTime Timestamp { get; private set; }
        public string NhsNumber { get; private set; }
        public string Supplier { get; private set; }
        public string Operation { get; private set; }
        public string Details { get; private set; }
    }
}