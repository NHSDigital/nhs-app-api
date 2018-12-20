using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    public class OrganDonationMockClient : IOrganDonationClient
    {
        public Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
            LookupRegistrationRequest request,
            UserSession userSession)
        {
            return Task.FromResult(
                new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.NotFound));
        }

        public Task<OrganDonationResponse<ReferenceDataResponse>> GetAllReferenceData()
        {
            var referenceData = new ReferenceDataBuilder()
                .AddSection("ethnicities", codes => codes
                    .Add("01", "White - British")
                    .Add("02", "White - Irish")
                    .Add("77", "Not stated")
                    .Add("88", "Not reported"))
                .AddSection("religions", codes => codes
                    .Add("01", "No religion")
                    .Add("02", "Christian - Protestant")
                    .Add("10", "Christian - Catholic")
                    .Add("60", "Other")
                    .Add("88", "Not stated"))
                .Build();

            return Task.FromResult(referenceData);
        }
    }
}

