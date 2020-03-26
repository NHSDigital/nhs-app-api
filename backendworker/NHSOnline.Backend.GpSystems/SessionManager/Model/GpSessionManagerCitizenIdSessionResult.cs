using System;

namespace NHSOnline.Backend.GpSystems.SessionManager.Model
{
    public class GpSessionManagerCitizenIdSessionResult
    {
        public string OdsCode { get; set; }
        public string Im1ConnectionToken { get; set; }
        public string NhsNumber { get; set; }
        public GpSessionManagerCitizenIdUserSession Session { get; set; }
    }

    public class GpSessionManagerCitizenIdUserSession
    {
        public string AccessToken { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdTokenJti { get; set; }
    }
}