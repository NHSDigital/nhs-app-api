using System.Threading.Tasks;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal interface IOrganDonationClient
    {
        Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
                LookupRegistrationRequest request,
                UserSession userSession);
        
        Task<OrganDonationResponse<RegistrationResponse>> PostRegistration(
                RegistrationRequest request,
                UserSession userSession);

        Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData();
    }
}