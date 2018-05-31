using System;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
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