package features.pushNotifications.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.userDevices.RegisterUserDevicesRequest

class NotificationsApi {

    companion object {
        fun setupRegistration(authToken: String?) = postRegistration(authToken)

        fun postRegistration(authToken: String?) {
            val pnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
            val deviceType = PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>()
            val request = RegisterUserDevicesRequest(pnsToken, deviceType)
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

        fun deleteRegistration(authToken: String?, pnsToken: String? = null) {
            val pnsTokenToDelete = pnsToken ?: PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail()
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .delete(pnsTokenToDelete, authToken)
            PushNotificationsSerenityHelpers.DELETE_RESPONSE.set(response)
        }
    }
}