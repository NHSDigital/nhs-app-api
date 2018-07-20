package mocking.emis.appointments

import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.gpServiceBuilderInterfaces.IMyAppointmentsBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class EmisViewAppointmentBuilder(configuration: EmisConfiguration?, patient: Patient)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {
    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)

    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }

}