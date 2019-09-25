package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import worker.models.messages.MessageRequest
import worker.models.messages.MessageResponse

class WorkerClientMessages(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(message: MessageRequest, nhsLoginId: String): HttpResponse {
        val httpPost = HttpPost(uri(nhsLoginId))

        val jsonRequest = gson.toJson(message)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsync(httpPost)
        httpPost.releaseConnection()
        return response!!
    }

    fun get(authToken: String?): Array<MessageResponse> {
        val httpGet = HttpGet(uri("me"))

        if (authToken != null) {
            httpGet.addHeader("Authorization", "Bearer $authToken")
        }

        val response = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        if (response != null) {
            return gson.fromJson(response, Array<MessageResponse>::class.java)
        }
        return arrayOf()
    }

    private fun uri(userIdentifier: String): String {
        return config.apiBackendUrl +
                WorkerPaths.userMessages.replace(WorkerPaths.userPlaceholder, userIdentifier)
    }
}