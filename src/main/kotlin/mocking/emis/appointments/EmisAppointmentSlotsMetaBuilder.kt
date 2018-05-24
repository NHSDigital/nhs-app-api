package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.MockingClient
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisAppointmentSlotsMetaBuilder(configuration: EmisConfiguration,
                                      apiEndUserSessionId: String,
                                      apiSessionId: String,
                                      sessionStartDate: String? = null,
                                      sessionEndDate: String? = null,
                                      userPatientLinkToken: String? = null)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots/meta") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (sessionStartDate != null) requestBuilder.andQueryParameter("sessionStartDate", sessionStartDate)
        if (sessionEndDate != null) requestBuilder.andQueryParameter("sessionEndDate", sessionEndDate)
        if (userPatientLinkToken != null) requestBuilder.andQueryParameter("userPatientLinkToken", userPatientLinkToken)
    }

    fun respondWithSuccess(model: GetAppointmentSlotsMetaResponseModel): Mapping {
        return respondWithSuccessAny(model)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}