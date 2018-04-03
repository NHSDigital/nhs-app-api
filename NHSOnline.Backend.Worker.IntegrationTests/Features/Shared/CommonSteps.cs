using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.IntegrationTests.Features.Emis.Im1Connection.Verification;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features.Shared
{
    [Binding]
    public class CommonSteps
    {
        private readonly ScenarioContext _context;

        public CommonSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"EMIS is unavailable")]
        public async Task GivenEmisIsUnavailable()
        {
            _context
                .SetConnectionToken(PatientVerificationSteps.DefaultConnectionToken)
                .SetOdsCode(PatientVerificationSteps.DefaultOdsCode);

            await _context
                .GetMockingClient()
                .PostMappingAsync(SessionConfigurator.CreateEndUserSessionMappingWithError((int)HttpStatusCode.ServiceUnavailable, "service unavailable"));
        }

        [Then(@"I receive (?:a|an) ""(.*)"" error")]
        public void ThenIReceiveAnMessage(HttpStatusCode expectedStatusCode)
        {
            var exception = _context.GetHttpException();
            exception.Should().NotBeNull("An exception was expected but was not returned within the expected time limit.");
            exception.StatusCode.Should().Be(expectedStatusCode);
        }
    }
}
