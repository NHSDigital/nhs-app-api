using System;
using NHSOnline.Backend.Worker.CitizenId;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker
{
    [Serializable]
    public class UserSession
    {
        public string Key { get; set; }

        public string CsrfToken { get; set; }
        
        public GpUserSession GpUserSession { get; set; }

        public CitizenIdUserSession CitizenIdUserSession { get; set; }

        public Guid OrganDonationSessionId { get; set; }
    }
}