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
                    NhsNumber = x.IdentifierValue.FormatToNhsNumber()
                });
        }
    }
}
