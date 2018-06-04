package mocking.emis.allergies

import mocking.emis.*
import mocking.emis.models.*
import mocking.models.Mapping
import org.apache.http.HttpStatus.SC_OK

class EmisAllergiesBuilder(configuration: EmisConfiguration,
                              linkToken: String,
                              apiEndUserSessionId: String,
                              apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/record") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
                .andQueryParameter("itemType", "Allergies", "equalTo")
    }

    fun respondWithSuccess(allergiesResponse: AllergiesResponse): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(allergiesResponse)
                    .build()
        }
    }
}
