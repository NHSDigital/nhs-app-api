package mocking.emis.appointments

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisGetAppointmentBuilder(
        configuration: EmisConfiguration,
        endUserSessionId: String,
        sessionId: String,
        userPatientLinkToken: String,
        fetchPreviousAppointments: Boolean = false,
        previousAppointmentsFromDate: String? = null)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointments") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, sessionId)
        requestBuilder.andQueryParameter("userPatientLinkToken", userPatientLinkToken)
        requestBuilder.andQueryParameter("fetchPreviousAppointments", fetchPreviousAppointments.toString())

        if (!previousAppointmentsFromDate.isNullOrEmpty())
            requestBuilder.andQueryParameter("previousAppointmentsFromDate", previousAppointmentsFromDate.toString())
    }

    fun respondWithSuccess(model: GetAppointmentsResponseModel): Mapping {
        return respondWithBody(model)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }

    }
}

