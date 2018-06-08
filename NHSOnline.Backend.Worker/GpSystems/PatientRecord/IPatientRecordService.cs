using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public interface IPatientRecordService
    {
        Task<GetAllergyResult> GetPatientAllergies(UserSession userSession);        
    }
}