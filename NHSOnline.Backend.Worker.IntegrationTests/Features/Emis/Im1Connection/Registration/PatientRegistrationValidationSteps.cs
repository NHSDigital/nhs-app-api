using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Registration
{
    [Binding]
    class PatientRegistrationValidationSteps
    {
        private readonly Dictionary<string, string> _headers;
        private readonly ScenarioContext _context;

        private const string XApiApplicationId = "D66BA979-60D2-49AA-BE82-AEC06356E41F";
        private const string XApiVersion = "2.1.0.0";
        private const string XApiEndUserSessionId = "SE333ION989ID";

        public PatientRegistrationValidationSteps(ScenarioContext context)
        {
            _context = context;

            _headers = new Dictionary<string, string>
            {
                { "X-API-ApplicationId", XApiApplicationId },
                { "X-API-Version", XApiVersion },
                { "X-API-EndUserSessionId", XApiEndUserSessionId }
            };
        }

        [Given(@"I have an EMIS user's IM1 credentials with an ODS Code not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithAnOdsCodeNotInTheExpectedFormat()
        {
            await MockSuccessfulIm1Connection();

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "xxx-wrong-format-xxx",
                    Surname = "Yoda",
                    DateOfBirth = "1919-12-24T14:03:15.892"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Yoda",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    NationalPracticeCode = "xxx-wrong-format-xxx"
                }
            };

            await CreateApplicationsMapping(400, emisRequestBody);
        }

        [Given(@"I have an EMIS user's IM1 credentials with a Surname not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithASurnameNotInTheExpectedFormat()
        {
            await MockSuccessfulIm1Connection();

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "A82010",
                    Surname = "xxx-wrong-format-xxx",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "xxx-wrong-format-xxx",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    NationalPracticeCode = "A82010"
                }
            };

            await CreateApplicationsMapping(400, emisRequestBody);
        }

        [Given(@"I have an EMIS user's IM1 credentials with an Account ID not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithAnAccountIdNotInTheExpectedFormat()
        {
            await MockSuccessfulIm1Connection();

            _context.SetIm1ConnectionRequest(
                new Im1ConnectionRequest
                {
                    AccountId = "xxx-wrong-format-xxx",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "A82010",
                    Surname = "Yoda",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Yoda",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "xxx-wrong-format-xxx",
                    LinkageKey = "MASTER000YODA",
                    NationalPracticeCode = "A82010"
                }
            };

            await CreateApplicationsMapping(400, emisRequestBody);
        }

        [Given(@"I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithADateOfBirthNotInTheExpectedFormat()
        {
            await MockSuccessfulIm1Connection();

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "A82010",
                    Surname = "Yoda",
                    DateOfBirth = "xxx-wrong-format-xxx"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Yoda",
                DateOfBirth = "xxx-wrong-format-xxx",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    NationalPracticeCode = "A82010"
                }
            };

            await CreateApplicationsMapping(400, emisRequestBody);
        }

        [Given(@"I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithALinkageKeyNotInTheExpectedFormat()
        {
            await MockSuccessfulIm1Connection();

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "xxx-wrong-format-xxx",
                    OdsCode = "A82010",
                    Surname = "Yoda",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Yoda",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "xxx-wrong-format-xxx",
                    NationalPracticeCode = "A82010"
                }
            };

            await CreateApplicationsMapping(400, emisRequestBody);
        }

        [Given(@"I have an EMIS user's IM1 credentials with missing ODS Code")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithMissingOdsCode()
        {
            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    Surname = "Yoda",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );
        }

        [Given(@"I have an EMIS user's IM1 credentials with missing Surname")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithMissingSurname()
        {
            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "A82010",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );
        }

        [Given(@"I have an EMIS user's IM1 credentials with missing Account ID")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithMissingAccountId()
        {
            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    LinkageKey = "MASTER000YODA",
                    Surname = "Yoda",
                    OdsCode = "A82010",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );
        }

        [Given(@"I have an EMIS user's IM1 credentials with missing Linkage Key")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithMissingLinkageKey()
        {
            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    Surname = "Yoda",
                    OdsCode = "A82010",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );
        }

        private async Task MockSuccessfulIm1Connection()
        {
            const int statusCodeCreated = (int)HttpStatusCode.Created;
            const int statusCodeOk = (int)HttpStatusCode.OK;

            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMapping(statusCodeCreated, XApiEndUserSessionId, XApiApplicationId, XApiVersion));
            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateSessionsMapping(statusCodeCreated, null, "A82010", "session_id", "link_token", AssociationType.Self, XApiEndUserSessionId, XApiApplicationId, XApiVersion));
            await _context.GetMockingClient().PostMappingAsync(DemographicsConfigurator.CreateDemographicsMapping(statusCodeOk, "link_token", "session_id", XApiEndUserSessionId, new string[0], XApiApplicationId, XApiVersion));
        }

        private async Task CreateApplicationsMapping(int statusCode, LinkApplicationRequest requestBody)
        {
            var responseBody = new LinkApplicationResponse
            {
                AccessIdentityGuid = "IYYTYODA876786tT"
            };

            await _context
                .GetMockingClient()
                .PostMappingAsync(
                    MeConfigurator.CreateApplicationsMapping(
                        statusCode,
                        _headers,
                        requestBody,
                        responseBody
                    )
                );
        }
    }
}
