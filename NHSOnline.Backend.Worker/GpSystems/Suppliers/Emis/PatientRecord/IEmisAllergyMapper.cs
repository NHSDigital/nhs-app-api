using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public interface IEmisAllergyMapper
    {
        AllergyListResponse Map(AllergyRequestsGetResponse allergiesGetResponse);        
    }
}