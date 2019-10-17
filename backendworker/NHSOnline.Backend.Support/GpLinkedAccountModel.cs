using System;

namespace NHSOnline.Backend.Support
{
    public class GpLinkedAccountModel
    {
        public GpLinkedAccountModel(GpUserSession gpUserSession)
        {
            GpUserSession = gpUserSession;
            PatientId = gpUserSession.Id;
        }
        public GpLinkedAccountModel(GpUserSession gpUserSession, Guid id)
        {
            GpUserSession = gpUserSession;
            PatientId = id;
        }
        
        public GpUserSession GpUserSession { get; set; }
        public Guid PatientId { get; set; }
    }
}