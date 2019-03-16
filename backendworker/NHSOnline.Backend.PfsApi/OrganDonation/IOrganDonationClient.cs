using System.Threading.Tasks;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal interface IOrganDonationClient
    {
        Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
                RegistrationLookupRequest request,
                UserSession userSession);
        
        Task<OrganDonationResponse<OrganDonationBasicResponse>> PostRegistration(
                RegistrationRequest request,
                UserSession userSession);

        Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData();

        Task<OrganDonationResponse<OrganDonationBasicResponse>> PutUpdate(
            RegistrationRequest request,
            UserSession userSession);

        Task<OrganDonationResponse<OrganDonationBasicResponse>> Delete(
            WithdrawRequest request,
            UserSession userSession);
    }
}