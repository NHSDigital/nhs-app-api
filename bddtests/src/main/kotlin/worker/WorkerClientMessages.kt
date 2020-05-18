package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
import worker.models.messages.MessageRequest
import worker.models.messages.MessagesResponse

class WorkerClientMessages(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(message: MessageRequest, nhsLoginId: String, includeApiKey:Boolean)
            : HttpResponse? {
        val httpPost = RequestBuilder.post(uri(nhsLoginId))
                .addBody(message, gson)
                .addExternalSystemApiKey(includeApiKey)
        return httpPost.send(sender)
    }

    fun get(authToken: String?, summary:Boolean, targetSender:String? =null): Array<MessagesResponse> {
        val uriBuilder = URIBuilder(uri("me"))
        uriBuilder.setParameter("summary", summary.toString())
        uriBuilder.setParameterIfNotNull("sender", targetSender)

        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
                .addAuthorizationIfNotNull(authToken)
        val response = httpGet.sendAndGetResult(sender)
        if (response != null) {
            return gson.fromJson(response, Array<MessagesResponse>::class.java)
        }
        return arrayOf()
    }

    fun patch(authToken: String?, messageId: String, patch: JsonPatch): HttpResponse? {
        val uriBuilder = URIBuilder(uri("me") +"/"+ messageId)
        val httpPatch = RequestBuilder.patch(uriBuilder.build().toString())
                .addBody(arrayListOf(patch), gson)
                .addAuthorizationIfNotNull(authToken)
        return httpPatch.send(sender)
    }

    private fun uri(userIdentifier: String): String {
        return config.apiBackendUrl +
                WorkerPaths.userMessages.replace(WorkerPaths.userPlaceholder, userIdentifier)
    }
}

