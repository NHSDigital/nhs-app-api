using System.Linq;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using MedicationRootObject = NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord.MedicationRootObject;
using System;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord
{
    internal sealed class EmisImmunisationMapper
    {
        internal Immunisations Map(MedicationRootObject immunisationsGetResponse)
        {
            if (immunisationsGetResponse == null)
            {
                throw new ArgumentNullException(nameof(immunisationsGetResponse));
            }

            var immunisations = new Immunisations();

            if (immunisationsGetResponse.MedicalRecord == null)
            {
                return immunisations;
            }

            var medicalRecord = immunisationsGetResponse.MedicalRecord;

            immunisations.Data = (medicalRecord.Immunisations ?? Enumerable.Empty<Immunisation>())
                .Where(i => !string.IsNullOrEmpty(i.Term))
                .Select(Map);

            return immunisations;
        }

        private static ImmunisationItem Map(Immunisation immunisation)
            => new ImmunisationItem
            {
                Term = immunisation.Term,
                EffectiveDate = immunisation.EffectiveDate != null ? new MyRecordDate
                {
                    Value = immunisation.EffectiveDate?.Value,
                    DatePart = immunisation.EffectiveDate?.DatePart
                } : new MyRecordDate()
            };
    }
}