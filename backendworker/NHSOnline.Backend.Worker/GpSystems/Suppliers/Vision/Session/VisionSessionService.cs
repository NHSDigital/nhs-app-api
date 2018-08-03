using System;
using System.Diagnostics.CodeAnalysis;
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

        [SuppressMessage("Microsoft.Usage", "CA2201", Justification = "Raised bug NHSO-2040 to fix inconsistent approaches when no NHS number")]
        public async Task<SessionCreateResult> Create(string connectionToken, string odsCode)
        {
            try
            {
                var visionConnectionToken = connectionToken.DeserializeJson<VisionConnectionToken>();
                
                var response = await _visionClient.GetConfiguration(visionConnectionToken, odsCode);

                if (!response.HasErrorResponse)
                {
                    var nhsNumber = response.Body.Account.PatientNumbers.FirstOrDefault(x => "NHS".Equals(x.NumberType, StringComparison.Ordinal));

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