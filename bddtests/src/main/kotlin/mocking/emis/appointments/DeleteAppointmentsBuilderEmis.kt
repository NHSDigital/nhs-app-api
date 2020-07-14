package mocking.emis.appointments

import constants.ErrorResponseCodeEmis
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus

class DeleteAppointmentsBuilderEmis (configuration: EmisConfiguration,
                                     patient: Patient, request: CancelAppointmentSlotFacade) :
        EmisMappingBuilder(configuration, "DELETE", "/appointments"), ICancelAppointmentsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)
        requestBuilder.andJsonBody(request, gson = GsonFactory.asPascal)
    }

    override fun respondWithSuccess(): Mapping {
        return respondWithSuccessAny(DeleteAppointmentResponseModel(true))
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_NO_CONTENT) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody("< Non parsable {:< as a XML or JSON", contentType = "application/json")
        }
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("").build()
        }
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {
        return respondWithStandardError(ErrorResponseCodeEmis.NOT_AVAILABLE.toInt(), HttpStatus.SC_NOT_FOUND)
    }

    override fun respondWithWithinAnHour(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithEmisUnknownError()
    }
}
