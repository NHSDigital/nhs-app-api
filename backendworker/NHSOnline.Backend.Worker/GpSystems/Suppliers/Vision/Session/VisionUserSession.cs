using System.Collections.Generic;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionUserSession : UserSession
    {
        public override Supplier Supplier => Supplier.Vision;

        public string RosuAccountId { get; set; }

        public string OdsCode { get; set; }

        public string PatientId { get; set; }

        public string ApiKey { get; set; }
        
        public bool IsRepeatPrescriptionsEnabled { get; set; }

        public bool IsAppointmentsEnabled { get; set; } = true;

        public bool AllowFreeTextPrescriptions { get; set; }

        public List<string> OwnerIds { get; set; }

        public List<string> LocationIds { get; set; }
    }
}
