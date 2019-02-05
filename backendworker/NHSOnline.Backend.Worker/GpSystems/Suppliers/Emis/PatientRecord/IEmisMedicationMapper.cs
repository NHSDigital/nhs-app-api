using NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisMedicationMapper
    {
        Medications Map(MedicationRootObject medicationRootObject);          
    }
}