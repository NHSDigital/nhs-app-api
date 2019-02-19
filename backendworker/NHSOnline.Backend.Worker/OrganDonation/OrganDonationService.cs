using System.Threading.Tasks;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationService : IOrganDonationService
    {
        private readonly OrganDonationRegistrationService _registrationService;
        private readonly OrganDonationUpdateService _updateService;
        private readonly OrganDonationLookupService _lookupService;
        private readonly OrganDonationReferenceDataService _referenceDataService;

        public OrganDonationService(
            OrganDonationRegistrationService registrationService,
            OrganDonationUpdateService updateService,
            OrganDonationLookupService lookupService,
            OrganDonationReferenceDataService referenceDataService)
        {
            _registrationService = registrationService;
            _updateService = updateService;
            _lookupService = lookupService;
            _referenceDataService = referenceDataService;
        }

        public async Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession)
        {
            return await _lookupService.GetOrganDonation(myRecord, userSession);
        }

        public async Task<OrganDonationRegistrationResult> Register(
            OrganDonationRegistrationRequest request,
            UserSession userSession)
        {
            return await _registrationService.Register(request, userSession);
        }

        public async Task<OrganDonationRegistrationResult> Update(
            OrganDonationRegistrationRequest request,
            UserSession userSession)
        {
            return await _updateService.Update(request, userSession);
        }

        public async Task<OrganDonationReferenceDataResult> GetReferenceData()
        {
            return await _referenceDataService.GetReferenceData();
        }
    }
}