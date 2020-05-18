package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse

class WorkerClientSession(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun postSessionConnection(patientId: String): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.sessionConnectionExtend)
                .addHeader("NHSO-Patient-Id", patientId)
        return httpPost.send(sender)
    }
}