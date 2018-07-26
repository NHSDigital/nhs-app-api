package mocking.emis.appointments

import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class GetAppointmentBuilderEmis(configuration: EmisConfiguration?, patient: Patient, fetchPreviousAppointments: Boolean = false)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)
                .andQueryParameter("userPatientLinkToken", patient.userPatientLinkToken)
                .andQueryParameter("fetchPreviousAppointments", fetchPreviousAppointments.toString())
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorWhenGPDisabledAppointmentsService()
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }
}
