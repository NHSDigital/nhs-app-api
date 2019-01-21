using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public interface IOrganDonationService
    {
        Task<OrganDonationResult> GetOrganDonation(DemographicsResult myRecord, UserSession userSession);
        
        Task<OrganDonationRegistrationResult> Register(OrganDonationRegistrationRequest request, UserSession userSession);
        
        Task<OrganDonationReferenceDataResult> GetReferenceData();
    }
}
