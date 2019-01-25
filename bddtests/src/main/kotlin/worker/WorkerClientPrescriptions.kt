package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.entity.StringEntity
import org.apache.http.protocol.HttpContext
import worker.models.courses.CoursesListResponse
import worker.models.prescriptions.PrescriptionsListResponse
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.net.URLEncoder

class WorkerClientPrescriptions(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun getPrescriptionsConnection(fromDate: String?, context: HttpContext? = null): PrescriptionsListResponse {
        var queryString = ""
        if (fromDate != null) queryString = "?FromDate=" + URLEncoder.encode(fromDate, "UTF-8")
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getPrescriptionsConnection + queryString)
        val result = sender.sendAsyncAndGetResult(httpGet, context)
        httpGet.releaseConnection()

        return gson.fromJson<PrescriptionsListResponse>(result, PrescriptionsListResponse::class.java)
    }

    fun postPrescriptionsConnection(requestBody: PrescriptionSubmissionRequest?): HttpResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.postPrescriptionsConnection)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsync(httpPost)
        httpPost.releaseConnection()
        return response!!
    }

    fun getCoursesConnection(): CoursesListResponse {
        val httpGet = HttpGet(config.pfsBackendUrl + WorkerPaths.getCoursesConnection)

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()

        return gson.fromJson<CoursesListResponse>(result, CoursesListResponse::class.java)
    }
}