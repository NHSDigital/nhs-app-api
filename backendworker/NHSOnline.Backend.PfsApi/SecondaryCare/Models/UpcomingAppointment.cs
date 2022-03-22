using System;
using System.Diagnostics.CodeAnalysis;

namespace NHSOnline.Backend.PfsApi.SecondaryCare.Models
{
    public class UpcomingAppointment
    {
        public string AppointmentId { get; set; }

        public DateTimeOffset AppointmentDateTime { get; set; }

        public string LocationDescription { get; set; }

        public string ServiceSpeciality { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Intentional; we wish to expose this as a string, no intention to parse the URL")]
        public string DeepLinkUrl { get; set; }

        public string Provider { get; set; }
    }
}