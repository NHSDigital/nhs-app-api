using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class MeApplicationsPostRequest
    {
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

        public LinkageDetails LinkageDetails { get; set; }
    }
}