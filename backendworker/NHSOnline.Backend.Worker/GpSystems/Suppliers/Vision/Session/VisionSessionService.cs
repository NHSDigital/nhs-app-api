using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionService : ISessionService
    {
        private readonly IVisionClient _visionClient;

        public VisionSessionService(IVisionClient visionClient)
        {
            _visionClient = visionClient;
        }

        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode, string nhsNumber)
        {
            try
            {
                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();

                var response = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (!response.HasErrorResponse)
                {
                    return new SessionCreateResult.SuccessfullyCreated(
                        response.Body.Account.Name,
                        new VisionUserSession
                        {
                            RosuAccountId = visionConnectionToken.RosuAccountId,
                            OdsCode = odsCode,
                            Key = visionConnectionToken.ApiKey,
                            NhsNumber = nhsNumber,
                            PatientId = response.Body.Account.PatientId,
                        }
                    );
                }

                return GetCorrectErrorResult(response);
            }
            catch (HttpRequestException)
            {
                return new SessionCreateResult.SupplierSystemUnavailable();
            }
        }

        // Vision does not have a logoff endpoint, returning successfully deleted
        public Task<SessionLogoffResult> Logoff(UserSession userSession)
        {
            return Task.FromResult((SessionLogoffResult) new SessionLogoffResult.SuccessfullyDeleted(userSession));
        }

        private static SessionCreateResult GetCorrectErrorResult<T>(VisionApiObjectResponse<T> response)
        {
            if (response.IsInvalidRequestError)
            {
                return new SessionCreateResult.InvalidRequest();
            }

            if (response.IsInvalidUserCredentialsError)
            {
                return new SessionCreateResult.InvalidUserCredentials();
            }

            if (response.IsInvalidSecurtyHeaderError)
            {
                return new SessionCreateResult.ErrorProcessingSecurityHeader();
            }

            if (response.IsUnknownError)
            {
                return new SessionCreateResult.UnknownError();
            }

            return new SessionCreateResult.SupplierSystemUnavailable();
        }
    }
}