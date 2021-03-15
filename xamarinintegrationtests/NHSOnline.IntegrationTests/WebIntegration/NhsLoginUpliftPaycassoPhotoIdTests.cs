using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-11.7", "Invoking Paycasso in the NHS login uplift journey to capture an image of a photo ID displays Paycasso")]
    public class NhsLoginUpliftPaycassoPhotoIdTests
    {
        [NhsAppManualTest("NHSO-13721", "Not implemented as Paycasso in NHS login is on hold")]
        public void APatientWithProofLevelFiveCanStartPaycassoToTakeAPhotoOfTheirPhotoIdIos()
        {
        }
    }
}