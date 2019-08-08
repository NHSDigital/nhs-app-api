package mocking.tpp.appointments


import constants.ErrorResponseCodeTpp
import mocking.JSonXmlConverter
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.BookAppointmentReply
import mocking.tpp.models.Error
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.Duration


class BookAppointmentsBuilderTpp(patient: Patient, request: BookAppointmentSlotFacade)
    : TppMappingBuilder()
        , IBookAppointmentsBuilder {

    private val tppPatient: Patient = patient
    private val errorText = "There was a problem booking the appointment"
    private val appointmentLimitErrorText = "The appointment has not been booked because you " +
            "have reached the limit of pending appointments"
    private val defaultAppointmentReply = BookAppointmentReply(tppPatient.patientId,
            onlineUserId = tppPatient.patientId,
            message = "Remember to bring your medication!",
            uuid = TppConfig.uuid)

    init {
        requestBuilder.andHeader(HEADER_TYPE, "BookAppointment")
        requestBuilder.andBodyMatchingXpath("//BookAppointment[" +
                "@patientId='${patient.patientId}' and " +
                "@sessionId='${request.slotId}' and " +
                "@notes='${request.bookingReason}']")
    }

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderTpp {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWith(defaultAppointmentReply)
    }

    override fun respondWithCorrupted(): Mapping {
        val response = JSonXmlConverter.toXML(defaultAppointmentReply)
        return respondWithCorruptedContent(response)
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
        return respondWithTppUnknownError(errorText)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {

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

    override fun respondWithExceptionWhenRequiredFieldMissing(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun respondWithExceptionWhenBeforePracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun respondWithExceptionWhenAfterPracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }
}