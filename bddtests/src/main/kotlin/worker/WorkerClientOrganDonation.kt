package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.client.methods.HttpGet
import worker.models.organdonation.OrganDonationSearchResponse

class WorkerClientOrganDonation(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getOrganDonationConnection(): OrganDonationSearchResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.organDonationConnection)

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, OrganDonationSearchResponse::class.java)
    }
}
