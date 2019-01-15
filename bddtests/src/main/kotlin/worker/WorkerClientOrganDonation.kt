package worker

import com.google.gson.Gson
import config.Config
import mocking.organDonation.models.OrganDonationRegistrationRequest
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import worker.models.organdonation.OrganDonationRegistrationResponse
import worker.models.organdonation.OrganDonationSearchResponse

class WorkerClientOrganDonation(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getOrganDonationConnection(): OrganDonationSearchResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.organDonationConnection)

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, OrganDonationSearchResponse::class.java)
    }

    fun postRegistration(registration: OrganDonationRegistrationRequest): OrganDonationRegistrationResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.organDonationConnection )

        val jsonRequest = gson.toJson(registration)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsyncAndGetResult(httpPost)
        httpPost.releaseConnection()

        return gson.fromJson<OrganDonationRegistrationResponse>(response,
         OrganDonationRegistrationResponse::class.java)
    }
}
