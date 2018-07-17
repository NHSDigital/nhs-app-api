package mocking.tpp.appointments


import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.data.TppConfig
import mocking.tpp.models.BookAppointmentReply
import mocking.tpp.models.Error
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.Duration


class BookAppointmentsBuilderTpp (patient: Patient, request: BookAppointmentSlotFacade)
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

    override fun respondWithUnavailableException(): Mapping {

      return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE){
          andXmlBody("").build()
      }
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