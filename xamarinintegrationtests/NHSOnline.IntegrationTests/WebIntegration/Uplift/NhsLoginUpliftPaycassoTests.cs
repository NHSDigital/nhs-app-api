using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.WebIntegration.Uplift
{
    [TestClass]
    [BusinessRule("BR-LOG-11.3", "Invoking Paycasso in the NHS login uplift journey to capture an image of a drivers licence displays Paycasso")]
    [BusinessRule("BR-LOG-11.7", "Invoking Paycasso in the NHS login uplift journey to capture an image of a photo ID displays Paycasso")]
    public class NhsLoginUpliftPaycassoTests
    {
        [NhsAppManualTest("NHSO-13405", "Paycasso integration is currently on hold")]
        public void APatientInvokingPaycassoThroughTheirDriversLicenceIsShownThePaycassoJourney() { }

        [NhsAppManualTest("NHSO-13405", "Paycasso integration is currently on hold")]
        public void APatientInvokingPaycassoThroughTheirPhotoIdIsShownThePaycassoJourney() { }
    }
}