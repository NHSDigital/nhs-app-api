using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Router.MyRecord
{
    public interface IPatientRecordService
    {
        Task<GetAllergyResult> GetPatientAllergies(UserSession userSession);        
    }
}