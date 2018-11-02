using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.Session;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.Appointments;
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
                        response.Body.Configuration.Account.Name,
                        new VisionUserSession
                        {
                            RosuAccountId = visionConnectionToken.RosuAccountId,
                            OdsCode = odsCode,
                            ApiKey = visionConnectionToken.ApiKey,
                            NhsNumber = nhsNumber,
                            PatientId = response.Body.Configuration.Account.PatientId,
                            IsRepeatPrescriptionsEnabled = response.Body.Configuration.Prescriptions.RepeatEnabled,
                            IsAppointmentsEnabled = response.Body.Configuration.Appointments.BookingEnabled,
                            LocationIds = GetLocationIds(response),
                            OwnerIds = GetOwnerIds(response)
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

        private static List<string> GetLocationIds(VisionApiObjectResponse<PatientConfigurationResponse> response) =>
            response.Body.Configuration.References?.Locations != null
                ? response.Body.Configuration.References.Locations.Select(l => l.Id).ToList()
                : new List<string>();

        private static List<string> GetOwnerIds(VisionApiObjectResponse<PatientConfigurationResponse> response) =>
            response.Body.Configuration.References?.Owners != null
                ? response.Body.Configuration.References.Owners.Select(o => o.Id).ToList()
                : new List<string>();

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

            if (response.IsInvalidSecurityHeaderError)
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