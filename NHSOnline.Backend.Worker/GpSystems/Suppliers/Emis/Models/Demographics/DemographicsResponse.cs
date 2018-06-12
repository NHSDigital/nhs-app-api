using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Demographics
{
    public class DemographicsResponse
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Sex { get; set; }
        public Address Address { get; set; }
        public string NhsNumber { get; set; }
    }
}