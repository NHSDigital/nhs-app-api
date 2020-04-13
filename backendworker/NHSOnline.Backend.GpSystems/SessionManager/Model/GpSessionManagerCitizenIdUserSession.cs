using System;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.SessionManager.Model
{
    public class GpSessionManagerCitizenIdUserSession
    {
        public string AccessToken { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string IdTokenJti { get; set; }
        public ProofLevel ProofLevel { get; set; }
    }
}