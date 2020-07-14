package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(registration: RegisterUserDevicesRequest,
             authToken: String?): RegisterUserDevicesResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.userDevices)
                .addBody(registration, gson)
                .addAuthorizationIfNotNull(authToken)
        return httpPost.sendAndGetResult(sender, gson, RegisterUserDevicesResponse::class.java)
    }

    fun get(devicePns: String, authToken: String?): HttpResponse? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userDevices)
        uriBuilder.setParameter("devicePns", devicePns)
        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
                .addAuthorizationIfNotNull(authToken)
        return httpGet.send(sender)
    }

    fun delete(devicePns: String, authToken: String?): HttpResponse? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userDevices)
        uriBuilder.setParameter("devicePns", devicePns)
        val httpDelete = RequestBuilder.delete(uriBuilder.build().toString())
                .addAuthorizationIfNotNull(authToken)
        return httpDelete.send(sender)
    }
}
