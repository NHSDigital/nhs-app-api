using System.Threading.Tasks;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal interface IOrganDonationClient
    {
        Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
                RegistrationLookupRequest request,
                UserSession userSession);
        
        Task<OrganDonationResponse<RegistrationResponse>> PostRegistration(
                RegistrationRequest request,
                UserSession userSession);

        Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData();

        Task<OrganDonationResponse<RegistrationResponse>> PutUpdate(
            RegistrationRequest request,
            UserSession userSession);
    }
}