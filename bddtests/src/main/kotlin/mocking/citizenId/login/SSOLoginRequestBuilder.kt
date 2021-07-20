package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient

class SSOLoginRequestBuilder(
        val patient: Patient,
        redirectUri: String,
        clientId: String,
        matcherToUse: String? = null)
    : CitizenIdMappingBuilder("GET", "/authorize") {

    init {
        val matcherString = matcherToUse ?: "equalTo"
        requestBuilder
                .andQueryParameter("redirect_uri", redirectUri, matcherString)
                .andQueryParameter("client_id", clientId)
                .andQueryParameter("response_type", "code")
                .andQueryParameter("vtr",
                        "[\"P5.Cp.Cd\", \"P5.Cp.Ck\", \"P5.Cm\", \"P9.Cp.Cd\", \"P9.Cp.Ck\", \"P9.Cm\"]")
                .andQueryParameter(
                        "scope",
                        "openid profile email nhs_app_credentials gp_registration_details profile_extended")
    }

    fun respondWithRedirectURI(): Mapping {
        return redirectTo("{{request.query.redirect_uri}}?state={{request.query.state}}&code=" +
                    patient.authCode)
    }
}
