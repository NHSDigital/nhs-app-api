using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Extensions
{
    public static class DemographicsGetResponseExtensions
    {
        public static IEnumerable<PatientNhsNumber> ExtractNhsNumbers(this DemographicsGetResponse demographicsResponse)
        {
            var patientIdentifiers = demographicsResponse?.PatientIdentifiers;

            if (patientIdentifiers == null)
            {
                return Enumerable.Empty<PatientNhsNumber>();
            }

            return patientIdentifiers
                .Where(x => x.IdentifierType == IdentifierType.NhsNumber)
                .Select(x => new PatientNhsNumber
                {
                    NhsNumber = FormatNhsNumber(x.IdentifierValue)
                });
        }
        
        private static string FormatNhsNumber(string sourceNhsNumber) {
    
            if (string.IsNullOrEmpty(sourceNhsNumber)) return "";

            // Belt and braces here, apparantly the nhsnumber will always be 10 long,
            // if not, jut return whatever it is
            if (sourceNhsNumber.Length < 10) return sourceNhsNumber;
            
            return string.Format("{0} {1} {2}", 
                sourceNhsNumber.Substring(0, 3),
                sourceNhsNumber.Substring(3, 3),
                sourceNhsNumber.Substring(6, 4));
        }
    }
}
