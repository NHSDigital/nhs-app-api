package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
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

    fun respondWithUnavailableException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Unavailable Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_SERVICE_UNAVAILABLE)
    }

    fun respondWithConflictException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Conflict Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_CONFLICT)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithException(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse, httpStatus: Int): Mapping {
        return respondWithBody(exceptionResponse, httpStatus)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}