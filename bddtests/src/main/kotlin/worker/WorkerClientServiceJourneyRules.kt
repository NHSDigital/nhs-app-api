package worker

import com.google.gson.Gson
import config.Config
import worker.models.serviceJourneyRules.ServiceJourneyRulesResponse

class WorkerClientServiceJourneyRules(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun getServiceJourneyRulesConfiguration(): ServiceJourneyRulesResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.serviceJourneyRules)
        return httpGet.sendAndGetResult(sender, gson, ServiceJourneyRulesResponse::class.java)
    }
}
