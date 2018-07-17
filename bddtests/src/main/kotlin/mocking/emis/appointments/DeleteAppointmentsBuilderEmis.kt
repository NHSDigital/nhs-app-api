package mocking.emis.appointments

import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.models.Mapping
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus

class DeleteAppointmentsBuilderEmis (configuration: EmisConfiguration, patient: Patient, request: CancelAppointmentSlotFacade) :
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
}