package worker

import com.google.gson.Gson
import config.Config
import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.OrganDonationWithdrawRequest
import mocking.organDonation.models.ReferenceDataResponse
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.methods.HttpPut
import org.apache.http.entity.StringEntity
import worker.models.organdonation.OrganDonationRegistrationResponse
import worker.models.organdonation.OrganDonationSearchResponse
import worker.WorkerClient.HttpDeleteWithBody

class WorkerClientOrganDonation(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getOrganDonationConnection(patientId: String): OrganDonationSearchResponse {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.organDonationConnection)
        httpGet.setHeader(WorkerHeaders.PatientId, patientId)

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, OrganDonationSearchResponse::class.java)
    }

    fun getOrganDonationReferenceData(): ReferenceDataResponse {
        val httpGet = HttpGet(config.apiBackendUrl + WorkerPaths.organDonationConnection + "/ReferenceData")

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson(result, ReferenceDataResponse::class.java)
    }

    fun postRegistration(registration: OrganDonationRegistrationRequest, patientId: String)
            : OrganDonationRegistrationResponse {
        val httpPost = HttpPost(config.apiBackendUrl + WorkerPaths.organDonationConnection )
        httpPost.setHeader(WorkerHeaders.PatientId, patientId)

        val jsonRequest = gson.toJson(registration)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsyncAndGetResult(httpPost)
        httpPost.releaseConnection()

        return gson.fromJson<OrganDonationRegistrationResponse>(response,
         OrganDonationRegistrationResponse::class.java)
    }

    fun putRegistration(registration: OrganDonationRegistrationRequest, patientId: String)
            : OrganDonationRegistrationResponse {
        val httpPut = HttpPut(config.apiBackendUrl + WorkerPaths.organDonationConnection)
        httpPut.setHeader(WorkerHeaders.PatientId, patientId)

        val jsonRequest = gson.toJson(registration)
        val entity = StringEntity(jsonRequest, "UTF-8")
        entity.setContentType("application/json")
        httpPut.entity = entity

        val response = sender.sendAsyncAndGetResult(httpPut)
        httpPut.releaseConnection()

        return gson.fromJson<OrganDonationRegistrationResponse>(response,
                OrganDonationRegistrationResponse::class.java)
    }

    fun deleteRegistration(withdrawalRequestBody: OrganDonationWithdrawRequest, patientId: String): HttpResponse {
        val httpDelete = HttpDeleteWithBody(config.apiBackendUrl + WorkerPaths.organDonationConnection)
        httpDelete.setHeader(WorkerHeaders.PatientId, patientId)
        val entity = StringEntity(gson.toJson(withdrawalRequestBody), "UTF-8")
        entity.setContentType("application/json")
        httpDelete.entity = entity

        val response = sender.sendAsync(httpDelete)
        httpDelete.releaseConnection()
        return response!!
    }
}
