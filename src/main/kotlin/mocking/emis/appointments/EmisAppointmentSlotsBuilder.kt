package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.emis.*
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisAppointmentSlotsBuilder(configuration: EmisConfiguration,
                                  apiEndUserSessionId: String,
                                  apiSessionId: String,
                                  fromDateTime: String?,
                                  toDateTime: String?,
                                  sessionId: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (fromDateTime != null) requestBuilder.andQueryParameter(name = "fromDateTime", value = fromDateTime)
        if (toDateTime != null) requestBuilder.andQueryParameter(name = "toDateTime", value = toDateTime)
        if (sessionId != null) requestBuilder.andQueryParameter(name = "sessionId", value = sessionId)
    }

    fun respondWithSuccess(slots: ArrayList<AppointmentSlot>, sessionId: Int?, sessionDate: String?): Mapping {
        val session = AppointmentSession(sessionDate, sessionId, slots)
        val model = GetAppointmentSlotsResponseModel(arrayListOf(session))
        return respondWithSuccessAny(model)
    }

    fun respondWithSuccess(response: GetAppointmentSlotsResponseModel): Mapping {
        return respondWithSuccessAny(response)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}