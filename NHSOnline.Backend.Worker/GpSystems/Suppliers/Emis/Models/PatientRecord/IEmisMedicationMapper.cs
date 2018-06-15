using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Medication;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public interface IEmisMedicationMapper
    {
        Medications Map(MedicationRootObject medicationRootObject);          
    }
}