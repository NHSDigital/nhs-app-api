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

    @Given("^I am a user wishing to enable push notifications$")
    fun iAmAUserWishingToRegisterTheirDeviceForPushNotifications() {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
        factory.mockNativeNotificationFunctions()
    }

    @Given("^I am a user wishing to disable push notifications$")
    fun iAmAUserWishingToUnRegisterTheirDeviceForPushNotifications() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null,
                listOf(ServiceJourneyRulesConfiguration("notifications", "enabled")))
        val factory = NotificationsFactory()
        factory.setUpUser(patient = patient)
        factory.setUpDeviceValues()
        factory.setUpExistingRegistration()
        factory.mockNativeNotificationFunctions()
    }

    @When("I change the notifications toggle to on")
    fun iChangeTheNotificationsToggleToOn() {
        notificationsSettingsPage.notificationsToggle.assertIsVisible()
        notificationsSettingsPage.notificationsToggle.assertOff()
        notificationsSettingsPage.notificationsToggle.click()
        notificationsSettingsPage.notificationsToggle.assertOn()
    }

    @When("I change the notifications toggle to off")
    fun iChangeTheNotificationsToggleToOff() {
        notificationsSettingsPage.notificationsToggle.assertIsVisible()
        notificationsSettingsPage.notificationsToggle.assertOn()
        notificationsSettingsPage.notificationsToggle.click()
        notificationsSettingsPage.notificationsToggle.assertOff()
    }

    @Then("the Notifications Settings page is displayed")
    fun theNotificationsSettingsPageIsDisplayed() {
        notificationsSettingsPage.assertDisplayed()
    }

    @Then("the notifications toggle is displayed as on")
    fun theNotificationsToggleIsDisplayedAsOn() {
        notificationsSettingsPage.notificationsToggle.assertOn()
    }

    @Then("the notifications toggle is displayed as off")
    fun theNotificationsToggleIsDisplayedAsOff() {
        notificationsSettingsPage.notificationsToggle.assertOff()
    }
}
