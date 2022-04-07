using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSOnline.Backend.Users.Areas.Devices.Models;

namespace NHSOnline.Backend.Users.Areas.Devices.Models
{
    public class NotificationOutcomeResponse
    {
        public string State { get; set; }

        public DateTime? EnqueueTime { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [SuppressMessage("Design", "CA1056:URI-like properties should not be strings")]
        public string PnsErrorDetailsUri { get; set; }

        public IEnumerable<PlatformOutcome> PlatformOutcomes { get; set; }
    }
}