using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public interface IEmisMedicationMapper
    {
        Medications Map(MedicationRequestsGetResponse allergiesGetResponse);          
    }
}