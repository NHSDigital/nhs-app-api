package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.models.Mapping
import org.apache.http.HttpStatus
import worker.models.appointments.BookAppointmentSlotRequest
import worker.models.appointments.BookAppointmentSlotResponse

class EmisBookAppointmentsBuilder (configuration: EmisConfiguration,
                                   apiEndUserSessionId: String,
                                   apiSessionId: String,
                                   request: BookAppointmentSlotRequest)
    : EmisMappingBuilder(configuration, method = "POST", relativePath = "/appointments") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andJsonBody(request, "")

    }

    fun respondWithSuccess(): Mapping {
        return respondWithBody(BookAppointmentSlotResponse(true))
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}