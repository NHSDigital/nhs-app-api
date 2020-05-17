using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    internal interface IOrganDonationClient
    {
        Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
                RegistrationLookupRequest request,
                P9UserSession userSession);

        Task<OrganDonationResponse<OrganDonationBasicResponse>> PostRegistration(
                RegistrationRequest request,
                P9UserSession userSession);

        Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData();

        Task<OrganDonationResponse<OrganDonationBasicResponse>> PutUpdate(
            RegistrationRequest request,
            P9UserSession userSession);

        Task<OrganDonationResponse<OrganDonationBasicResponse>> Delete(
            WithdrawRequest request,
            P9UserSession userSession);
    }
}