using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationMockClient : IOrganDonationClient
    {
        public Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(LookupRegistrationRequest request, UserSession userSession)
        {
            return Task.FromResult(
                new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.NotFound));
        }
    }
}