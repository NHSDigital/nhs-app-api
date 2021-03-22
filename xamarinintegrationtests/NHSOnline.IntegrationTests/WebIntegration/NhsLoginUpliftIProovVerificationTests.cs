using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-10.1", "Capturing a face scan for identity verification in the PYI journey invokes iProov when iProov is enabled")]
    public class NhsLoginUpliftIProovVerificationTests
    {
        [NhsAppManualTest("NHSO-13695", "No reliable means via BrowserStack to determine the SDK is available, at the moment")]
        public void APatientWithProofLevelFiveWhenUpliftingCanVerifyTheirIdentityUsingIProovIOS() { }

        [NhsAppManualTest("NHSO-13794", "Currently unable to mock iProov SDK")]
        public void APatientWithProofLevelFiveWhenUpliftingCanVerifyTheirIdentityUsingIProovAndroid() { }
    }
}