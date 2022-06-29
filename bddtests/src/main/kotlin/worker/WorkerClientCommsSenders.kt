package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.utils.URIBuilder
import worker.models.messages.CommsSenderRequest
import worker.models.messages.CommsSenderResponse

class WorkerClientCommsSenders(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun get(senderId:String, includeApiKey:Boolean): CommsSenderResponse? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.commsSender + '/' + senderId)

        val httpGet = RequestBuilder
            .get(uriBuilder.build().toString())
            .addExternalSystemApiKey(includeApiKey)

        val response = httpGet.sendAndGetResult(sender)

        if (response != null) {
            return gson.fromJson(response, CommsSenderResponse::class.java)
        }

        return null
    }

    fun getLastUpdatedBefore(lastUpdatedBefore: String, limit: Int, includeApiKey: Boolean): List<String>? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.commsSender
            + "?lastUpdatedBefore=" + lastUpdatedBefore + "&limit=" + limit)

        val httpGet = RequestBuilder
            .get(uriBuilder.build().toString())
            .addExternalSystemApiKey(includeApiKey)

        val response = httpGet.sendAndGetResult(sender)

        if (response != null) {
            return gson.fromJson(response, Array<String>::class.java).toList()
        }

        return null
    }

    fun post(senderDetails: CommsSenderRequest, includeApiKey:Boolean): CommsSenderResponse? {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.commsSender)

        val httpPost = RequestBuilder
            .post(uriBuilder.build().toString())
            .addBody(senderDetails, gson)
            .addExternalSystemApiKey(includeApiKey)

        val response = httpPost.sendAndGetResult(sender)

        if (response != null) {
            return gson.fromJson(response, CommsSenderResponse::class.java)
        }

        return null
    }
}
