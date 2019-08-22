package features.pushNotifications.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import pages.account.NotificationsSettingsPage

class PushNotificationsStepDefinitions {

    val mockingClient = MockingClient.instance

    private lateinit var notificationsSettingsPage: NotificationsSettingsPage

    @Given("^I am a (\\w+) user wishing to register for push notifications$")
    fun iAmAUserWishingToRegisterTheirDeviceForPushNotifications(gpSystem: String) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(gpSystem,
                listOf(ServiceJourneyRulesConfiguration("notifications", "enabled")))
        val factory = NotificationsFactory()
        factory.setUpUser(gpSystem, patient)
        factory.setUpRegistration()
        factory.mockPNS(true)
    }

    @When ("I change the notifications toggle to enabled")
    fun iChangeTheNotificationsToggleToEnabled(){
        notificationsSettingsPage.notificationsToggle.assertIsVisible()
        notificationsSettingsPage.notificationsToggle.assertDisabled()
        notificationsSettingsPage.notificationsToggle.click()
        notificationsSettingsPage.notificationsToggle.assertEnabled()
    }

    @Then("the Notifications Settings page is displayed")
    fun theNotificationsSettingsPageIsDisplayed(){
        notificationsSettingsPage.assertDisplayed()
    }

    @Then ("the notifications toggle is displayed as enabled")
    fun theNotificationsToggleIsDisplayedAsEnabled(){
        notificationsSettingsPage.notificationsToggle.assertEnabled()
    }
}
