package features.pushNotifications.stepDefinitions

import features.authentication.steps.NotificationFailureSteps
import features.authentication.steps.NotificationSteps
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.BrowserSteps
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserDevice
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.account.NotificationsSettingsPage
import utils.GlobalSerenityHelpers
import utils.clearList
import utils.getOrFail

private const val SLEEP_TIME_FOR_PROMISE: Long = 4000

class PushNotificationsStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var notificationPromptSteps: NotificationSteps
    @Steps
    lateinit var notificationFailureSteps: NotificationFailureSteps

    val errorPage = ErrorPage()

    private lateinit var notificationsSettingsPage: NotificationsSettingsPage

    @Given("^I am a user wishing to enable push notifications$")
    fun iAmAUserWishingToEnablePushNotifications() {
        initialSetup(SettingStatus.Authorised, true)
    }

    @Given("^I am a user wishing to enable push notifications with existing incorrect user device$")
    fun iAmAUserWishingToEnablePushNotificationsWithExistingIncorrectUserDevice() {
        val factory = initialSetup(SettingStatus.Authorised, true)
        factory.setUpInvalidMongoDeviceRegistration()
    }

    @Given("^I am a user wishing to enable push notifications for the first time, with my initial state undetermined$")
    fun iAmAUserWishingToEnablePushNotificationsForTheFirstTime() {
        initialSetup(SettingStatus.NotDetermined, true)
    }

    @Given("^I am a user wishing to disable push notifications$")
    fun iAmAUserWishingToDisablePushNotifications() {
        val factory = initialSetup(SettingStatus.Authorised, true)
        factory.setUpExistingRegistration()
    }

    @Given("^I am a user wishing to enable push notifications with disabled device's notifications$")
    fun iAmAUserWishingToEnablePushNotificationsWithDisabledDeviceNotifications() {
        initialSetup(SettingStatus.Denied, false)
    }

    @When("^I change the notifications toggle to on$")
    fun iChangeTheNotificationsToggleToOn() {
        notificationsSettingsPage.notificationsToggle.assertIsVisible()
        notificationsSettingsPage.notificationsToggle.assertOff()
        notificationsSettingsPage.notificationsToggle.click()
    }

    @When("^I change the notifications toggle to off$")
    fun iChangeTheNotificationsToggleToOff() {
        notificationsSettingsPage.notificationsToggle.assertIsVisible()
        notificationsSettingsPage.notificationsToggle.assertOn()
        notificationsSettingsPage.notificationsToggle.click()
    }

    @When("^the push notification can no longer be found in the repository$")
    fun thePushNotificationCanNoLongerBeFoundInTheRepository() {
        MongoDBConnection.UserDevicesCollection.clearCache()
    }

    @When("^I disable notifications in the device's settings$")
    fun iDisableNotificationsInTheDeviceSettings() {
        resetScripts(SettingStatus.Denied, false)
    }

    @When("^I enable notifications in the device's settings$")
    fun iEnableNotificationsInTheDeviceSettings() {
        resetScripts(SettingStatus.Authorised, true)
    }

    @When("^I accept notifications and continue$")
    fun iAcceptNotificationsAndContinue() {
        notificationPromptSteps.acceptNotifications()
    }

    @When("^I accept notifications but I am denied$")
    fun iAcceptNotifications() {
        notificationPromptSteps.acceptNotificationsButUnauthorisedReturned()
    }

    @When("^I continue from the notification failure$")
    fun iContinueFromTheNotificationFailure() {
        notificationFailureSteps.continueToHome()
    }

    @When("^I do not accept notifications and continue$")
    fun iDontAcceptNotificationsAndContinue() {
        notificationPromptSteps.dontAcceptNotifications()
    }

    @Then("^the Notifications Settings page is displayed$")
    fun theNotificationsSettingsPageIsDisplayed() {
        notificationsSettingsPage.assertDisplayed()
    }

    @Then("^the notifications toggle is displayed as on$")
    fun theNotificationsToggleIsDisplayedAsOn() {
        notificationsSettingsPage.notificationsToggle.assertOn()
    }

    @Then("^the notifications toggle is displayed as off$")
    fun theNotificationsToggleIsDisplayedAsOff() {
        notificationsSettingsPage.notificationsToggle.assertOff()
    }

    @Then("^the push registration has been added to the repository$")
    fun thePushRegistrationHasBeenAddedToTheRepository() {
        MongoDBConnection.UserDevicesCollection.assertNumberOfDocuments(1)
        val pushRegistrations = MongoDBConnection.UserDevicesCollection
                .getValues<MongoRepositoryUserDevice>(MongoRepositoryUserDevice::class.java)
        Assert.assertNotNull("Push registrations", pushRegistrations)
        assertEquals("Number of push registrations", 1, pushRegistrations.count())
        val pushRegistration = pushRegistrations.first()
        Assert.assertNotNull("Push registration", pushRegistration)
        val expectedPnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        val expectedNhsLoginId = PushNotificationsSerenityHelpers.EXPECTED_NHS_LOGIN_ID.getOrFail<String>()
        assertEquals("Push registration nhsLoginId", expectedNhsLoginId, pushRegistration.NhsLoginId)
        assertEquals("Push registration token", expectedPnsToken, pushRegistration.PnsToken)
    }

    @Then("^the push registration has been removed from the repository$")
    fun thePushRegistrationHasBeenRemovedFromTheRepository() {
        MongoDBConnection.UserDevicesCollection.assertNumberOfDocuments(0)
    }

    @Then("^an error is displayed indicating that the notifications service is not available$")
    fun anErrorIsDisplayedIndicatingThatTheNotificationsServiceIsNotAvailable() {
        errorPage.assertHeaderText("Sorry, there is a problem with the service")
                .assertNoSubHeader()
                .assertMessageText("Go back to settings and try again.")
                .assertRetryButtonText("Back to settings")
    }

    @Then("^an error is displayed indicating that the device's notifications are disabled$")
    fun anErrorIsDisplayedIndicatingThatTheDeviceNotificationsAreDisabled() {
        errorPage.assertHeaderText("Notifications are turned off on your device")
                .assertNoSubHeader()
                .assertMessageText("To turn on notifications, go to your device settings and allow notifications." +
                        " Then return to the app and try again.")
                .assertRetryButtonText("Try again")
    }

    @Then("^an error is displayed indicating that it could not save because the device's notifications are disabled$")
    fun anErrorIsDisplayedIndicatingThatItCouldNotSaveBecauseTheDeviceNotificationsAreDisabled() {
        errorPage.assertHeaderText("Sorry, we could not change your notifications choice")
                .assertNoSubHeader()
                .assertMessageText("This might be because notifications are turned off in your device settings.")
                .assertErrorDetailText("Go to your device settings and check notifications are turned on," +
                        " then try again.")
                .assertRetryButtonText("Try again")
    }

    @Then("^I see the notifications prompt$")
    fun iSeeTheNotificationsPrompt() {
        Thread.sleep(SLEEP_TIME_FOR_PROMISE)
        notificationPromptSteps.notificationsPromptPage.assertDisplayed()
    }

    @Then("^I see the notification failure$")
    fun iSeeTheNotificationFailure() {
        notificationFailureSteps.notificationsPromptFailurePage.assertDisplayed()
    }

    private fun initialSetup(status: SettingStatus, authorised: Boolean): NotificationsFactory {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
        factory.mockNativeNotificationFunctions(status, authorised)

        return factory
    }

    private fun resetScripts(status: SettingStatus, authorised: Boolean) {
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.clearList<String>()
        val factory = NotificationsFactory()
        factory.mockNativeNotificationFunctions(status, authorised)

        browser.executeScripts()
    }
}
