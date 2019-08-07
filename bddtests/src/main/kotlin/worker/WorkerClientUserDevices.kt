package worker

import com.google.gson.Gson
import config.Config
import mocking.AccessTokenBuilder
import models.Patient
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class WorkerClientUserDevices(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(patient: Patient,
             registration: RegisterUserDevicesRequest,
             withAuthToken: Boolean = true): RegisterUserDevicesResponse {
        val httpPost = HttpPost(config.usersBackendUrl + WorkerPaths.userDevices)

        if (withAuthToken) {
            val authToken = AccessTokenBuilder().getSignedToken(patient).serialize()
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
