package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpPost

class WorkerClientSession(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun postSessionConnection(patientId: String): HttpResponse {
        val httpPost = HttpPost(config.apiBackendUrl + WorkerPaths.sessionConnectionExtend)
        httpPost.addHeader("NHSO-Patient-Id", patientId)

        val response = sender.sendAsync(httpPost)
        httpPost.releaseConnection()
        return response!!
    }
}