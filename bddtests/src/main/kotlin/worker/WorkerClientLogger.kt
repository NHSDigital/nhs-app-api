package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import worker.models.clientLogger.ClientLoggerRequest

class WorkerClientLogger(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun post(log: ClientLoggerRequest): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.clientLogger)
                .addBody(log, gson)

        return httpPost.send(sender)
    }
}

