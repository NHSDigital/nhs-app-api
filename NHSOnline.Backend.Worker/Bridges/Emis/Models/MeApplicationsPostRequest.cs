using System;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class MeApplicationsPostRequest
    {
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

        public LinkageDetails LinkageDetails { get; set; }
    }
}