using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public interface IOrganDonationService
    {
        Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession);
        
        Task<OrganDonationRegistrationResult> Register(OrganDonationRegistrationRequest request, UserSession userSession);
        
        Task<OrganDonationReferenceDataResult> GetReferenceData();

        Task<OrganDonationRegistrationResult> Update(OrganDonationRegistrationRequest request, UserSession userSession);

        Task<OrganDonationWithdrawResult> Withdraw(OrganDonationWithdrawRequest model, UserSession userSession);
    }
}
