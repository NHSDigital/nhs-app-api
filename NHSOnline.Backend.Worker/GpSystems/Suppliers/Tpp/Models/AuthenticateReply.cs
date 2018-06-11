using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models
{
    [Serializable]
    public class AuthenticateReply
    {        
        public User User { get; set; }
    }
}