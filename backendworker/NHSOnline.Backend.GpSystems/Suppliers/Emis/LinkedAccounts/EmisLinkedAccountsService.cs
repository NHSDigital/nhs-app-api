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
        private readonly CalculateAge _calculateAge;

        public EmisLinkedAccountsService(
            ILogger<EmisLinkedAccountsService> logger,
            IEmisDemographicsService demographicsService,
            IEmisClient emisClient,
            CalculateAge calculateAge)
        {
            _logger = logger;
            _demographicsService = demographicsService;
            _emisClient = emisClient;
            _calculateAge = calculateAge;
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients
                .FirstOrDefault(x => x.PatientActivityContextGuid == patientGpIdentifier);

            return proxy?.NhsNumber;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients
                .FirstOrDefault(x => x.PatientActivityContextGuid == patientGpIdentifier);

            return proxy?.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            if (patientGpIdentifier == emisUserSession.PatientActivityContextGuid)
            {
                return await Task.FromResult(new SwitchAccountResult.Success());
            }

            if (IsValidLinkedAccount(emisUserSession, patientGpIdentifier))
            {
                return await EnsureNhsNumberPopulated(emisUserSession, patientGpIdentifier);
            }

            return await Task.FromResult(new SwitchAccountResult.NotFound(patientGpIdentifier));
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            var proxy = emisUserSession.ProxyPatients.FirstOrDefault(x => x.PatientActivityContextGuid == patientGpIdentifier);

            if (proxy == null)
            {
                _logger.LogError($"Proxy patient not found in {nameof(emisUserSession.ProxyPatients)}");
                return await Task.FromResult(new LinkedAccountAccessSummaryResult.NotFound());
            }

            var tempProxyUserSession = new EmisUserSession
            {
                SessionId = emisUserSession.SessionId,
                EndUserSessionId = emisUserSession.EndUserSessionId,
                UserPatientLinkToken = proxy.UserPatientLinkToken,
            };

            _logger.LogInformation("Getting linked account summary");

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
                    .FirstOrDefault(x => x.PatientActivityContextGuid == gpLinkedAccountModel.RequestingPatientGpIdentifier)?.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession, Dictionary<Guid, string> gpIdentifierMapping)
        {
            var emisUserSession = (EmisUserSession)gpUserSession;
            bool hasAnyNhsNumberBeenUpdatedInSession = false;

            var linkedAccounts = Enumerable.Empty<LinkedAccount>();

            if (emisUserSession.HasLinkedAccounts)
            {
                var tasks = new Dictionary<string, Task<DemographicsResult>>();

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
                                PatientActivityContextGuid = user.PatientActivityContextGuid,
                            },
                            user.PatientActivityContextGuid));

                    tasks.Add(user.PatientActivityContextGuid, demographicsTask);
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
                    var ageInMonthsAndYears = _calculateAge.CalculateAgeInMonthsAndYears(dateOfBirth);
                    return new LinkedAccount
                    {
                        Id = gpIdentifierMapping.First(gim => gim.Value == x.Key).Key,
                        GivenName = demographics.Response.NameParts.Given,
                        FullName = demographics.Response.PatientName,
                        AgeMonths = ageInMonthsAndYears.AgeMonths,
                        AgeYears = ageInMonthsAndYears.AgeYears,
                        DisplayPersonalizedContent = true
                    };
                });

                foreach (var proxy in emisUserSession.ProxyPatients)
                {
                    if (!successResults.ContainsKey(proxy.PatientActivityContextGuid))
                    {
                        continue;
                    }

                    var nhsNumberFromDemographics = ((DemographicsResult.Success)successResults[proxy.PatientActivityContextGuid].Result).Response.NhsNumber;

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

            var linkedAccountsGrouped = GroupLinkedAccounts(emisUserSession, linkedAccounts, gpIdentifierMapping);

            return new LinkedAccountsResult.Success(linkedAccountsGrouped.ValidAccounts, hasAnyNhsNumberBeenUpdatedInSession);
        }

        private static bool IsValidLinkedAccount(EmisUserSession emisUserSession, string patientGpIdentifier)
        {
            return emisUserSession.ProxyPatients
                .Any(x => x.PatientActivityContextGuid == patientGpIdentifier);
        }

        private static bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var emisUserSession = (EmisUserSession) gpLinkedAccountModel.GpUserSession;
            return IsValidLinkedAccount(emisUserSession, gpLinkedAccountModel.RequestingPatientGpIdentifier);
        }

        private async Task<SwitchAccountResult> EnsureNhsNumberPopulated(EmisUserSession emisUserSession, string patientGpIdentifier)
        {
            var patient = emisUserSession.ProxyPatients.First(
                x => x.PatientActivityContextGuid == patientGpIdentifier);

            if (string.IsNullOrEmpty(patient.NhsNumber))
            {
                // Using SessionId and EndUserSessionId of the logged in user
                // but the UserPatientLinkToken of the user they are acting
                // on behalf of.
                var demographics = await _demographicsService.GetDemographics(
                    new GpLinkedAccountModel(
                        new EmisUserSession
                        {
                            SessionId = emisUserSession.SessionId,
                            EndUserSessionId = emisUserSession.EndUserSessionId,
                            UserPatientLinkToken = patient.UserPatientLinkToken,
                            PatientActivityContextGuid = patient.PatientActivityContextGuid,
                        },
                        patient.PatientActivityContextGuid));

                if (demographics is DemographicsResult.Success success)
                {
                    patient.NhsNumber = success.Response.NhsNumber;
                }
                else
                {
                    _logger.LogError("Couldn't get patient's NHS no to switch to");
                    return await Task.FromResult(new SwitchAccountResult.Failure(patientGpIdentifier));
                }
            }

            return await Task.FromResult(new SwitchAccountResult.Success());
        }

        private LinkedAccountsBreakdownSummary GroupLinkedAccounts(EmisUserSession emisUserSession, IEnumerable<LinkedAccount> linkedAccounts, Dictionary<Guid, string> gpIdentifierMapping)
        {
            var withNhsNumbers = new List<LinkedAccount>();
            var withoutNhsNumbers = new List<LinkedAccount>();
            var validAccounts = new List<LinkedAccount>();
            var mismatchingOdsCodes = new List<LinkedAccount>();

            foreach (var account in linkedAccounts)
            {
                var proxyPatientGpIdentifier = gpIdentifierMapping[account.Id];
                var accountInSession = emisUserSession.ProxyPatients.FirstOrDefault(pp => pp.PatientActivityContextGuid == proxyPatientGpIdentifier);

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
                var proxyPatientGpIdentifier = gpIdentifierMapping[item.Id];
                var proxyUserInSession = emisUserSession.ProxyPatients.FirstOrDefault(x => x.PatientActivityContextGuid == proxyPatientGpIdentifier);

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
