using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Fake.Users;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Fake.LinkedAccounts
{
    [FakeGpAreaBehaviour(Behaviour.Default)]
    public class DefaultLinkedAccountsAreaBehaviour : ILinkedAccountsAreaBehaviour
    {
        private readonly ILogger<DefaultLinkedAccountsAreaBehaviour> _logger;
        private readonly IFakeUserRepository _fakeUserRepository;
        private readonly CalculateAge _calculateAge;

        public DefaultLinkedAccountsAreaBehaviour(
            ILogger<DefaultLinkedAccountsAreaBehaviour> logger,
            IFakeUserRepository fakeUserRepository,
            CalculateAge calculateAge)
        {
            _logger = logger;
            _fakeUserRepository = fakeUserRepository;
            _calculateAge = calculateAge;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;

            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.NhsNumber == patientGpIdentifier);
            return proxy?.OdsCode;
        }

        public async Task<SwitchAccountResult> SwitchAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var fakeUserSession = (FakeUserSession)gpUserSession;

            if (IsValidLinkedAccount(gpUserSession, patientGpIdentifier)
                || fakeUserSession.NhsNumber == patientGpIdentifier)
            {
                return await Task.FromResult(new SwitchAccountResult.Success());
            }

            return await Task.FromResult(new SwitchAccountResult.NotFound(patientGpIdentifier));
        }

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession, Dictionary<Guid, string> gpIdentifierMapping)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;

            var linkedAccounts = new List<LinkedAccount>();

            if (fakeUserSession.HasLinkedAccounts)
            {
                foreach (var proxyPatient in fakeUserSession.ProxyPatients)
                {
                    var proxyUser = await _fakeUserRepository.Find(proxyPatient.NhsNumber);

                    var ageData = _calculateAge.CalculateAgeInMonthsAndYears(proxyUser.DateOfBirth);

                    var sessionKey = gpIdentifierMapping.First(x => x.Value == proxyPatient.NhsNumber).Key;
                    linkedAccounts.Add(new LinkedAccount
                    {
                        Id = sessionKey,
                        AgeMonths = ageData.AgeMonths,
                        AgeYears = ageData.AgeYears,
                        FullName = proxyUser.Name,
                        GivenName = proxyUser.GivenName,
                        DisplayPersonalizedContent = true
                    });
                }
            }

            return await Task.FromResult(new LinkedAccountsResult.Success(linkedAccounts, false));
        }

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.NhsNumber == patientGpIdentifier);

            if (proxy == null)
            {
                _logger.LogError($"Proxy patient not found in {nameof(fakeUserSession.ProxyPatients)}");
                return await Task.FromResult(new LinkedAccountAccessSummaryResult.NotFound());
            }

            var response = new GetAccountAccessSummaryResponse
            {
                CanBookAppointment = true,
                CanOrderRepeatPrescription = true,
                CanViewMedicalRecord = true,
                IsValidData = true
            };

            return await Task.FromResult(new LinkedAccountAccessSummaryResult.Success(response));
        }

        public LinkedAccountAuditInfo GetProxyAuditData(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var linkedAccountAuditResult = new LinkedAccountAuditInfo();

            if (IsValidLinkedAccount(gpLinkedAccountModel))
            {
                var fakeUserSession = (FakeUserSession) gpLinkedAccountModel.GpUserSession;

                linkedAccountAuditResult.IsProxyMode = true;
                var proxyPatient = fakeUserSession.ProxyPatients
                    .FirstOrDefault(x => x.NhsNumber == gpLinkedAccountModel.RequestingPatientGpIdentifier);
                linkedAccountAuditResult.ProxyNhsNumber = proxyPatient.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.NhsNumber == patientGpIdentifier);
            return proxy?.NhsNumber;
        }

        private static bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            return IsValidLinkedAccount(
                gpLinkedAccountModel.GpUserSession,
                gpLinkedAccountModel.RequestingPatientGpIdentifier);
        }

        private static bool IsValidLinkedAccount(GpUserSession gpUserSession, string patientGpIdentifier)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            return fakeUserSession.ProxyPatients.Any(x =>
                x.NhsNumber == patientGpIdentifier);
        }
    }
}