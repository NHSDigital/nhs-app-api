package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.protocol.HttpContext
import worker.models.courses.CoursesListResponse
import worker.models.prescriptions.PrescriptionsListResponse
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.net.URLEncoder

class WorkerClientPrescriptions(val config: Config, val sender: WorkerClientSender, val gson: Gson) {

    fun getPrescriptionsConnection(
            patientId: String?,
            fromDate: String?,
            context: HttpContext? = null): PrescriptionsListResponse? {
        var queryString = ""
        if (fromDate != null) queryString = "?FromDate=" + URLEncoder.encode(fromDate, "UTF-8")
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.getPrescriptionsConnection + queryString)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, PrescriptionsListResponse::class.java, context)
    }

    fun postPrescriptionsConnection(patientId: String, requestBody: PrescriptionSubmissionRequest?): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.postPrescriptionsConnection)
                .addBody(requestBody, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpPost.send(sender)
    }

    fun getCoursesConnection(patientId: String): CoursesListResponse? {
        val httpGet = RequestBuilder.get(config.apiBackendUrl + WorkerPaths.getCoursesConnection)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpGet.sendAndGetResult(sender, gson, CoursesListResponse::class.java)
    }
}