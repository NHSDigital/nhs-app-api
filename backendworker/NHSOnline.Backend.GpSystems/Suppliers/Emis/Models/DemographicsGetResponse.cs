using System;
using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models
{
    public class DemographicsGetResponse
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public ContactDetails ContactDetails { get; set; }
        public EmisAddress Address { get; set; }
        public IEnumerable<PatientIdentifier> PatientIdentifiers { get; set; }

        public IEnumerable<PatientNhsNumber> ExtractNhsNumbers()
        {
            if (PatientIdentifiers == null)
            {
                return Enumerable.Empty<PatientNhsNumber>();
            }

            return PatientIdentifiers
                .Where(x => x.IdentifierType == IdentifierType.NhsNumber)
                .Select(x => new PatientNhsNumber
                {
                    NhsNumber = x.IdentifierValue.FormatToNhsNumber()
                });
        }
    }
}