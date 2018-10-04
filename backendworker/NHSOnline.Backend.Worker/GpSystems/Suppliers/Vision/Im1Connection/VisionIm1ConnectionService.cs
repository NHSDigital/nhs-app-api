using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.Im1Connection.Models;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Im1Connection
{
    public class VisionIm1ConnectionService : IIm1ConnectionService
    {
        private readonly ILogger<VisionIm1ConnectionService> _logger;
        private readonly IVisionClient _visionClient;

        public VisionIm1ConnectionService(IVisionClient visionClient, ILogger<VisionIm1ConnectionService> logger)
        {
            _visionClient = visionClient;
            _logger = logger;
        }

        public async Task<Im1ConnectionVerifyResult> Verify(string connectionToken, string odsCode)
        {
            try
            {
                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();

                var getConfigurationReply = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (getConfigurationReply.HasErrorResponse)
                {
                    return GetCorrectErrorResult(getConfigurationReply);
                }

                var formattedNhsNumbers = getConfigurationReply.Body.Configuration.Account.PatientNumbers
                    .Select(x => new PatientNhsNumber
                    {
                        NhsNumber = x.Number.FormatToNhsNumber()
                    });

                var response = new PatientIm1ConnectionResponse
                {
                    ConnectionToken = connectionToken,
                    NhsNumbers = formattedNhsNumbers
                };

                return new Im1ConnectionVerifyResult.SuccessfullyVerified(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical("Critical Error sending configuration request to vision");
                _logger.LogCritical(ex.ToString());
                return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
            }
        }

        public Task<Im1ConnectionRegisterResult> Register(PatientIm1ConnectionRequest request)
        {
            _logger.LogCritical("Vision IM1 Registration not yet implemented!");
            throw new NotImplementedException();
        }

        private Im1ConnectionVerifyResult GetCorrectErrorResult<T>(VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                LogError<T>("Invalid Request");
                return new Im1ConnectionVerifyResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                LogError<T>("Invalid User Credentials");
                return new Im1ConnectionVerifyResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurtyHeaderError)
            {
                LogError<T>("Invalid Security Error");
                return new Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                LogError<T>("Unknown Error");
                return new Im1ConnectionVerifyResult.UnknownError();
            }
            
            LogError<T>("Other Error");
            return new Im1ConnectionVerifyResult.SupplierSystemUnavailable();
        }

        private void LogError<T>(string errorType)
        {
            _logger.LogError($"Vision IM1 Login - {errorType}");
        }
    }
}