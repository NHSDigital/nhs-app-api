using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication;
using MedicationRootObject = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication.MedicationRootObject;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisImmunisationMapper
    {
        public Immunisations Map(MedicationRootObject immunisationsGetResponse)
        {
            var immunisations = new Immunisations();

            if (immunisationsGetResponse.MedicalRecord == null) return immunisations;
            
            var medicalRecord = immunisationsGetResponse.MedicalRecord;

            immunisations.Data = (medicalRecord.Immunisations ?? Enumerable.Empty<Immunisation>()).Select(x =>
                new ImmunisationItem
                {
                    Term = x.Term,
                    EffectiveDate = new Date { Value = x.EffectiveDate.Value, DatePart = x.EffectiveDate.DatePart }
                });

            return immunisations;
        }   
    }
}