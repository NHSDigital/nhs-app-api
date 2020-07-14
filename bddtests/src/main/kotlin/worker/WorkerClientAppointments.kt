package worker

import com.google.gson.Gson
import config.Config
import mocking.emis.models.SlotTypeStatus
import org.apache.http.HttpResponse
import org.apache.http.client.utils.URIBuilder
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

    fun getMyAppointments(
            patientId: String,
            fromDate: String,
            includePastAppointments: Boolean = false): MyAppointmentsResponse? {
        val uriBuilder = URIBuilder(config.apiBackendUrl)
                .setPath(WorkerPaths.myAppointments)
                .addParameter("pastAppointmentsFromDate", fromDate)
                .addParameter("includePastAppointments", includePastAppointments.toString())

        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
        .setHeader(WorkerHeaders.PatientId, patientId)

        val response =  httpGet.sendAndGetResult(sender, gson,  MyAppointmentsResponse::class.java)
        if(response!=null) {
            response.upcomingAppointments.forEach { appt ->
                appt.channel = appt.channel ?: SlotTypeStatus.Unknown
                appt.disableCancellation = appt.disableCancellation ?: "false"
            }
            response.pastAppointments.forEach { appt ->
                appt.channel = appt.channel ?: SlotTypeStatus.Unknown
            }
        }

        return response
    }

    fun getAppointmentSlots(patientId: String?,
                            fromDate: String? = null,
                            toDate: String? = null,
                            sessionCookie: Cookie? = null): AppointmentSlotsResponse? {
        val uriBuilder = createUriBuilderForAppointmentSlots(fromDate, toDate)
        val httpGet = RequestBuilder.get(uriBuilder.build().toString())
        .setHeader(WorkerHeaders.PatientId, patientId)
                .addCookieIfNotNull(sessionCookie)
        return httpGet.sendAndGetResult(sender, gson, AppointmentSlotsResponse::class.java)
    }

    private fun createUriBuilderForAppointmentSlots(fromDate: String?, toDate: String?): URIBuilder {
        val uriBuilder = URIBuilder(config.apiBackendUrl + WorkerPaths.appointmentSlots)
        if (!fromDate.isNullOrEmpty()) {
            uriBuilder.setParameter("fromDate", fromDate)
        }
        if (!toDate.isNullOrEmpty()) {
            uriBuilder.setParameter("toDate", toDate)
        }
        return uriBuilder
    }

    fun deleteAppointment(patientId: String, requestBody: CancelAppointmentRequest): HttpResponse? {
        val httpDelete = RequestBuilder.delete(config.apiBackendUrl + WorkerPaths.myAppointments)
                .addBody(requestBody, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
        return httpDelete.send(sender)
    }

    fun postAppointment(
            patientId: String?,
            appointmentBookRequest: AppointmentBookRequest,
            sessionCookie: Cookie? = null): HttpResponse? {
        val httpPost = RequestBuilder.post(config.apiBackendUrl + WorkerPaths.myAppointments)
                .addBody(appointmentBookRequest, gson)
                .setHeader(WorkerHeaders.PatientId, patientId)
                .addCookieIfNotNull(sessionCookie)
        return httpPost.send(sender)
    }
}
