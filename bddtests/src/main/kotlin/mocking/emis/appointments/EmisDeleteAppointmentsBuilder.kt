package mocking.emis.appointments

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class EmisDeleteAppointmentsBuilder (configuration: EmisConfiguration, patient: Patient, request: CancelAppointmentRequest) :
        EmisMappingBuilder(configuration, "DELETE", "/appointments") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)
        requestBuilder.andJsonBody(request, gson = GsonFactory.asPascal)
    }

    fun respondWithSuccess(response: DeleteAppointmentResponseModel): Mapping {
        return respondWithSuccessAny(response)
    }

    fun respondWithSuccess(jsonBody: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(jsonBody, contentType = "application/json")
        }
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_NO_CONTENT) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}