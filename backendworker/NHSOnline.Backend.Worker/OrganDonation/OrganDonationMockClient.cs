using System;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationMockClient : IOrganDonationClient
    {
        public Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
            LookupRegistrationRequest request,
            UserSession userSession)
        {
            OrganDonationResponse<RegistrationLookupResponse> response;

            switch (request.NhsNumber)
            {
                // William Raymond
                case ("9458248744"):
                    var builder1 = new RegistrationLookupResponseBuilder();
                    response = builder1
                        .AddIdentifier(request.NhsNumber)
                        .AddOptInSomeOrgansDecision()
                        .AddFaithDeclaration(FaithDeclaration.Yes)
                        .Build();
                    break;
                //Paul Smith
                case ("8434446473"):
                    var builder2 = new RegistrationLookupResponseBuilder();
                    response = builder2
                        .AddIdentifier(request.NhsNumber)
                        .AddOptOutDecision()
                        .AddFaithDeclaration(FaithDeclaration.Yes)
                        .Build();
                    break;
                //Mary Davies
                case ("9987574309"):
                    var builder3 = new RegistrationLookupResponseBuilder();
                    response = builder3
                        .AddIdentifier(request.NhsNumber)
                        .AddOptInAllOrgansDecision()
                        .AddFaithDeclaration(FaithDeclaration.No)
                        .Build();
                    break;
                default:
                    response = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.NotFound);
                    break;
            }

            return Task.FromResult(
                response);
        }

        public Task<OrganDonationResponse<RegistrationResponse>> PostRegistration(
            RegistrationRequest request,
            UserSession userSession)
        {
            return Task.FromResult(new OrganDonationResponse<RegistrationResponse>(HttpStatusCode.OK)
            {
                Body = new RegistrationResponse
                {
                    Id = Guid.NewGuid().ToString()
                }
            });
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
                .AddSection("titles", codes => codes
                    .Add("MR", "Mr")
                    .Add("MRS", "Mrs")
                    .Add("MISS", "Ms")
                    .Add("LADY", "Lady")
                    .Add("SIR", "Sir"))
                .AddSection("genders", codes => codes
                    .Add("01", "Male")
                    .Add("02", "Female")
                    .Add("10", "Other"))
                .AddSection("withdraw-reasons", codes => codes
                    .Add("01", "Don't like it")
                    .Add("10", "Religious")
                    .Add("02", "Other"))
                .Build();

            return Task.FromResult(referenceData);
        }
    }
}

