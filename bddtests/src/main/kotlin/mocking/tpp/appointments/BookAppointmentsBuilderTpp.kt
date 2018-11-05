package mocking.tpp.appointments


import constants.ErrorResponseCodeTpp
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.BookAppointmentReply
import mocking.tpp.models.Error
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.Duration


class BookAppointmentsBuilderTpp(patient: Patient, request: BookAppointmentSlotFacade)
    : TppMappingBuilder()
        , IBookAppointmentsBuilder {

    private val tppPatient: Patient = patient
    private val errorText = "There was a problem booking the appointment"
    private val appointmentLimitErrorText = "The appointment has not been booked because you " +
            "have reached the limit of pending appointments"

    init {
        requestBuilder.andHeader(HEADER_TYPE, "BookAppointment")
        requestBuilder.andBodyMatchingXpath("//BookAppointment[" +
                "@patientId='${patient.patientId}' and " +
                "@sessionId='${request.slotId}']")
    }

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderTpp {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWith(
                BookAppointmentReply(tppPatient.patientId,
                        onlineUserId = tppPatient.patientId,
                        message = "Remember to bring your medication!",
                        uuid = TppConfig.uuid
                ))
    }

    override fun respondWithCorrupted(): Mapping {
        val mapping = respondWithSuccess()
        return respondWith(HttpStatus.SC_OK) {
            andBody(mapping.response!!.body!!.replace(">", "|").replace("}", "|"), contentType = "text/xml")
        }
    }

    override fun respondWithUnavailableException(): Mapping {

        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("").build()
        }
    }

    override fun respondWithConflictException(): Mapping {

        val error = Error(ErrorResponseCodeTpp.SLOT_ALREADY_BOOKED, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithBookingLimitException(): Mapping {
        val error = Error(ErrorResponseCodeTpp.APPOINTMENT_LIMIT_REACHED, appointmentLimitErrorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithUnknownException(): Mapping {

        val error = Error(ErrorResponseCodeTpp.UNKNOWN_ERROR, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {

        val error = Error(ErrorResponseCodeTpp.NO_ACCESS, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        val error = Error(ErrorResponseCodeTpp.SLOT_NOT_FOUND, errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {

        val error = Error(ErrorResponseCodeTpp.START_DATE_IN_PAST, errorText, TppConfig.uuid)
        return respondWith(error)
    }
}