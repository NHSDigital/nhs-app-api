package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
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

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderEmis {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWithBody(BookAppointmentSlotResponse(true))
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
        val errorResponse = ErrorResponse(-1030)
        return respondWithError(errorResponse, HttpStatus.SC_FORBIDDEN)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {
        val errorResponse = ErrorResponse(-1151)
        return respondWithError(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {
        val errorResponse = ErrorResponse(-1152)
        return respondWithError(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithException(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse, httpStatus: Int): Mapping {
        return respondWithBody(exceptionResponse, httpStatus)
    }

    private fun respondWithError(exceptionResponse: ErrorResponse, httpStatus: Int): Mapping {
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