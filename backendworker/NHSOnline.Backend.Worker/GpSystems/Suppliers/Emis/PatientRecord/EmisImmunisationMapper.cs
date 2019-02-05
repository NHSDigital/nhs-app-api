using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;
using MedicationRootObject = NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.MedicationRootObject;
using System;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisImmunisationMapper
    {
        public Immunisations Map(MedicationRootObject immunisationsGetResponse)
        {
            if(immunisationsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(immunisationsGetResponse));
            }

            var immunisations = new Immunisations();

            if (immunisationsGetResponse.MedicalRecord == null) return immunisations;
            
            var medicalRecord = immunisationsGetResponse.MedicalRecord;

            immunisations.Data = (medicalRecord.Immunisations ?? Enumerable.Empty<Immunisation>()).Select(x =>
                new ImmunisationItem
                {
                    Term = x.Term,
                    EffectiveDate = new MyRecordDate { Value = x.EffectiveDate.Value, DatePart = x.EffectiveDate.DatePart }
                });

            return immunisations;
        }   
    }
}