using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection
{
    public class TppIm1ConnectionService : IIm1ConnectionService
    {
        private readonly ITppClient _tppClient;

        public TppIm1ConnectionService(ITppClient tppClient)
        {
            _tppClient = tppClient;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                var authenticateRequest = connectionToken.DeserializeJson<Authenticate>();
                authenticateRequest.UnitId = odsCode;
                
                var authenticateReply = await _tppClient.AuthenticatePost(authenticateRequest);

                if (!authenticateReply.HasSuccessResponse)
                {
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumber = authenticateReply.Body?.User?.Person?.NationalId?.Value;
                var nhsNumbers = Enumerable.Empty<PatientNhsNumber>();
                
                if (nhsNumber != null)
                {
                    nhsNumbers = new List<PatientNhsNumber>
                    {
                        new PatientNhsNumber
                        {
                            NhsNumber = nhsNumber.FormatToNhsNumber()
                        }
                    };
                }  

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = nhsNumbers
                };

                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException)
            {
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();   
            }
        }

        public Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}