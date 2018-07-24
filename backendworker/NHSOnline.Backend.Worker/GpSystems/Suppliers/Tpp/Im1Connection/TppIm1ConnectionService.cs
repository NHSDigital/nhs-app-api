using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Extensions;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Im1Connection
{
    public class TppIm1ConnectionService : IIm1ConnectionService
    {
        private readonly ITppClient _tppClient;
        private readonly ILogger<TppIm1ConnectionService> _logger;

        public TppIm1ConnectionService(ITppClient tppClient, ILogger<TppIm1ConnectionService> logger)
        {
            _tppClient = tppClient;
            _logger = logger;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                _logger.LogEnter(nameof(Verify));

                var authenticateRequest = connectionToken.DeserializeJson<Authenticate>();
                authenticateRequest.UnitId = odsCode;

                var authenticateReply = await _tppClient.AuthenticatePost(authenticateRequest);

                if (!authenticateReply.HasSuccessResponse)
                {
                    _logger.LogError("Tpp Authentication call failed");
                    return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
                }

                var nhsNumbers = authenticateReply.Body.ExtractNhsNumbers();

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = nhsNumbers
                };

                _logger.LogDebug("TppIm1ConnectionService Verify successfully completed");
                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e,
                    "Failed request to verify Tpp Im1ConnectionToken,HttpRequestException has been thrown.");
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
            finally
            {
                _logger.LogExit(nameof(Verify));
            }
        }

        public Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}