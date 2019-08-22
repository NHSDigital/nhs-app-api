package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.methods.HttpGet
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

class WorkerClientServiceJourneyRules(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun getServiceJourneyRulesConfiguration(): ServiceJourneyRulesResponse {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.serviceJourneyRules)
        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        return gson.fromJson(result, ServiceJourneyRulesResponse::class.java)
    }
}
