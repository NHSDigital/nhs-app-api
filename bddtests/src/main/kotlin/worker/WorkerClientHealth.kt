package worker

import com.google.gson.Gson
import org.apache.http.HttpResponse

class WorkerClientHealth(val baseUrl: String, val sender: WorkerClientSender, val gson: Gson) {

    fun getReady(): HttpResponse? {
        val httpPost = RequestBuilder.get(baseUrl + WorkerPaths.healthReady)

        return httpPost.send(sender)
    }
}
