using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisAllergyMapper
    {
        public Allergies Map(MedicationRootObject allergiesGetResponse)
        {
            var allergies = new Allergies();
            
            if (allergiesGetResponse.MedicalRecord != null)
            {
                var medicalRecord = allergiesGetResponse.MedicalRecord;

                allergies.Data = (medicalRecord.Allergies ?? Enumerable.Empty<Allergy>()).Select(x =>
                    new AllergyItem
                    {
                        Name = x.Term,
                        Date = x.EffectiveDate != null ? 
                                        new Date { Value = x.EffectiveDate.Value, DatePart = x.EffectiveDate.DatePart } :
                                        null,
                    });
            }

            return allergies;
        }
    }
}