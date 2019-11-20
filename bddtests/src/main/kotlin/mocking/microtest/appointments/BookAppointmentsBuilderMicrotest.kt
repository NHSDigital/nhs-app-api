package mocking.microtest.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.BookAppointmentSlotFacade
import org.apache.http.HttpStatus
import java.time.Duration

class BookAppointmentsBuilderMicrotest(request: BookAppointmentSlotFacade)
    : MicrotestMappingBuilder(method = "POST", relativePath = "/appointments")
        , IBookAppointmentsBuilder {


    init {
        val requestBody = PostAppointmentRequestModel (
                slotId = request.slotId.toString(),
                bookingReason = request.bookingReason.toString(),
                telephoneNumber = request.telephoneNumber)
        requestBuilder
                .andJsonBody(requestBody)
        }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
                    .andDelay(delayMillisecs)
        }
    }

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderMicrotest {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWithBody("\"Appointment successfully created.\"")
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("< Non parsable {:< as a XML or JSON")
    }

    override fun respondWithConflictException(): Mapping {
        return respondWithConflictError()
    }

    override fun respondWithBookingLimitException(): Mapping {
        TODO("not implemented")
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithUnknownExceptionError()
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondWithForbiddenError()
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {
        return respondWithConflictError()
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {
        return respondWithConflictError()
    }

    override fun respondWithExceptionWhenBeforePracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun respondWithExceptionWhenAfterPracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }
}
