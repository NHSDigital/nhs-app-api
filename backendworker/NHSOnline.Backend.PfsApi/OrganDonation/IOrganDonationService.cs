using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationService
    {
        Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, P9UserSession userSession);
        
        Task<OrganDonationRegistrationResult> Register(OrganDonationRegistrationRequest request, P9UserSession userSession);
        
        Task<OrganDonationReferenceDataResult> GetReferenceData();

        Task<OrganDonationRegistrationResult> Update(OrganDonationRegistrationRequest request, P9UserSession userSession);

        Task<OrganDonationWithdrawResult> Withdraw(OrganDonationWithdrawRequest model, P9UserSession userSession);
    }
}
