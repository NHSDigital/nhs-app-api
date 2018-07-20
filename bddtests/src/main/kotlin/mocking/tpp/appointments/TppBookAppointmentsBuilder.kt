package mocking.tpp.appointments


import mocking.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.data.TppConfig
import mocking.tpp.models.BookAppointmentReply
import mocking.tpp.models.Error
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.Duration


class TppBookAppointmentsBuilder (patient: Patient, request: BookAppointmentSlotFacade)
    : TppMappingBuilder()
        , IBookAppointmentsBuilder {

    var tppPatient: Patient = patient
    var errorText = "There was a problem booking the appointment"

    init {
        requestBuilder.andHeader(HEADER_TYPE, "BookAppointment")
        requestBuilder.andBodyMatchingXpath("//BookAppointment[" +
                "@patientId='${patient.patientId}' and " +
                "@sessionId='${request.slotId}']")
    }

    override fun withDelay(delayMilliseconds: Duration): TppBookAppointmentsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {

        return respondWith(
                BookAppointmentReply(tppPatient.patientId,
                        message = "Remember to bring your medication!",
                        uuid = TppConfig.uuid
                ))
    }

    override fun respondWithUnavailableException(): Mapping {

        var error = Error("1103", errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithConflictException(): Mapping {

        var error = Error("1103", errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithUnknownException(): Mapping {

        var error = Error("0000", errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {

        var error = Error("6", errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        var error = Error("1103", errorText, TppConfig.uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {

        var error = Error("5", errorText, TppConfig.uuid)
        return respondWith(error)
    }
}