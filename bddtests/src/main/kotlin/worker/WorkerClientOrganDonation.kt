package worker

import com.google.gson.Gson
import config.Config
import mocking.organDonation.models.OrganDonationRegistrationRequest
import mocking.organDonation.models.OrganDonationWithdrawRequest
import mocking.organDonation.models.ReferenceDataResponse
import org.apache.http.HttpResponse
import worker.models.organdonation.OrganDonationRegistrationResponse
import worker.models.organdonation.OrganDonationSearchResponse

class WorkerClientOrganDonation(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    private val path = config.apiBackendUrl + WorkerPaths.organDonationConnection

    fun getOrganDonationConnection(patientId: String): OrganDonationSearchResponse? {
        val httpGet = RequestBuilder.get(path)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, OrganDonationSearchResponse::class.java)
    }

    fun getOrganDonationReferenceData(): ReferenceDataResponse? {
        val httpGet = RequestBuilder.get(path + "/ReferenceData")
        return httpGet.sendAndGetResult(sender, gson, ReferenceDataResponse::class.java)
    }

    fun postRegistration(registration: OrganDonationRegistrationRequest, patientId: String)
            : OrganDonationRegistrationResponse? {
        val httpPost = RequestBuilder.post(path)
                .addBody(registration, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpPost.sendAndGetResult(sender, gson, OrganDonationRegistrationResponse::class.java)
    }

    fun putRegistration(registration: OrganDonationRegistrationRequest, patientId: String)
            : OrganDonationRegistrationResponse? {
        val httpPut = RequestBuilder.put(path)
                .addBody(registration, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpPut.sendAndGetResult(sender, gson, OrganDonationRegistrationResponse::class.java)
    }

    fun deleteRegistration(withdrawalRequestBody: OrganDonationWithdrawRequest, patientId: String): HttpResponse? {
        val httpDelete = RequestBuilder.delete(config.apiBackendUrl + WorkerPaths.organDonationConnection)
                .addBody(withdrawalRequestBody, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpDelete.send(sender)
    }
}
