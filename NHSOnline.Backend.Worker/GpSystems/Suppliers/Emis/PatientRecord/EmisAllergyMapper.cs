using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisAllergyMapper
    {
        public Allergies Map(AllergyRequestsGetResponse allergiesGetResponse)
        {
            var allergies = new Allergies();
            
            if (allergiesGetResponse.MedicalRecord != null)
            {
                var medicalRecord = allergiesGetResponse.MedicalRecord;

                allergies.Data = (medicalRecord.Allergies ?? Enumerable.Empty<AllergyResponse>()).Select(x =>
                    new AllergyItem
                    {
                        Name = x.Term,
                        Date = x.AvailabilityDateTime
                    });
            }

            return allergies;
        }
    }
}