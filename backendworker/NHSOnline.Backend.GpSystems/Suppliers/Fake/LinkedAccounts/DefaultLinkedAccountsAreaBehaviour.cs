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

        public DefaultLinkedAccountsAreaBehaviour(
            ILogger<DefaultLinkedAccountsAreaBehaviour> logger,
            IFakeUserRepository fakeUserRepository
        )
        {
            _logger = logger;
            _fakeUserRepository = fakeUserRepository;
        }

        public string GetOdsCodeForLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);
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

        public async Task<LinkedAccountsResult> GetLinkedAccounts(GpUserSession gpUserSession)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;

            var linkedAccounts = new List<LinkedAccount>();

            if (gpUserSession.HasLinkedAccounts)
            {
                foreach (var proxyPatient in fakeUserSession.ProxyPatients)
                {
                    var proxyUser = await _fakeUserRepository.Find(proxyPatient.NhsNumber);

                    var ageData = CalculateAge.CalculateAgeInMonthsAndYears(proxyUser.DateOfBirth);

                    linkedAccounts.Add(new LinkedAccount
                    {
                        Id = proxyUser.UserUuid,
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

        public async Task<LinkedAccountAccessSummaryResult> GetLinkedAccount(GpUserSession gpUserSession, Guid id)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);

            if (proxy == null)
            {
                _logger.LogError($"Proxy patient with id {id} not found in {nameof(fakeUserSession.ProxyPatients)}");
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
                    .FirstOrDefault(x => x.Id == gpLinkedAccountModel.PatientId);
                linkedAccountAuditResult.ProxyNhsNumber = proxyPatient.NhsNumber;
            }

            return linkedAccountAuditResult;
        }

        public string GetNhsNumberForProxyUser(GpUserSession gpUserSession, Guid id)
        {
            var fakeUserSession = (FakeUserSession) gpUserSession;
            var proxy = fakeUserSession.ProxyPatients.FirstOrDefault(x => x.Id == id);
            return proxy?.NhsNumber;
        }

        private static bool IsValidLinkedAccount(GpLinkedAccountModel gpLinkedAccountModel)
        {
            var fakeUserSession = (FakeUserSession) gpLinkedAccountModel.GpUserSession;
            return fakeUserSession.ProxyPatients.Any(x =>
                x.Id == gpLinkedAccountModel.PatientId);
        }
    }
}