package mocking.emis.me

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.QUERY_PARAM_USER_PATIENT_LINK_TOKEN
import mocking.emis.models.MeSettingsResponseModel
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisMeSettingsBuilder(configuration: EmisConfiguration,
                            endUserSessionId: String,
                            sessionId: String,
                            userPatientLinkToken: String):
    EmisMappingBuilder(configuration, "GET", "/me/settings") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, sessionId)
                .andQueryParameter(QUERY_PARAM_USER_PATIENT_LINK_TOKEN, userPatientLinkToken)
    }

    fun respondWithSuccess(response: MeSettingsResponseModel): Mapping {
        return respondWith(HttpStatus.SC_OK){
            andJsonBody(response, GsonFactory.asPascal)
        }
    }
}
