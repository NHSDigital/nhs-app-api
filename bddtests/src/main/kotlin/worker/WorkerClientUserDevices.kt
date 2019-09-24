package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpDelete
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.utils.URIBuilder
import org.apache.http.entity.StringEntity
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(registration: RegisterUserDevicesRequest,
             authToken: String?): RegisterUserDevicesResponse {
        val httpPost = HttpPost(config.apiBackendUrl + WorkerPaths.userDevices)

        if (authToken != null) {
            httpPost.addHeader("Authorization", "Bearer $authToken")
        }

        val jsonRequest = gson.toJson(registration)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsyncAndGetResult(httpPost)
        httpPost.releaseConnection()

        return gson.fromJson<RegisterUserDevicesResponse>(response, RegisterUserDevicesResponse::class.java)
    }

    fun get(devicePns: String, authToken: String?): HttpResponse {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userDevices)
        uriBuilder.setParameter("devicePns", devicePns)
        val httpGet = HttpGet(uriBuilder.build())
        if (authToken != null) {
            httpGet.addHeader("Authorization", "Bearer $authToken")
        }
        val response = sender.sendAsync(httpGet)
        httpGet.releaseConnection()
        return response!!
    }

    fun delete(devicePns: String, authToken: String?): HttpResponse {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.userDevices)
        uriBuilder.setParameter("devicePns", devicePns)
        val httpDelete = HttpDelete(uriBuilder.build())
        if (authToken != null) {
            httpDelete.addHeader("Authorization", "Bearer $authToken")
        }
        val response = sender.sendAsync(httpDelete)
        httpDelete.releaseConnection()
        return response!!
    }
}