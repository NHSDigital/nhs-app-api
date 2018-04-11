using System.Collections.Generic;
using System.Threading.Tasks;
using NHSOnline.Backend.Worker.IntegrationTests.Worker.Models.Patient;
using NHSOnline.Backend.Worker.Mocking.Emis;
using NHSOnline.Backend.Worker.Mocking.Models;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Registration
{
    [Binding]
    class PatientRegistrationValidationSteps
    {
        private readonly Dictionary<string, string> _headers;
        private readonly ScenarioContext _context;

        public PatientRegistrationValidationSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I have an EMIS user's IM1 credentials with an ODS Code not in the expected format")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithAnOdsCodeNotInTheExpectedFormat()
        {
            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = "MASTER_YODA",
                    LinkageKey = "MASTER000YODA",
                    OdsCode = "xxx-wrong-format-xxx",
                    Surname = "Yoda",
                    DateOfBirth = "1919-12-24T14:03:15.892"
                }
            );
            _context.SetHttpExceptionExpected(true);
        }

        [Given(@"I have an EMIS user's IM1 credentials with a Surname not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithASurnameNotInTheExpectedFormat()
        {
            const string odsCode = "A82010";
            const string surname = "xxx-wrong-format-xxx";
            const string dateOfBirth = "1919-12-24T14:03:15.892Z";
            const string accountId = "MASTER_YODA";
            const string linkageKey = "MASTER000YODA";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";

            _context.SetIm1ConnectionRequest(new Im1ConnectionRequest
                {
                    AccountId = accountId,
                    LinkageKey = linkageKey,
                    OdsCode = odsCode,
                    Surname = surname,
                    DateOfBirth = dateOfBirth
                }
            );

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithBadRequest("The Surname value cannot exceed 100 characters."));
        }

        [Given(@"I have an EMIS user's IM1 credentials with an Account ID not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithAnAccountIdNotInTheExpectedFormat()
        {
            const string odsCode = "A82010";
            const string surname = "Yoda";
            const string dateOfBirth = "1919-12-24T14:03:15.892Z";
            const string accountId = "xxx-wrong-format-xxx";
            const string linkageKey = "MASTER000YODA";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";

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

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithBadRequest("AccountId length outside of valid range. Must be between 10 - 15 (inclusive) characters."));
        }

        [Given(@"I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format")]
        public void GivenIHaveAnEmisUsersIm1CredentialsWithADateOfBirthNotInTheExpectedFormat()
        {
            const string odsCode = "A82010";
            const string surname = "Yoda";
            const string dateOfBirth = "xxx-wrong-format-xxx";
            const string accountId = "MASTER_YODA";
            const string linkageKey = "MASTER000YODA";

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

        [Given(@"I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format")]
        public async Task GivenIHaveAnEmisUsersIm1CredentialsWithALinkageKeyNotInTheExpectedFormat()
        {
            const string odsCode = "A82010";
            const string surname = "Yoda";
            const string dateOfBirth = "1919-12-24T14:03:15.892Z";
            const string accountId = "MASTER_YODA";
            const string linkageKey = "xxx-wrong-format-xxx";
            const string endUserSessionId = "zVfHuYArbENW4aoAUeQPyS";

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

            await PostMapping(EndUserSessionConfigurator
                .ForRequest()
                .RespondWithSuccess(endUserSessionId));

            await PostMapping(MeConfigurator
                .ForRequest(endUserSessionId, surname, dateOfBirth, accountId, linkageKey, odsCode)
                .RespondWithBadRequest("LinkageKey length outside of valid range. Must be between 6 - 15 (inclusive) characters."));
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
            _context.SetHttpExceptionExpected(true);
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
            _context.SetHttpExceptionExpected(true);
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
            _context.SetHttpExceptionExpected(true);
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
            _context.SetHttpExceptionExpected(true);
        }

        private async Task PostMapping(Mapping mapping)
        {
            await _context.GetMockingClient()
                .PostMappingAsync(mapping);
        }
    }
}
