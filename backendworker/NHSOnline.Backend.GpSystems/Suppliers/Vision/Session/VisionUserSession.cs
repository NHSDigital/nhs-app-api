using System.Collections.Generic;
using NHSOnline.Backend.GpSystems.SharedModels;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision.Session
{
    public class VisionUserSession : GpUserSession, IVisionUserSession
    {
        public override Supplier Supplier => Supplier.Vision;
        
        public override bool HasLinkedAccounts => false;

        public string RosuAccountId { get; set; }

        public string PatientId { get; set; }

        public string ApiKey { get; set; }
        
        public bool IsRepeatPrescriptionsEnabled { get; set; }

        public bool IsAppointmentsEnabled { get; set; } = true;

        public bool AllowFreeTextPrescriptions { get; set; }

        public Necessity AppointmentBookingReasonNecessity { get; set; } = Necessity.Optional;

        public List<string> LocationIds { get; set; }
    }
}
