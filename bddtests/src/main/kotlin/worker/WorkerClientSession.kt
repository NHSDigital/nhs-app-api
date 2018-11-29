package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpPost

class WorkerClientSession(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun postSessionConnection(): HttpResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.sessionConnectionExtend)

        val response = sender.sendAsync(httpPost)
        httpPost.releaseConnection()
        return response
    }
}