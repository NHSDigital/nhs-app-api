package features.pushNotifications.stepDefinitions

import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.pushNotifications.NotificationOutComeResponse
import worker.models.pushNotifications.PushNotificationResponse
import worker.models.userDevices.NotificationSendRequest
import worker.models.userDevices.RegisterUserDevicesRequest

class NotificationsApi {
    companion object {
        fun setupRegistration(authToken: String?) = postRegistration(authToken)

        fun postRegistration(authToken: String?) {
            val pnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
            val deviceType = PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>()
            val installationId = PushNotificationsSerenityHelpers.EXPECTED_INSTALLATION_ID.getOrFail<String>()
            val request = RegisterUserDevicesRequest(pnsToken, deviceType, installationId)
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .post(request, authToken)
            PushNotificationsSerenityHelpers.REGISTER_RESPONSE.set(response)
        }

        fun getRegistration(authToken: String?) {
            val pnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .get(pnsToken, authToken)
            PushNotificationsSerenityHelpers.GET_RESPONSE.set(response)
        }

        fun checkUserExists(authToken: String?): Boolean {
            val pnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .get(pnsToken, authToken)

            if (response?.statusLine !== null &&
                    response.statusLine?.statusCode !== null &&
                    response.statusLine.statusCode == HttpStatus.SC_NO_CONTENT) {
                return true
            }

            return false
        }

        fun deleteRegistration(authToken: String?, pnsToken: String? = null) {
            val pnsTokenToDelete = pnsToken ?: PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail()
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .delete(pnsTokenToDelete, authToken)
            PushNotificationsSerenityHelpers.DELETE_RESPONSE.set(response)
        }

        fun postNotification(nhsLoginId: String, notification: NotificationSendRequest): PushNotificationResponse? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .postNotification(nhsLoginId, notification, true)
            PushNotificationsSerenityHelpers.CREATE_PUSH_NOTIFICATION_RESPONSE.set(response)
            return response
        }

        fun getNotificationOutComeDetails(notificationId: String, hubPath: String) : NotificationOutComeResponse? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .getNotificationOutComeDetails(notificationId, hubPath,true)
            PushNotificationsSerenityHelpers.GET_NOTIFICATION_OUTCOME_RESPONSE.set(response)
            return response
        }

    }
}
