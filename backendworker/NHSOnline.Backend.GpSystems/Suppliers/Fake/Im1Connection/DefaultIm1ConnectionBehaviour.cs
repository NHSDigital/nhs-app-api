using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.GpSystems.Im1Connection.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Models.Im1Connection;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.Im1Connection
{
    public class DefaultIm1ConnectionBehaviour : IIm1ConnectionBehaviour
    {
        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            var token = connectionToken.DeserializeJson<FakeConnectionToken>();
            var response = new PatientIm1ConnectionResponse
            {
                ConnectionToken = connectionToken,
                NhsNumbers = new List<PatientNhsNumber>
                {
                    new PatientNhsNumber
                    {
                        NhsNumber = token.NhsNumber
                    }
                },
                OdsCode = odsCode
            };

            return await Task.FromResult<Im1ConnectionVerifyResult>(new Im1ConnectionVerifyResult.Success(response));
        }

        public async Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request,
            FakeConnectionToken connectionToken)
        {
            var response = new CreateIm1ConnectionResponse
            {
                ConnectionToken = connectionToken.SerializeJson(),
                NhsNumbers = new List<PatientNhsNumber>
                {
                    new PatientNhsNumber
                    {
                        NhsNumber = connectionToken.NhsNumber
                    }
                },
                OdsCode = request.OdsCode,
                AccountId = request.AccountId,
                LinkageKey = request.LinkageKey,
            };

            return await Task.FromResult<Im1ConnectionRegisterResult>(new Im1ConnectionRegisterResult.Success(response));
        }
    }
}