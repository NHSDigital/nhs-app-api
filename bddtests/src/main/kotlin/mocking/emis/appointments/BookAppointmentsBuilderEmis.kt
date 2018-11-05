package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import constants.ErrorResponseCodeEmis
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import org.apache.http.HttpStatus
import worker.models.appointments.BookAppointmentSlotResponse
import java.time.Duration

class BookAppointmentsBuilderEmis(configuration: EmisConfiguration,
                                  apiEndUserSessionId: String,
                                  apiSessionId: String,
                                  request: BookAppointmentSlotFacade)
    : EmisMappingBuilder(configuration, method = "POST", relativePath = "/appointments")
        , IBookAppointmentsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andJsonBody(request, gson = GsonFactory.asPascal)

    }

    fun respondWithBookingLimitExceptionForOldEMIS(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.INTERNAL_ERROR,
                "Maximum booked appointments is limited to 0 by the practice")
        return respondWithBody(exceptionResponse, HttpStatus.SC_SERVICE_UNAVAILABLE)
    }

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderEmis {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWithBody(BookAppointmentSlotResponse(true))
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody("< Non parsable {:< as a XML or JSON", contentType = "application/json")
        }
    }

    override fun respondWithUnavailableException(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.EXCEPTION,
                "Unavailable Exception")
        return respondWithBody(exceptionResponse, HttpStatus.SC_SERVICE_UNAVAILABLE)
    }

    override fun respondWithConflictException(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.EXCEPTION,
                "Conflict Exception")
        return respondWithBody(exceptionResponse, HttpStatus.SC_CONFLICT)
    }

    override fun respondWithBookingLimitException(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.ONLINE_USER_MAX_APPOINTMENT_BOOKED_COUNT,
                "Maximum booked appointments is limited to 0 by the practice")
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.EXCEPTION,
                "Unknown Exception")
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.NOT_AVAILABLE.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.REQUESTED_APPOINTMENT_SLOT_IN_PAST.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_BAD_REQUEST)
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
