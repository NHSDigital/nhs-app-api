package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(
             registration: RegisterUserDevicesRequest,
             authToken: String?): RegisterUserDevicesResponse {
        val httpPost = HttpPost(config.usersBackendUrl + WorkerPaths.userDevices)

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
}