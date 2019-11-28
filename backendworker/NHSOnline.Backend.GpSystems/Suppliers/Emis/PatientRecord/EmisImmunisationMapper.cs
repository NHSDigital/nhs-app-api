using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using MedicationRootObject = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord.MedicationRootObject;
using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
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

            immunisations.Data = (medicalRecord.Immunisations ?? Enumerable.Empty<Immunisation>())
                .Where(i => !string.IsNullOrEmpty(i.Term))
                .Select(x =>
                    new ImmunisationItem
                    {
                        Term = x.Term,
                        EffectiveDate = x.EffectiveDate != null ? new MyRecordDate
                        {
                            Value = x.EffectiveDate?.Value,
                            DatePart = x.EffectiveDate?.DatePart
                        } : new MyRecordDate()
                    });
            return immunisations;
        }   
    }
}