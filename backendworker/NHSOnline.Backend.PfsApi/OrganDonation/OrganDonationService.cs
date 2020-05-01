using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal class OrganDonationService : IOrganDonationService
    {
        private readonly OrganDonationRegistrationService _registrationService;
        private readonly OrganDonationUpdateService _updateService;
        private readonly OrganDonationLookupService _lookupService;
        private readonly OrganDonationReferenceDataService _referenceDataService;
        private readonly OrganDonationWithdrawService _withdrawService;

        public OrganDonationService(
            OrganDonationRegistrationService registrationService,
            OrganDonationUpdateService updateService,
            OrganDonationLookupService lookupService,
            OrganDonationReferenceDataService referenceDataService,
            OrganDonationWithdrawService withdrawService)
        {
            _registrationService = registrationService;
            _updateService = updateService;
            _lookupService = lookupService;
            _referenceDataService = referenceDataService;
            _withdrawService = withdrawService;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, P9UserSession userSession)
            => await _lookupService.GetOrganDonation(myRecord, userSession);

        public async Task<OrganDonationRegistrationResult> Register(
            OrganDonationRegistrationRequest request,
            P9UserSession userSession)
            => await _registrationService.Register(request, userSession);

        public async Task<OrganDonationRegistrationResult> Update(
            OrganDonationRegistrationRequest request,
            P9UserSession userSession)
            => await _updateService.Update(request, userSession);

        public async Task<OrganDonationReferenceDataResult> GetReferenceData()
            => await _referenceDataService.GetReferenceData();

        public async Task<OrganDonationWithdrawResult> Withdraw(
            OrganDonationWithdrawRequest request,
            P9UserSession userSession) => await _withdrawService.Withdraw(request, userSession);
    }
}