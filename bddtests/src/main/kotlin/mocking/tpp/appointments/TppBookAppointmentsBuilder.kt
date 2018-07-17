package mocking.tpp.appointments


import mocking.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.BookAppointmentReply
import mocking.tpp.models.Error
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import java.time.Duration


class TppBookAppointmentsBuilder (patient: Patient, request: BookAppointmentSlotFacade)
    : TppMappingBuilder(method = "POST", relativePath = "/tpp/")
        , IBookAppointmentsBuilder {

    var tppPatient: Patient
    var uuid = "12E75A70-6149-48A9-871C-A3152EEEE90E"
    var errorText = "There was a problem booking the appointment"

    init {
        tppPatient = patient;
        requestBuilder.andHeader(HEADER_TYPE, "BookAppointment")
        requestBuilder.andBodyMatchingXpath("//BookAppointment[" +
                "@patientId='${patient.patientId}' and " +
                "@sessionId='${request.slotId}']")
    }

    override fun withDelay(delayMilliseconds: Duration): TppBookAppointmentsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this;
    }

    override fun respondWithSuccess(): Mapping {

        return respondWith(
                BookAppointmentReply(tppPatient.patientId,
                        message = "Remember to bring your medication!",
                        uuid = uuid
                ))
    }

    override fun respondWithUnavailableException(): Mapping {

        var error = Error("1103", errorText, uuid)
        return respondWith(error)
    }

    override fun respondWithConflictException(): Mapping {

        var error = Error("1103", errorText, uuid)
        return respondWith(error)
    }

    override fun respondWithUnknownException(): Mapping {

        var error = Error("0000", errorText, uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {

        var error = Error("6", errorText, uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        var error = Error("1103", errorText, uuid)
        return respondWith(error)
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {

        var error = Error("5", errorText, uuid)
        return respondWith(error)
    }
}