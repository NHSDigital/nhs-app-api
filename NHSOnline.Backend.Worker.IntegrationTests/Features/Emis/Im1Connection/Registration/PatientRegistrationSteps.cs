using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Nhso.Models.Patient;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Registration
{
    [Binding]
    class PatientRegistrationSteps
    {
        private readonly Dictionary<string, string> _headers;
        private readonly ScenarioContext _context;

        public PatientRegistrationSteps(ScenarioContext context)
        {
            _context = context;

            _headers = new Dictionary<string, string>
            {
                { "X-API-ApplicationId", Configuration.EmisApplicationId },
                { "X-API-Version", Configuration.EmisVersion },
                { "X-API-EndUserSessionId", "bar" }
            };
        }

        [Given(@"I have valid patient data to register new account")]
        public async Task GivenIHaveValidPatientDataToRegisterNewAccount()
        {
            string[] nhsNumbers = { "nhsNumber" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(nhsNumbers, connectionToken, odsCode);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "Smith",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Smith",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = EmisDefaults.OdsCode
                }
            };

            await CreateApplicationsMapping(200, emisRequestBody);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid patient data with multiple nhs numbers to register new account")]
        public async Task GivenIhaveValidPatientDataWithMultipleNhsNumbersToRegisterNewAccount()
        {
            string[] nhsNumbers = { "nhsNumber1", "nhsNumber2" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(nhsNumbers, connectionToken, odsCode);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "Smith",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Smith",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = "A29928"
                }
            };

            await CreateApplicationsMapping(200, emisRequestBody);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid data for a patient with no NHS Number")]
        public async Task GivenIHaveValidDataForAPatientWithNoNhsNumber()
        {
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(null, connectionToken, odsCode);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "Smith",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Smith",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = "A29928"
                }
            };

            await CreateApplicationsMapping(200, emisRequestBody);

            _context
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have data for a patient that does not exist")]
        public async Task GivenIHaveDataForAPatientThatDoesNotExist()
        {
            string[] nhsNumbers = { "notExistingNhsNumber" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(nhsNumbers, connectionToken, odsCode);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "Smith",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Smith",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = EmisDefaults.OdsCode
                }
            };

            await CreateApplicationsMapping(404, emisRequestBody);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have data for a patient that has already been associated with the application in the GP system")]
        public async Task GivenIHaveDataForAPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGpSystem()
        {
            string[] nhsNumbers = { "nhsNumber1" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(nhsNumbers, connectionToken, odsCode);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "AlreadyLinked",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "AlreadyLinked",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = EmisDefaults.OdsCode
                }
            };

            await CreateApplicationsMapping(409, emisRequestBody);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"EMIS Demographics endpoint is disable")]
        public async Task GivenEmisDemographicsEndpointIsDisable()
        {
            string[] nhsNumbers = { "nhsNumber1", "nhsNumber2" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;

            await MockIm1Connection(nhsNumbers, connectionToken, odsCode, (int)HttpStatusCode.Forbidden);

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = "Smith",
                    DateOfBirth = "1919-12-24T14:03:15.892Z"
                }
            );

            var emisRequestBody = new LinkApplicationRequest
            {
                Surname = "Smith",
                DateOfBirth = "1919-12-24T14:03:15.892Z",
                LinkageDetails = new LinkageDetails
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    NationalPracticeCode = EmisDefaults.OdsCode
                }
            };

            await CreateApplicationsMapping(403, emisRequestBody);

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }


        [When(@"I register an EMIS user's IM1 credentials")]
        public async Task WhenIRegisterAnEmisUsersIm1Credentials()
        {
            try
            {
                var result = await _context
                    .GetWorkerClient()
                    .PostIm1Connection(_context.GetIm1ConnectionRequest());

                _context.Add("GetIm1ConnectionResult", result);

            }
            catch (NhsoHttpException httpException)
            {
                Console.WriteLine(httpException);
                _context.SetHttpException(httpException);
            }
        }

        [Then(@"I receive the expected connection token and single NHS Number")]
        public void ThenIReceiveTheExpectedConnectionTokenAndSingleNhsNumber()
        {
            var result = _context.Get<Im1ConnectionResponse>("GetIm1ConnectionResult");
            var nhsNumbers = _context.GetNhsNumbers();
            var connectionToken = _context.GetConnectionToken();

            result.Should().NotBeNull();
            result.NhsNumbers.Should().HaveCount(1);
            result.NhsNumbers.First().NhsNumber.Should().Be(nhsNumbers.First());
            result.ConnectionToken.Should().Be(connectionToken);
        }

        [Then(@"I receive the expected connection token and multipe NHS Numbers")]
        public void ThenIReceiveTheExpectedConnectionTokenAndMultipeNhsNumbers()
        {
            var result = _context.Get<Im1ConnectionResponse>("GetIm1ConnectionResult");
            var nhsNumbers = _context.GetNhsNumbers();
            var connectionToken = _context.GetConnectionToken();

            result.Should().NotBeNull();
            result.NhsNumbers.Select(x => x.NhsNumber).Should().BeEquivalentTo(nhsNumbers);
            result.ConnectionToken.Should().Be(connectionToken);
        }

        [Then(@"I receive the expected connection token without NHS Numbers")]
        public void ThenIReceiveTheExpectedConnectionTokenWithoutNhsNumbers()
        {
            var result = _context.Get<Im1ConnectionResponse>("GetIm1ConnectionResult");
            var connectionToken = _context.GetConnectionToken();

            result.ConnectionToken.Should().Be(connectionToken);
            result.NhsNumbers.Should().BeEmpty();
        }

        private async Task MockIm1Connection(string[] nhsNumbers, string connectionToken, string odsCode, int statusCodeOk = (int)HttpStatusCode.OK)
        {
            if (nhsNumbers == null)
            {
                nhsNumbers = new string[0];
            }

             const int statusCodeCreated = (int)HttpStatusCode.Created;
             const string endUserSessionId = "bar";
             const string linkToken = "link_token";
             const string sessionId = "session_id";

            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateEndUserSessionMapping(statusCodeCreated, endUserSessionId));
            await _context.GetMockingClient().PostMappingAsync(SessionConfigurator.CreateSessionsMapping(statusCodeCreated, connectionToken, odsCode, sessionId, linkToken, AssociationType.Self, endUserSessionId));
            await _context.GetMockingClient().PostMappingAsync(DemographicsConfigurator.CreateDemographicsMapping(statusCodeOk, linkToken, sessionId, endUserSessionId, nhsNumbers));
        }

        private async Task CreateApplicationsMapping(int statusCode, LinkApplicationRequest requestBody)
        {
            var responseBody = new LinkApplicationResponse
            {
                AccessIdentityGuid = EmisDefaults.ConnectionToken
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
