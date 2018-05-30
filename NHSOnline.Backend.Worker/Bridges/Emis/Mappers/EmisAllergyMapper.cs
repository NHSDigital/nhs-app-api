using System;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Mappers
{
    public class EmisAllergyMapper : IEmisAllergyMapper
    {
        public AllergyListResponse Map(AllergyRequestsGetResponse allergiesGetResponse)
        {
            if (allergiesGetResponse == null)
            {
                throw new ArgumentNullException(nameof(allergiesGetResponse));
            }

            var result = new AllergyListResponse
            {
                Allergies = (allergiesGetResponse.Allergies ?? Enumerable.Empty<AllergyResponse>()).Select(x => new AllergyItem
                {
                    AllergyName = x.Term,
                    AvailabilityDate = x.AvailabilityDateTime,
                }),
            };

            return result;
        }
    }
}