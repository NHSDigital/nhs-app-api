using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.Settings;
using static NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.VisionClient;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Session
{
    public class VisionSessionService : ISessionService
    {
        private readonly IVisionClient _visionClient;

        private readonly ConfigurationSettings _settings;

        public VisionSessionService(IVisionClient visionClient, IOptions<ConfigurationSettings> settings)
        {
            _visionClient = visionClient;
            _settings = settings.Value;
        }

        public async Task<SessionCreateResult> Create(string im1ConnectionToken, string odsCode)
        {
            try
            {
                var visionConnectionToken = im1ConnectionToken.DeserializeJson<VisionConnectionToken>();
                
                var response = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (!response.HasErrorResponse)
                {
                    var nhsNumber = response.Body.Account.PatientNumbers.FirstOrDefault(x => x.NumberType == "NHS");

                    if (nhsNumber == null)
                    {
                        throw new Exception("NHS Number Null");
                    }

                    return new SessionCreateResult.SuccessfullyCreated(
                        response.Body.Account.Name,
                        new VisionUserSession
                        {
                            RosuAccountId = visionConnectionToken.RosuAccountId,
                            OdsCode = odsCode,
                            Key = visionConnectionToken.ApiKey,
                            NhsNumber = nhsNumber.Number
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

        private SessionCreateResult GetCorrectErrorResult<T>(VisionApiObjectResponse<T> response)
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