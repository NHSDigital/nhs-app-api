using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models
{
    public class DemographicsGetResponse
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public EmisAddress Address { get; set; }
        public IEnumerable<PatientIdentifier> PatientIdentifiers { get; set; }
        
        
    }
}