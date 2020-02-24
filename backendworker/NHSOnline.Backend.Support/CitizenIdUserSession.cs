using System;

namespace NHSOnline.Backend.Support
{
    [Serializable]
    public class CitizenIdUserSession
    {
        public string AccessToken { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdTokenJti { get; set; }
    }
}