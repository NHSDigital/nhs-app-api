using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.Im1Connection;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Im1Connection
{
    public class MicrotestIm1ConnectionService : IIm1ConnectionService
    {
        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            var response = await Task.FromResult(new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken,
                NhsNumbers = new List<PatientNhsNumber>(),
                OdsCode = odsCode
            });

            return new Im1ConnectionVerifyResult.Success(response);
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            var connectionToken = new MicrotestConnectionToken();

            var response = await Task.FromResult(new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken.SerializeJson(),
                NhsNumbers = new List<PatientNhsNumber>(),
                OdsCode = request.OdsCode,
                AccountId = request.AccountId,
                LinkageKey = request.LinkageKey
            });

            return new Im1ConnectionRegisterResult.Success(response);
        }
    }
}