using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class EmisAllergyMapper
    {
        internal Allergies Map(MedicationRootObject allergiesGetResponse)
        {
            if (allergiesGetResponse == null)
            {
                throw new System.ArgumentNullException(nameof(allergiesGetResponse));
            }

            var allergies = new Allergies();

            if (allergiesGetResponse.MedicalRecord != null)
            {
                var medicalRecord = allergiesGetResponse.MedicalRecord;

                allergies.Data = (medicalRecord.Allergies ?? Enumerable.Empty<Allergy>()).Select(Map).ToList();
            }

            return allergies;
        }

        private static AllergyItem Map(Allergy allergy)
        {
            return new AllergyItem
            {
                Name = allergy.Term,
                Date = allergy.EffectiveDate != null ?
                    new MyRecordDate { Value = allergy.EffectiveDate.Value, DatePart = allergy.EffectiveDate.DatePart } :
                    new MyRecordDate()
            };
        }
    }
}