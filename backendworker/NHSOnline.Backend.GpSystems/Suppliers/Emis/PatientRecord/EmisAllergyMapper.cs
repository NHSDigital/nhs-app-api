using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisAllergyMapper
    {
        public Allergies Map(MedicationRootObject allergiesGetResponse)
        {
            if(allergiesGetResponse == null)
            {
                throw new System.ArgumentNullException(nameof(allergiesGetResponse));
            }

            var allergies = new Allergies();
            
            if (allergiesGetResponse.MedicalRecord != null)
            {
                var medicalRecord = allergiesGetResponse.MedicalRecord;

                allergies.Data = (medicalRecord.Allergies ?? Enumerable.Empty<Allergy>()).Select(x =>
                    new AllergyItem
                    {
                        Name = x.Term,
                        Date = x.EffectiveDate != null ? 
                                        new MyRecordDate { Value = x.EffectiveDate.Value, DatePart = x.EffectiveDate.DatePart } :
                                        null,
                    });
            }

            return allergies;
        }
    }
}