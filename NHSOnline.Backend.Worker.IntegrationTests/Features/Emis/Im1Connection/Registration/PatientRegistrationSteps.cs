using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NHSOnline.Backend.Worker.IntegrationTests.Worker;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Emis.Models;
using NHSOnline.Backend.Worker.Mocking.Models;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Registration
{
    [Binding]
    class PatientRegistrationSteps
    {
        private readonly ScenarioContext _context;

        public PatientRegistrationSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have valid patient data to register new account")]
        public async Task GivenIHaveValidPatientDataToRegisterNewAccount()
        {
            const string endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5";
            const string sessionId = "MT4vWCxTKXRYr7fFJWM3wB";
            const string userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u";
            const string odsCode = EmisDefaults.OdsCode;
            const string accountId = "1195029928";
            const string title = "Mr";
            const string firstName = "John";
            const string surname = "Smith";
            const string dateOfBirth = "1919-12-24T14:03:15.892Z";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string nhsNumber = "7174450393";
            
            const string connectionToken = EmisDefaults.ConnectionToken;

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithSuccess(EmisDefaults.ConnectionToken));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, new[] { PatientIdentifier.NHSNumber(nhsNumber) }));


            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

            _context
                .SetNhsNumbers(new[] { nhsNumber })
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have valid patient data with multiple nhs numbers to register new account")]
        public async Task GivenIhaveValidPatientDataWithMultipleNhsNumbersToRegisterNewAccount()
        {
            string[] nhsNumbers = { "nhsNumber1", "nhsNumber2" };
            const string connectionToken = EmisDefaults.ConnectionToken;
            const string odsCode = EmisDefaults.OdsCode;
            const string title = "Miss";
            const string firstName = "Gertrude";
            const string surname = "Jones";
            const string dateOfBirth = "1965-08-12T00:00:00Z";
            const string accountId = "1195029928";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";
            const string sessionId = "DPcqihby6RDaVrnf4hHyv7";
            const string userPatientLinkToken = "BvN2TuCzPtHLZvv5ZgK6wN";

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithSuccess(EmisDefaults.ConnectionToken));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, nhsNumbers.Select(PatientIdentifier.NHSNumber)));


            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

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
            const string title = "Mrs";
            const string firstName = "Jackie";
            const string surname = "Thompson";
            const string dateOfBirth = "2001-01-02T11:00:52Z";
            const string accountId = "1195029928";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string endUserSessionId = "zVGrzHH7YUPeEBRk1nat1D";
            const string sessionId = "h3pYG9By2tVTqcvPvpw3DL";
            const string userPatientLinkToken = "5d4p6ZExhi97mmerMrtD5p";

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithSuccess(EmisDefaults.ConnectionToken));

            await PostMapping(SessionConfigurator
                .ForRequest(connectionToken, odsCode, endUserSessionId)
                .RespondWithSuccess(sessionId, title, firstName, surname, userPatientLinkToken, odsCode, AssociationType.Self));

            await PostMapping(DemographicsConfigurator
                .ForRequest(endUserSessionId, sessionId, userPatientLinkToken)
                .RespondWithSuccess(title, firstName, surname, Enumerable.Empty<PatientIdentifier>()));

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

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
            const string surname = "Smith";
            const string dateOfBirth = "1919-12-24T14:03:15Z";
            const string accountId = "1195029928";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";


            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithNoOnlineUserFound());


            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = EmisDefaults.OdsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

            _context
                .SetNhsNumbers(nhsNumbers)
                .SetConnectionToken(connectionToken)
                .SetOdsCode(odsCode);
        }

        [Given(@"I have data for a patient that has already been associated with the application in the GP system")]
        public async Task GivenIHaveDataForAPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGpSystem()
        {
            const string odsCode = EmisDefaults.OdsCode;
            const string surname = "AlreadyLinked";
            const string dateOfBirth = "1919-12-24T14:03:15Z";
            const string accountId = "1195029928";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";


            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithUserAlreadyLinked());

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = odsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

            _context.SetHttpExceptionExpected(true);
        }

        [Given(@"EMIS Demographics endpoint is disable")]
        public async Task GivenEmisDemographicsEndpointIsDisable()
        {
            const string odsCode = EmisDefaults.OdsCode;
            const string surname = "Smith";
            const string dateOfBirth = "1919-12-24T14:03:15Z";
            const string accountId = "1195029928";
            const string linkageKey = "KjwzyFSEUAGj4";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";


            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithInvalidLinkLevel());

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "1195029928",
                    LinkageKey = "KjwzyFSEUAGj4",
                    OdsCode = odsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );
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
                if (_context.GetHttpExceptionExpected())
                {
                    _context.SetHttpException(httpException);
                }
                else
                {
                    throw;
                }
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

        private async Task PostMapping(Mapping mapping)
        {
            await _context.GetMockingClient()
                .PostMappingAsync(mapping);
        }
    }
}
