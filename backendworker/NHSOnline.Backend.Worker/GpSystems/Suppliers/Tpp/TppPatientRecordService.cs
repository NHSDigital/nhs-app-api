using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.PatientRecord;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp
{
    public class TppPatientRecordService: IPatientRecordService
    {
        public async Task<GetMyRecordResult> Get(UserSession userSession)
        {
            return new GetMyRecordResult.SuccessfullyRetrieved(new MyRecordResponse());
        }
    }
}