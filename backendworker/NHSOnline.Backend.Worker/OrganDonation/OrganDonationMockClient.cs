using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.OrganDonation.ApiModels;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.OrganDonation
{
    internal class OrganDonationMockClient : IOrganDonationClient
    {
        private const string AaronDonorNhsNumber = "7883642723";
        private const string WilliamRaymondNhsNumber = "9458248744";
        private const string PaulSmithNhsNumber = "8434446473";
        private const string MaryDaviesNhsNumber = "9987574309";
        private const string BobDonorNhsNumber = "0538236728";
        private const string CatherineDonorNhsNumber = "6151552431";
        private const string KevinBarryNhsNumber = "5785445875";

        public Task<OrganDonationResponse<RegistrationLookupResponse>> PostLookup(
            RegistrationLookupRequest request,
            UserSession userSession)
        {
            OrganDonationResponse<RegistrationLookupResponse> response;
            
            switch (request.NhsNumber)
            {
                // William Raymond
                case (WilliamRaymondNhsNumber):
                    var builder1 = new RegistrationLookupResponseBuilder();
                    response = builder1
                        .AddIdentifier(request.NhsNumber)
                        .AddOptInSomeOrgansDecision()
                        .AddFaithDeclaration(FaithDeclaration.Yes)
                        .Build();
                    break;
                //Paul Smith
                case (PaulSmithNhsNumber):
                    var builder2 = new RegistrationLookupResponseBuilder();
                    response = builder2
                        .AddIdentifier(request.NhsNumber)
                        .AddOptOutDecision()
                        .AddFaithDeclaration(FaithDeclaration.Yes)
                        .Build();
                    break;
                //Mary Davies
                case (MaryDaviesNhsNumber):
                    var builder3 = new RegistrationLookupResponseBuilder();
                    response = builder3
                        .AddIdentifier(request.NhsNumber)
                        .AddOptInAllOrgansDecision()
                        .AddFaithDeclaration(FaithDeclaration.No)
                        .Build();
                    break;
                // Kevin Barry
                case(KevinBarryNhsNumber):
                    var builder4 = new RegistrationLookupResponseBuilder();
                    response = builder4
                        .AddIdentifier(request.NhsNumber)
                        .AddAppRepDecision()
                        .AddFaithDeclaration(FaithDeclaration.No)
                        .Build();
                    break;
                //Aaron Donor
                case (AaronDonorNhsNumber):
                    response = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.Conflict);
                    break;
                default:
                    response = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.NotFound);
                    break;
            }

            return Task.FromResult(response);
        }

        public Task<OrganDonationResponse<RegistrationResponse>> PostRegistration(
            RegistrationRequest request,
            UserSession userSession)
        {
            return Task.FromResult(new OrganDonationResponse<RegistrationResponse>(HttpStatusCode.OK)
            {
                Body = new RegistrationResponse
                {
                    Id = Guid.NewGuid().ToString(),
                    Issue = GetIssue(request)
                }
            });
        }

        private Issue GetIssue(RegistrationRequest request)
        {
            if (string.Equals(request.Identifier.FirstOrDefault()?.Value.RemoveWhiteSpace(), BobDonorNhsNumber, StringComparison.OrdinalIgnoreCase))
            {
                return new Issue
                {
                    Details = new CodeableConcept
                    {
                        Coding = new List<Coding> { new Coding { Code = "10001" } }
                    }
                };
            }
            if (string.Equals(request.Identifier.FirstOrDefault()?.Value.RemoveWhiteSpace(), CatherineDonorNhsNumber, StringComparison.OrdinalIgnoreCase))
            {
                return new Issue
                {
                    Details = new CodeableConcept
                    {
                        Coding = new List<Coding> { new Coding { Code = "10002" } }
                    }
                };
            }
            return null;
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

