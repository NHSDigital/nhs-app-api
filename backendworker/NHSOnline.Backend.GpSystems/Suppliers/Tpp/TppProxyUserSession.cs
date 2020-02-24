using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppProxyUserSession
    {
        public Guid Id { get; set; }
        
        public string Suid { get; set; }
        
        public string PatientId { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        
        public string NhsNumber { get; set; }

        public string Name { get; set; }
    }
}
