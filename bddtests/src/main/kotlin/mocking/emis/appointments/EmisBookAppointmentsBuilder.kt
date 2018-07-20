package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.IBookAppointmentsBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import org.apache.http.HttpStatus
import worker.models.appointments.BookAppointmentSlotResponse
import java.time.Duration

class EmisBookAppointmentsBuilder (configuration: EmisConfiguration,
                                   apiEndUserSessionId: String,
                                   apiSessionId: String,
                                   request: BookAppointmentSlotFacade)
    : EmisMappingBuilder(configuration, method = "POST", relativePath = "/appointments")
        , IBookAppointmentsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andJsonBody(request, "")

    }
    var delayMillisecs = 0

    override fun  withDelay(delayMilliseconds : Duration):EmisBookAppointmentsBuilder{
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWithBody(BookAppointmentSlotResponse(true))
    }

    fun respondWithSuccess(jsonBody: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(jsonBody, contentType = "application/json")
        }
    }

    override fun respondWithUnavailableException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Unavailable Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_SERVICE_UNAVAILABLE)
    }

    override fun respondWithConflictException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Conflict Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_CONFLICT)
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Unknown Exception")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(-1030,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'AppointmentBooking' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, RecordViewer, RepeatPrescribing, SharedRecordAuditView'. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {
        val exceptionResponse = ExceptionResponse(-1002,
                "Appointment not found")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {
        val exceptionResponse = ExceptionResponse(-1002,
                "Appointment cannot be booked in the past")
        return respondWithException(exceptionResponse)
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
                    .andDelay(delayMillisecs)
        }
    }
}