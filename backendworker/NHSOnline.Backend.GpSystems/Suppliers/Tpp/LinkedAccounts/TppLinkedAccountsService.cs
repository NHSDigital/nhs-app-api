using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.LinkedAccounts
{
    public class TppLinkedAccountsService : ILinkedAccountsService, ITppLinkedAccountsService
    {
        private readonly ILogger<TppLinkedAccountsService> _logger;
        private readonly IGpSessionManager _gpSessionManager;
        private readonly IFireAndForgetService _fireAndForgetService;

        public TppLinkedAccountsService(
            ILogger<TppLinkedAccountsService> logger,
            IGpSessionManager gpSessionManager,
            IFireAndForgetService fireAndForgetService)
        {
            _logger = logger;
            _gpSessionManager = gpSessionManager;
            _fireAndForgetService = fireAndForgetService;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            return tppUserSession.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;

            if (tppUserSession.GetCurrentlyAuthenticatedId() == gpLinkedAccountModel.PatientId)
            {
                _logger.LogInformation("TPP user already authenticated");
                return new SwitchAccountResult.AlreadyAuthenticated(gpLinkedAccountModel.PatientId);
            }

            var proxy = GetProxyPatient(gpLinkedAccountModel);
            var validId = proxy != null || tppUserSession.Id == gpLinkedAccountModel.PatientId;

            if (!validId)
            {
                _logger.LogInformation("Unknown patient guid - could not find match on TppUserSession");
                return new SwitchAccountResult.NotFound(gpLinkedAccountModel.PatientId);
            }

            // The TppUserSession instance gets amended as part of re-creating a session, but
            // we still want a snapshot of the pre-amended version to close the old session with.
            var tppUserSessionBeforeRecreated = CloneTppUserSession(tppUserSession);

            var patientIdToSwitchTo = proxy != null ? proxy.PatientId : tppUserSession.PatientId;

            var result = await _gpSessionManager.RecreateSession(patientIdToSwitchTo);

            if (result is RecreateSessionResult.Failure)
            {
                _logger.LogInformation("Recreate TPP User Session failed");
                return new SwitchAccountResult.Failure(gpLinkedAccountModel.PatientId);
            }

            _fireAndForgetService.Run(
                (svcProvider) => CloseTppSession(svcProvider, tppUserSessionBeforeRecreated),
                "Failed to close TPP User Session");

            _logger.LogInformation("Successfully Recreated TPP User Session - account switched");

            return new SwitchAccountResult.Success();
        }

        private TppUserSession CloneTppUserSession(TppUserSession tppUserSession)
        {
            return JsonConvert.DeserializeObject<TppUserSession>(JsonConvert.SerializeObject(tppUserSession));
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            var tppUserSession = (TppUserSession)gpUserSession;

            var linkedAccounts = new List<LinkedAccount>();

            if (tppUserSession.HasLinkedAccounts)
            {
                foreach (var proxyPatient in tppUserSession.ProxyPatients)
                {
                    var dateOfBirth = proxyPatient.DateOfBirth;
                    linkedAccounts.Add(new LinkedAccount
                    {
                        Id = proxyPatient.Id,
                        GivenName = proxyPatient.FullName,
                        FullName = proxyPatient.FullName,
                        AgeMonths = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeMonths,
                        AgeYears = CalculateAge.CalculateAgeInMonthsAndYears(dateOfBirth).AgeYears,
                        DisplayPersonalizedContent = false
                    });
                }
            }

            return await Task.FromResult(new LinkedAccountsResult.Success(linkedAccounts, false));
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var response = new GetAccountAccessSummaryResponse
            {
                IsValidData = false,
            };
            return await Task.FromResult(new LinkedAccountAccessSummaryResult.Success(response));
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var linkedAccountAuditResult = new LinkedAccountAuditInfo();

            if (IsValidLinkedAccount(gpLinkedAccountModel))
            {
                var tppUserSession = (TppUserSession)gpLinkedAccountModel.GpUserSession;

                linkedAccountAuditResult.IsProxyMode = true;
                linkedAccountAuditResult.ProxyNhsNumber = tppUserSession.ProxyPatients
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId)?.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        public List<Person> ExtractValidProxyPatients(AuthenticateReply response)
        {
            var mainPatientId = response.User?.Person?.PatientId;

            if (string.IsNullOrWhiteSpace(mainPatientId))
            {
                _logger.LogWarning($"TPP user with no {nameof(AuthenticateReply.User.Person.PatientId)}");
                return new List<Person>();
            }

            var patientAccessItems = response.Registration?.PatientAccess ?? new List<PatientAccess>();

            var selfPatient = patientAccessItems.FirstOrDefault(
                x => mainPatientId.Equals(x.PatientId, StringComparison.Ordinal));

            if (selfPatient == null)
            {
                _logger.LogWarning(
                    $"TPP user details not found in {nameof(AuthenticateReply.Registration.PatientAccess)}");
                return new List<Person>();
            }

            var userPractice = new
            {
                selfPatient.SiteDetails?.UnitName,
                selfPatient.SiteDetails?.Address?.Address,
            };

            if (userPractice.UnitName == null || userPractice.Address == null)
            {
                _logger.LogWarning(
                    $"TPP user practice details not specified. unitName:{userPractice.UnitName} address:{userPractice.Address}");
                return new List<Person>();
            }

            var linkedPatientAccessItems = patientAccessItems
                .Where(x => !mainPatientId.Equals(x.PatientId, StringComparison.Ordinal))
                .ToList();

            var allLinkedPatients = response.ExtractLinkedPatients();
            var withoutNhsNumbers = new List<Person>();
            var mismatchingOdsCodes = new List<Person>();
            var validPatients = new List<Person>();

            foreach (var patient in allLinkedPatients)
            {
                if (string.IsNullOrEmpty(patient?.NationalId?.Value))
                {
                    withoutNhsNumbers.Add(patient);
                }
                else
                {
                    var siteDetailsForProxyPatient =
                        linkedPatientAccessItems
                            .FirstOrDefault(x => x.PatientId == patient.PatientId)?.SiteDetails;

                    var hasSamePracticeDetailsAsMainUser =
                        HasSamePracticeAsMainUser(selfPatient.SiteDetails, siteDetailsForProxyPatient);

                    if (!hasSamePracticeDetailsAsMainUser)
                    {
                        mismatchingOdsCodes.Add(patient);
                    }
                    else
                    {
                        validPatients.Add(patient);
                    }
                }
            }

            var differentPracticeAddressCount = linkedPatientAccessItems.Count(x =>
                !HasSamePracticeAsMainUser(selfPatient.SiteDetails, x.SiteDetails));

            _logger.LogInformation(
                $"User has linked_accounts={allLinkedPatients.Count()}, with different_ods_codes_to_user={differentPracticeAddressCount}");

            var validPatientsAndMismatchingOdsCodes = new List<Person>();
            validPatientsAndMismatchingOdsCodes.AddRange(validPatients);
            validPatientsAndMismatchingOdsCodes.AddRange(mismatchingOdsCodes);

            _logger.LogInformation($"Linked_profiles_count={allLinkedPatients.Count()}, " +
                                   $"excluded_for_not_having_NHS_number={withoutNhsNumbers.Count}, " +
                                   $"has_different_ODS_code={mismatchingOdsCodes.Count}, " +
                                   $"valid_and_being_returned: {validPatientsAndMismatchingOdsCodes.Count}");

            return validPatientsAndMismatchingOdsCodes;
        }

        public void LogMismatchingPractices(AuthenticateReply authenticateReply, ICollection<TppProxyUserSession> proxyPatients)
        {
            var mainUserSiteDetails = authenticateReply.GetSiteDetails(authenticateReply.User.Person.PatientId);

            foreach (var proxyPatient in proxyPatients)
            {
                var proxyUserSiteDetails = authenticateReply.GetSiteDetails(proxyPatient.PatientId);

                var unitNameMatch = string.Equals(mainUserSiteDetails.UnitName,
                    proxyUserSiteDetails.UnitName, StringComparison.Ordinal);

                var addressMatch = string.Equals(mainUserSiteDetails.Address?.Address,
                    proxyUserSiteDetails.Address?.Address, StringComparison.Ordinal);

                if (!unitNameMatch && !addressMatch)
                {
                    _logger.LogInformation(
                        $"Proxy Patient with Guid {proxyPatient.Id} has different unit name and address " +
                        $"{proxyUserSiteDetails.UnitName}, {proxyUserSiteDetails.Address?.Address} from main user" +
                        $" {mainUserSiteDetails.UnitName}, {mainUserSiteDetails.Address?.Address}");
                }
                else if (!unitNameMatch)
                {
                    _logger.LogInformation(
                        $"Proxy Patient with Guid {proxyPatient.Id} has different unit name " +
                        $"{proxyUserSiteDetails.UnitName} from main user {mainUserSiteDetails.UnitName}");
                }
                else if (!addressMatch)
                {
                    _logger.LogInformation(
                        $"Proxy Patient with Guid {proxyPatient.Id} has different address " +
                        $"{proxyUserSiteDetails.Address?.Address} from main user {mainUserSiteDetails.Address?.Address}");
                }
            }
        }

        private static bool HasSamePracticeAsMainUser(SiteDetails mainUserSiteDetails, SiteDetails proxyUserSiteDetails)
        {
            var unitNameMatch = string.Equals(mainUserSiteDetails.UnitName, proxyUserSiteDetails.UnitName, StringComparison.Ordinal);
            var addressMatch = string.Equals(mainUserSiteDetails.Address?.Address, proxyUserSiteDetails.Address?.Address, StringComparison.Ordinal);
            return unitNameMatch && addressMatch;
        }

        private bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return GetProxyPatient(gpLinkedAccountModel) != null;
        }

        private TppProxyUserSession GetProxyPatient(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var tppUserSession = (TppUserSession) gpLinkedAccountModel.GpUserSession;
            return tppUserSession.ProxyPatients
                .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId);
        }

        private async Task CloseTppSession(IServiceProvider serviceProvider, TppUserSession tppUserSession)
        {
            var scopedGpSessionManager = serviceProvider.GetService<IGpSessionManager>();
            var closeSessionResult = await scopedGpSessionManager.CloseSession(tppUserSession);

            if (closeSessionResult is CloseSessionResult.Success)
            {
                _logger.LogInformation(
                    $"Successfully closed TPP User Session for id {tppUserSession.GetCurrentlyAuthenticatedId()}");
            }
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            var tppUserSession = (TppUserSession)gpUserSession;
            var proxy = tppUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            return proxy?.NhsNumber;
        }
    }
}