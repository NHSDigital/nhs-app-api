using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.LinkedAccounts
{
    public class EmisLinkedAccountsService : ILinkedAccountsService
    {
        private readonly ILogger<EmisLinkedAccountsService> _logger;
        private readonly IEmisDemographicsService _demographicsService;
        private readonly IEmisClient _emisClient;

        public EmisLinkedAccountsService(
            ILogger<EmisLinkedAccountsService> logger,
            IEmisDemographicsService demographicsService,
            IEmisClient emisClient)
        {
            _logger = logger;
            _demographicsService = demographicsService;
            _emisClient = emisClient;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.OdsCode;
        }

        public bool IsValidAccountOrLinkedAccountId(GpUserSession gpUserSession, Guid id)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);
            return (proxy != null || emisUserSession.Id == id);
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            if (proxy == null)
            {
                _logger.LogError($"Proxy patient with id {id} not found in {nameof(emisUserSession.ProxyPatients)}");
                return await Task.FromResult(new LinkedAccountAccessSummaryResult.NotFound());
            }

            var tempProxyUserSession = new EmisUserSession
            {
                SessionId = emisUserSession.SessionId,
                EndUserSessionId = emisUserSession.EndUserSessionId,
                UserPatientLinkToken = proxy.UserPatientLinkToken,
            };

            _logger.LogInformation("Creating tasks to get linked account summary");

            try
            {
                var requestParameters = new EmisRequestParameters(tempProxyUserSession);
                var settings = await _emisClient.MeSettingsGet(requestParameters);

                _logger.LogInformation($"Finished call to {nameof(_emisClient.MeSettingsGet)}");

                if (settings.HasSuccessResponse)
                {
                    var response = new GetLinkedAccountAccessSummaryResponse
                    {
                        CanBookAppointment = settings.Body.AssignedServices.AppointmentsEnabled,
                        CanOrderRepeatPrescription = settings.Body.AssignedServices.PrescribingEnabled,
                        CanViewMedicalRecord = settings.Body.AssignedServices.MedicalRecordEnabled,
                    };

                    return new LinkedAccountAccessSummaryResult.Success(response);
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Unsuccessful request doing linked account summary requests");
                return new LinkedAccountAccessSummaryResult.BadGateway();
            }
            finally
            {
                _logger.LogExit();
            }

            _logger.LogError("Linked account summary requests not all successful");
            return new LinkedAccountAccessSummaryResult.BadGateway();
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            GetLinkedAccountsResponse response = new GetLinkedAccountsResponse();

            if (gpUserSession.HasLinkedAccounts)
            {
                var emisUserSession = (EmisUserSession)gpUserSession;

                var tasks = new Dictionary<Guid, Task<DemographicsResult>>();

                foreach (var user in emisUserSession.ProxyPatients)
                {
                    // Using SessionId and EndUserSessionId of the logged in user
                    // but the UserPatientLinkToken of the user they are acting
                    // on behalf of.
                    var demographicsTask = _demographicsService.GetDemographics(
                        new GpLinkedAccountModel(
                            new EmisUserSession
                            {
                                SessionId = emisUserSession.SessionId,
                                EndUserSessionId = emisUserSession.EndUserSessionId,
                                UserPatientLinkToken = user.UserPatientLinkToken,
                                Id = user.Id
                            },
                            user.Id));

                    tasks.Add(user.Id, demographicsTask);
                }

                await Task.WhenAll(tasks.Select(x => x.Value));

                var successResults = tasks
                    .Where(x => x.Value.Result is DemographicsResult.Success)
                    .ToList();

                response.LinkedAccounts = successResults.Select(x => {
                    var demographics = (DemographicsResult.Success)x.Value.Result;

                    return new LinkedAccount
                    {
                        Id = x.Key,
                        Name = demographics.Response.PatientName,
                        DateOfBirth = demographics.Response.DateOfBirth,
                        NhsNumber = demographics.Response.NhsNumber,
                    };
                });
            }

            return new LinkedAccountsResult.Success(response);
        }
    }
}
