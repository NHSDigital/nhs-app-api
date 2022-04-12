package features.pushNotifications.stepDefinitions
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.pushNotifications.NotificationOutComeResponse
import worker.models.pushNotifications.PushNotificationResponse
import worker.models.userDevices.NotificationSendRequest

class PushNotificationsStepDefinitionsBackendNotificationOutcome {

    @Given("^I am api user trying to get the notification outcome details using invalid hub path$")
    fun iGetTheNotificationOutComeDetailsUsingAnInvalidHubPath()
    {
        NotificationsApi.getNotificationOutComeDetails("notificationId","")
    }

    @Given("^I am api user trying to send a notification to a given Nhs Login Id$")
    fun iSendNotificationToAnNhsLoginId()
    {
        val factory = NotificationsFactory()
        val patient = factory.setUpAlternativeUser()
        SerenityHelpers.setPatient(patient)
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val notification = NotificationSendRequest("title", "subtitle", "body",
                                                "http://www.example.com", null)
        NotificationsApi.postNotification(nhsLoginId, notification)
    }

    @Then("^I am api user trying to get the notification outcome details$")
    fun iAmTryingToGetNotificationOutComeDetails()
    {
        val notificationResponse = PushNotificationsSerenityHelpers.CREATE_PUSH_NOTIFICATION_RESPONSE
                                    .getOrFail<PushNotificationResponse>()
        NotificationsApi.getNotificationOutComeDetails(notificationResponse.notificationId,notificationResponse.hubPath)
    }

    @Then("^I receive a response with outcome details$")
    fun iReceiveResponseWithOutComeDetails()
    {
        val notificationOutComeResponse = PushNotificationsSerenityHelpers.GET_NOTIFICATION_OUTCOME_RESPONSE
                                            .getOrFail<NotificationOutComeResponse>()
        Assert.assertNotNull(notificationOutComeResponse)
        Assert.assertNotNull(notificationOutComeResponse.state)
        Assert.assertNotNull(notificationOutComeResponse.platformOutcomes)
    }

    @Then("^I am api user trying to get the notification outcome details using non existent hub path$")
    fun iGetTheNotificationOutComeDetailsUsingNonExistentHubPath()
    {
        NotificationsApi.getNotificationOutComeDetails("notificationId","non-existent-hub-id");
    }

    @Given("^I have a valid hub path$")
    fun iHaveAValidHubPath()
    {
        val factory = NotificationsFactory()
        val patient = factory.setUpAlternativeUser()
        SerenityHelpers.setPatient(patient)
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val notification = NotificationSendRequest("title", "subtitle", "body",
                "http://www.example.com", null)
        NotificationsApi.postNotification(nhsLoginId, notification)
    }

    @Then("^I am api user trying to get the notification outcome details using an non existent notification id$")
    fun iGetTheNotificationOutComeDetailsUsingNonExistentNotificationId()
    {
        val notificationResponse = PushNotificationsSerenityHelpers.CREATE_PUSH_NOTIFICATION_RESPONSE
                .getOrFail<PushNotificationResponse>()
        NotificationsApi.getNotificationOutComeDetails("non-existent-notification-id",notificationResponse.hubPath)
    }

}
