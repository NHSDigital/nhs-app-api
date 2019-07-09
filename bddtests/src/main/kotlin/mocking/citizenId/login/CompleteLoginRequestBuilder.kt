package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient

class CompleteLoginRequestBuilder(val patient: Patient, customId: String? = null)
    : CitizenIdMappingBuilder("GET",
        "/complete-login") {

    init {
        val customIdForLogin = customId ?: patient.hashCode().toString()
        requestBuilder.andQueryParameter("mock_patient",customIdForLogin , "equalTo")
    }

    fun respondWithRedirectResponse(): Mapping {
        return redirectTo(
                "{{request.query.redirect_uri}}?state={{request.query.state}}&code=" +
                "${patient.cidUserSession.authCode!!}")
    }
}