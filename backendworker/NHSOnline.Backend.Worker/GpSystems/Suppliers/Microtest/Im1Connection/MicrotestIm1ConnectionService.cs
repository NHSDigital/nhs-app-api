using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Models.Im1Connection;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Microtest.Im1Connection
{
    public class MicrotestIm1ConnectionService : IIm1ConnectionService
    {
        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            var response = await Task.FromResult(new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken,
                NhsNumbers = new List<PatientNhsNumber>(),
            });

            return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            var connectionToken = new MicrotestConnectionToken();

            var response = await Task.FromResult(new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken.SerializeJson(),
                NhsNumbers = new List<PatientNhsNumber>(),
            });

            return new Im1ConnectionRegisterResult.SuccessfullyRegistered(response);
        }
    }
}