using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.WebIntegration
{
    [TestClass]
    [BusinessRule("BR-LOG-11.5", "Invoking Paycasso in the NHS login uplift journey to capture an image of a drivers licence displays Paycasso")]
    public class NhsLoginUpliftPaycassoDrivingLicenceTests
    {
        [NhsAppManualTest("NHSO-13720", "Not implemented as Paycasso in NHS login is on hold")]
        public void APatientWithProofLevelFiveCanStartPaycassoToTakeAPhotoOfTheirPhotoIdIos()
        {
        }
    }
}