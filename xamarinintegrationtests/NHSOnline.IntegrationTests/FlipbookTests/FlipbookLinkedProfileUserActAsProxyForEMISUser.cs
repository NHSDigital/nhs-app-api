using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.IntegrationTests.Pages.IOS.Home;
using NHSOnline.IntegrationTests.Pages.IOS.LinkedProfile;
using NHSOnline.IntegrationTests.Pages.IOS.More;
using NHSOnline.IntegrationTests.UI;
using NHSOnline.IntegrationTests.UI.Drivers;


namespace NHSOnline.IntegrationTests.FlipbookTests
{
    [TestClass]
    public class FlipbookLinkedProfileUserActAsProxyForEmisUser
    {
        [NhsAppIOSTest]
        [NhsAppFlipbookTest(ParentJourney = "A user logs into the app - iOS",
            FlipbookTestName = "A user access a linked profile to act as proxy for another user (EMIS)")]
        public void UserWithLinkedProfileUserActAsProxyForEmisUser(IIOSDriverWrapper driver)
        {
            var linkedProfile = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithName(b => b.GivenName("Proxy").FamilyName("User"));
            var patient = new EmisPatient(EmisPatientOds.AllSilversEnabled)
                .WithLinkedProfileName(linkedProfile);
            Mocks.Patients.Add(linkedProfile);
            using var patients = Mocks.Patients.Add(patient);

            LoginProcess.LogIOSPatientIn(driver, patient);

            IOSLoggedInHomePage
                .AssertOnPage(driver, screenshot:true)
                .Navigation.NavigateToMore();

            IOSMorePage
                .AssertOnPage(driver, true)
                .NavigateToLinkedProfiles();

           IOSLinkedProfilesPage
               .AssertOnPage(driver, linkedProfile, true)
               .NavigateToLinkedProfile();

           IOSSwitchToLinkedProfilePage
               .AssertOnPage(driver, linkedProfile, true)
               .AssertPageElements()
               .NavigateToSwitchToLinkedProfile();

           IOSLoggedInHomePage
               .AssertOnPage(driver, true)
               .PageContent
               .AssertLinkedProfileYellowBannerVisible(linkedProfile.PersonalDetails.Name.GivenName + " " + linkedProfile.PersonalDetails.Name.FamilyName);
        }
    }
}