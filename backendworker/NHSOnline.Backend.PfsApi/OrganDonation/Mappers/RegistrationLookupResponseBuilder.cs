using System.Collections.Generic;
using System.Net;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.PfsApi.OrganDonation.ApiModels;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using static NHSOnline.Backend.Support.Constants.OrganDonationConstants;


namespace NHSOnline.Backend.PfsApi.OrganDonation.Mappers
{
    internal class RegistrationLookupResponseBuilder
    {
        private Registration _registration = new Registration();
        private Dictionary<string,string> _allOrganWishes = new Dictionary<string, string>
        {
            { "all", YesChoiceValue },
            { "kidney", NotStatedChoiceValue },
            { "corneas", NotStatedChoiceValue },
            { "heart", NotStatedChoiceValue },
            { "lungs", NotStatedChoiceValue },
            { "liver", NotStatedChoiceValue },
            { "pancreas", NotStatedChoiceValue },
            { "tissue", NotStatedChoiceValue },
            { "smallBowel", NotStatedChoiceValue }
        };
        private Dictionary<string,string> _someOrganWishes = new Dictionary<string, string>
        {
            { "all", NoChoiceValue },
            { "kidney", NoChoiceValue },
            { "corneas", YesChoiceValue },
            { "heart", NoChoiceValue },
            { "lungs", YesChoiceValue },
            { "liver", NoChoiceValue },
            { "pancreas", YesChoiceValue },
            { "tissue", NoChoiceValue },
            { "smallBowel", YesChoiceValue }
        };
        
        public OrganDonationResponse<RegistrationLookupResponse> Build()
        {
            var response = new OrganDonationResponse<RegistrationLookupResponse>(HttpStatusCode.OK)
            {
                Body = new RegistrationLookupResponse()
                {
                    Entry = new List<Entry<Registration>>
                    {
                        new Entry<Registration>()
                        {
                            Resource = _registration
                        }
                    }
                }
            };

            return response;
        }
        
        public RegistrationLookupResponseBuilder AddOptInAllOrgansDecision()
        {
            _registration.OrganDonationDecision = "opt-in";
            _registration.DonationWishes = _allOrganWishes;
            return this;
        }

        public RegistrationLookupResponseBuilder AddIdentifier(string nhsNumber)
        {
            _registration.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = "https://fhir.nhs.uk/Id/nhs-number",
                    Value = nhsNumber
                }
            };
            return this;
        }
        
        public RegistrationLookupResponseBuilder AddAppRepDecision()
        {
            _registration.OrganDonationDecision = "app-rep";
            return this;
        }

        public RegistrationLookupResponseBuilder AddOptInSomeOrgansDecision()
        {
            _registration.OrganDonationDecision = "opt-in";
            _registration.DonationWishes = _someOrganWishes;
            return this;
        }

        public RegistrationLookupResponseBuilder AddOptOutDecision()
        {
            _registration.OrganDonationDecision = "opt-out";
            return this;
        }

        public RegistrationLookupResponseBuilder AddFaithDeclaration(FaithDeclaration faithDeclaration)
        {
            switch (faithDeclaration)
            {
                case FaithDeclaration.NotStated:
                    _registration.FaithDeclaration = NotStatedChoiceValue;
                    break;
                case FaithDeclaration.Yes:
                    _registration.FaithDeclaration = YesChoiceValue;
                    break;
                case FaithDeclaration.No:
                    _registration.FaithDeclaration = NoChoiceValue;
                    break;
                default:
                    _registration.FaithDeclaration = NotStatedChoiceValue;
                    break;
            }

            return this;
        }
    }
}