using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.IntegrationTests.UI;

namespace NHSOnline.IntegrationTests.LoggedOut
{
    [TestClass]
    [BusinessRule("BR-LOG-07.2", "User is prompted to logout or extend their session when their session is close to expiring")]
    [BusinessRule("BR-LOG-07.3", "Extending a session when prompted extends the session and remains on the same screen")]
    [BusinessRule("BR-LOG-07.4", "Logging out when prompted when a session is expiring logs the user out")]
    [BusinessRule("BR-LOG-07.5", "Log out due to a session timeout displays the logged out home screen with a banner message")]
    [BusinessRule("BR-LOG-07.6", "Navigating back to the app from a web integration when the app session has expired logs the user out")]
    public class SessionExpiryTests
    {
        [NhsAppManualTest("NHSO-13579", "Session expiry is not implemented yet")]
        public void APatientIsShownASessionExpiryPromptWhenTheirSessionIsCloseToExpiring() { }

        [NhsAppManualTest("NHSO-13579", "Session expiry is not implemented yet")]
        public void APatientChoosingToExtendTheirSessionRemainsLoggedIn() { }

        [NhsAppManualTest("NHSO-13579", "Session expiry is not implemented yet")]
        public void APatientChoosingToEndTheirSessionIsLoggedOut() { }

        [NhsAppManualTest("NHSO-13579", "Session expiry is not implemented yet")]
        public void APatientAllowingTheirSessionToExpireIsShownTheSessionExpiryBannerOnTheLoggedOutHomeScreen() { }

        [NhsAppManualTest("NHSO-13579", "Session expiry is not implemented yet")]
        public void APatientRemainingOnAWebIntegrationWhileTheirSessionExpiresIsLoggedOutWhenNavigatingBackToThe() { }
    }
}