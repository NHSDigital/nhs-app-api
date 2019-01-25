package worker

import com.google.gson.Gson
import config.Config
import org.apache.http.HttpResponse
import org.apache.http.client.methods.HttpGet
import org.apache.http.client.methods.HttpPost
import org.apache.http.client.utils.URIBuilder
import org.apache.http.entity.StringEntity
import worker.models.appointments.AppointmentBookRequest
import worker.models.appointments.AppointmentSlotsResponse
import worker.models.appointments.CancelAppointmentRequest
import worker.models.appointments.MyAppointmentsResponse
import javax.servlet.http.Cookie

class WorkerClientAppointments(val config: Config, val sender: WorkerClientSender, val gson: Gson){

    fun setCsrfToken(token: String):WorkerClientAppointments {
        sender.setCsrfToken(token)
        return this
    }

    fun getMyAppointments(fromDate: String, includePastAppointments: Boolean = false): MyAppointmentsResponse {
        val uriBuilder = URIBuilder(config.pfsBackendUrl)
                .setPath(WorkerPaths.myAppointments)
                .addParameter("pastAppointmentsFromDate", fromDate)
                .addParameter("includePastAppointments", includePastAppointments.toString())

        val httpGet = HttpGet(uriBuilder.build())

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        println(result)

        return gson.fromJson<MyAppointmentsResponse>(result, MyAppointmentsResponse::class.java)
    }

    fun getAppointmentSlots(fromDate: String? = null,
                            toDate: String? = null,
                            sessionCookie: Cookie? = null): AppointmentSlotsResponse {
        val uriBuilder = createUriBuilderForAppointmentSlots(fromDate, toDate)
        val httpGet = HttpGet(uriBuilder.build())

        if (sessionCookie != null) httpGet.addHeader("Cookie", sessionCookie.value.split(";")[0])

        val result = sender.sendAsyncAndGetResult(httpGet)
        httpGet.releaseConnection()
        println(result)

        return gson.fromJson<AppointmentSlotsResponse>(result, AppointmentSlotsResponse::class.java)
    }

    private fun createUriBuilderForAppointmentSlots(fromDate: String?, toDate: String?): URIBuilder {
        val uriBuilder = URIBuilder(config.pfsBackendUrl + WorkerPaths.appointmentSlots)
        if (!fromDate.isNullOrEmpty()) {
            uriBuilder.setParameter("fromDate", fromDate)
        }
        if (!toDate.isNullOrEmpty()) {
            uriBuilder.setParameter("toDate", toDate)
        }
        return uriBuilder
    }

    fun deleteAppointment(requestBody: CancelAppointmentRequest): HttpResponse {
        val httpDelete = WorkerClient.HttpDeleteWithBody(config.pfsBackendUrl + WorkerPaths.myAppointments)
        val entity = StringEntity(gson.toJson(requestBody), "UTF-8")
        entity.setContentType("application/json")
        httpDelete.entity = entity

        val response = sender.sendAsync(httpDelete)
        httpDelete.releaseConnection()
        return response!!
    }

    fun postAppointment(appointmentBookRequest: AppointmentBookRequest, sessionCookie: Cookie? = null): HttpResponse {
        val httpPost = HttpPost(config.pfsBackendUrl + WorkerPaths.myAppointments)

        if (sessionCookie != null) httpPost.addHeader("Cookie", sessionCookie.value.split(";")[0])
        val entity = StringEntity(gson.toJson(appointmentBookRequest), "UTF-8")
        entity.setContentType("application/json")
        httpPost.entity = entity

        val response = sender.sendAsync(httpPost, null)
        httpPost.releaseConnection()
        return response!!
    }
}