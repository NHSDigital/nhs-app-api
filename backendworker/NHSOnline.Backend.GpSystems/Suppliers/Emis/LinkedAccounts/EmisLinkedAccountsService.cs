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
            var emisUserSession = (EmisUserSession) gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            if (IsValidLinkedAccount(gpLinkedAccountModel)
                || gpLinkedAccountModel.GpUserSession.Id == gpLinkedAccountModel.PatientId)
            {
                return await Task.FromResult(new SwitchAccountResult.Success());
            }
            return await Task.FromResult(new SwitchAccountResult.NotFound(gpLinkedAccountModel.PatientId));
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
                    var response = new GetAccountAccessSummaryResponse
                    {
                        IsValidData = true,
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

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var linkedAccountAuditResult = new LinkedAccountAuditInfo();

            if (IsValidLinkedAccount(gpLinkedAccountModel))
            {
                var emisUserSession = (EmisUserSession)gpLinkedAccountModel.GpUserSession;

                linkedAccountAuditResult.IsProxyMode = true;
                linkedAccountAuditResult.ProxyNhsNumber = emisUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.NhsNumber;
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            bool hasAnyNhsNumberBeenUpdatedInSession = false;

            var linkedAccounts = Enumerable.Empty<LinkedAccount>();

            if (gpUserSession.HasLinkedAccounts)
            {
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
                    .ToDictionary(x => x.Key, x => x.Value);

                if (emisUserSession.ProxyPatients.Count != successResults.Count)
                {
                    _logger.LogWarning("Not all demographics calls for proxy patients were successful.");
                }

                linkedAccounts = successResults.Select(x =>
                {
                    var demographics = (DemographicsResult.Success)x.Value.Result;

                    var dateOfBirth = demographics.Response.DateOfBirth;
                    return new LinkedAccount
                    {
                        Id = x.Key,
                        GivenName = demographics.Response.NameParts.Given,
                        FullName = demographics.Response.PatientName,
                        AgeMonths = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeMonths,
                        AgeYears = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeYears,
                        DisplayPersonalizedContent = true
                    };
                });

                foreach (var proxy in emisUserSession.ProxyPatients)
                {
                    if (!successResults.ContainsKey(proxy.Id))
                    {
                        continue;
                    }

                    var nhsNumberFromDemographics = ((DemographicsResult.Success)successResults[proxy.Id].Result).Response.NhsNumber;

                    if (string.IsNullOrEmpty(nhsNumberFromDemographics))
                    {
                        _logger.LogWarning("Linked Account demographic response contains no nhs number");
                    }
                    else if (!string.Equals(nhsNumberFromDemographics, proxy.NhsNumber, StringComparison.OrdinalIgnoreCase))
                    {
                        hasAnyNhsNumberBeenUpdatedInSession = true;
                        proxy.NhsNumber = nhsNumberFromDemographics;
                    }
                }
            }

            var linkedAccountsGrouped = GroupLinkedAccounts(emisUserSession, linkedAccounts);

            return new LinkedAccountsResult.Success(linkedAccountsGrouped.ValidAccounts, hasAnyNhsNumberBeenUpdatedInSession);
        }
        private bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var emisUserSession = (EmisUserSession)gpLinkedAccountModel.GpUserSession;
            return  emisUserSession.ProxyPatients
                .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId) != null;
        }

        private static EmisProxyUserSession GetLinkedAccountFromGpUserSession(GpUserSession gpUserSession, Guid linkedAccountId)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.Id == linkedAccountId);
            return proxy;
        }

        private LinkedAccountsBreakdownSummary GroupLinkedAccounts(EmisUserSession emisUserSession, IEnumerable<LinkedAccount> linkedAccounts)
        {
            var withNhsNumbers = new List<LinkedAccount>();
            var withoutNhsNumbers = new List<LinkedAccount>();
            var validAccounts = new List<LinkedAccount>();
            var mismatchingOdsCodes = new List<LinkedAccount>();

            foreach (var account in linkedAccounts)
            {
                var accountInSession = emisUserSession.ProxyPatients.FirstOrDefault(pp => pp.Id == account.Id);

                if (!string.IsNullOrEmpty(accountInSession?.NhsNumber))
                {
                    withNhsNumbers.Add(account);
                }
                else
                {
                    withoutNhsNumbers.Add(account);
                }
            }

            foreach (var item in withNhsNumbers)
            {
                var proxyUserInSession = GetLinkedAccountFromGpUserSession(emisUserSession, item.Id);

                if (proxyUserInSession != null)
                {
                    if (string.Equals(emisUserSession.OdsCode, proxyUserInSession.OdsCode, StringComparison.OrdinalIgnoreCase))
                    {
                        validAccounts.Add(item);
                    }
                    else
                    {
                        mismatchingOdsCodes.Add(item);
                    }
                }
                else
                {
                    _logger.LogInformation($"Proxy id {item.Id} not found in user session");
                }
            }

            var validAndMismatchingAccounts = new List<LinkedAccount>();
            validAndMismatchingAccounts.AddRange(validAccounts);
            validAndMismatchingAccounts.AddRange(mismatchingOdsCodes);

            _logger.LogInformation($"Linked_profiles_count={linkedAccounts.Count()}, " +
                                   $"excluded_for_not_having_NHS_number={withoutNhsNumbers.Count}, " +
                                   $"has_different_ODS_code={mismatchingOdsCodes.Count}, " +
                                   $"valid_and_being_returned: {validAndMismatchingAccounts.Count}");

            return new LinkedAccountsBreakdownSummary
            {
                ValidAccounts = validAndMismatchingAccounts,
                AccountsWithNoNhsNumber = withoutNhsNumbers,
            };
        }
    }
}
