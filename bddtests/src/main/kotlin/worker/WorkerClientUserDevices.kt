package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
import worker.models.userDevices.NotificationSendRequest
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

// Azure Notification Hubs (ANH) do not instantly create searchable records. This delay is used to ensure that tests
// which setup data using these will always give ANH time to create the records that will be searched for later.
// Polling would be preferable but due to the way the users GET endpoint works this approach causes more failures
// when it cleans orphaned records.
private const val WAIT_FOR_HUB_UPDATE = 1000L

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(registration: RegisterUserDevicesRequest,
             authToken: String?): RegisterUserDevicesResponse? {
        val httpPost = RequestBuilder.post(uri("me"))
                .addBody(registration, gson)
                .addAuthorizationIfNotNull(authToken)
        val result = httpPost.sendAndGetResult(sender, gson, RegisterUserDevicesResponse::class.java)
        Thread.sleep(WAIT_FOR_HUB_UPDATE)
        return result
    }

    fun get(devicePns: String, authToken: String?): HttpResponse? {
        val uriBuilder = URIBuilder(uri("me"))
        uriBuilder.setParameter("devicePns", devicePns)
        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
                .addAuthorizationIfNotNull(authToken)
        return httpGet.send(sender)
    }

    fun delete(devicePns: String, authToken: String?): HttpResponse? {
        val uriBuilder = URIBuilder(uri("me"))
        uriBuilder.setParameter("devicePns", devicePns)
        val httpDelete = RequestBuilder.delete(uriBuilder.build().toString())
                .addAuthorizationIfNotNull(authToken)
        val result = httpDelete.send(sender)
        Thread.sleep(WAIT_FOR_HUB_UPDATE)
        return result
    }

    fun getRegistrations(nhsLoginId: String, includeApiKey: Boolean): Array<String>? {
        val uriBuilder = URIBuilder(uri(nhsLoginId) + "/registrations")
        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
                .addExternalSystemApiKey(includeApiKey)
        return httpGet.sendAndGetResult(sender, gson, Array<String>::class.java)
    }

    fun postNotification(nhsLoginId: String, notification: NotificationSendRequest, includeApiKey:Boolean)
            : HttpResponse? {
        val uriBuilder = URIBuilder(uri(nhsLoginId) + "/notifications")
        val httpPost = RequestBuilder.post(uriBuilder.build().toString())
                .addBody(notification, gson)
                .addExternalSystemApiKey(includeApiKey)
        return httpPost.send(sender)
    }

    private fun uri(userIdentifier: String): String {
        return config.apiBackendUrl +
                WorkerPaths.userDevices.replace(WorkerPaths.userPlaceholder, userIdentifier)
    }
}
