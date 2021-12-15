package features.pushNotifications.stepDefinitions
import io.cucumber.java.en.Then
import utils.SerenityHelpers
import worker.models.messages.SenderContext
import worker.models.userDevices.NotificationSendRequest
import java.time.Instant
import java.time.temporal.ChronoUnit

class PushNotificationsStepDefinitionsBackendSend {

    @Then("^I send the notification$")
    fun iSendTheNotification() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val notification = NotificationSendRequest("title", "subtitle", "body", "http://www.example.com", null)
        NotificationsApi.postNotification(nhsLoginId, notification)
    }

    @Then("^I send the notification with sender context$")
    fun iSendTheNotificationWithSenderContext() {
        val senderContext = SenderContext(
                supplierId = "supplierId",
                communicationId = "communicationId",
                transmissionId = "transmissionId",
                communicationCreatedDateTime = Instant.now().truncatedTo(ChronoUnit.MILLIS).toString(),
                requestReference = "requestReference",
                campaignId = "campaignId",
                odsCode = "odsCode",
                nhsNumber = "nhsNumber",
                nhsLoginId = "nhsLoginId"
        )
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val notification = NotificationSendRequest("title", "subtitle", "body", "http://www.example.com", senderContext)
        NotificationsApi.postNotification(nhsLoginId, notification)
    }

    @Then("^I send a malformed notification$")
    fun iSendAMalformedNotification() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val notification = NotificationSendRequest("title", "subtitle", null, "http://www.example.com")
        NotificationsApi.postNotification(nhsLoginId, notification)
    }
}
