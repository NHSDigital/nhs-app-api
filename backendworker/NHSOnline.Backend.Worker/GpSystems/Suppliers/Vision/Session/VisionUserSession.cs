using System.Collections.Generic;
using NHSOnline.Backend.Worker.GpSystems.SharedModels;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionUserSession : GpUserSession, IVisionUserSession
    {
        public override Supplier Supplier => Supplier.Vision;

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
