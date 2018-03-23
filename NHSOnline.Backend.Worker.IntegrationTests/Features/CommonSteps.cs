using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.IntegrationTests.Mocking.Emis;
using TechTalk.SpecFlow;

namespace NHSOnline.Backend.Worker.IntegrationTests.Features
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
            _context.Add("TimeoutDelay", TimeSpan.FromMinutes(5));
            await _context
                .GetMockingClient()
                .PostMappingAsync(SessionConfigurator.CreateEndUserSessionMappingWithTimout(TimeSpan.FromMinutes(5)));
        }

        [Then(@"I receive (?:a|an) ""(.*)"" error")]
        public void ThenIReceiveAnMessage(HttpStatusCode expectedStatusCode)
        {
            var exception = _context.GetHttpException();
            Assert.IsNotNull(exception, "An exception was expected but was not returned within the expected time limit.");
            Assert.AreEqual(expectedStatusCode, exception.StatusCode);
        }
    }
}
