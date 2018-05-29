using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Mappers
{
    public interface IEmisAllergyMapper
    {
        AllergyListResponse Map(AllergyRequestsGetResponse allergiesGetResponse);        
    }
}