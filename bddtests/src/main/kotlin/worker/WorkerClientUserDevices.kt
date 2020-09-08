package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
import worker.models.userDevices.NotificationSendRequest
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(registration: RegisterUserDevicesRequest,
             authToken: String?): RegisterUserDevicesResponse? {
        val httpPost = RequestBuilder.post(uri("me"))
                .addBody(registration, gson)
                .addAuthorizationIfNotNull(authToken)
        return httpPost.sendAndGetResult(sender, gson, RegisterUserDevicesResponse::class.java)
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
        return httpDelete.send(sender)
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
