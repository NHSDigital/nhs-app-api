package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.MockDefaults
import mocking.models.Mapping
import models.Patient

class CompleteLoginRequestBuilder(val patient: Patient = MockDefaults.patient)
    : CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/complete-login") {

    init {
        requestBuilder.andQueryParameter("mock_patient", patient.hashCode().toString(), "equalTo")
    }

    fun respondWithRedirectResponse(): Mapping {
        return redirectTo("{{request.query.redirect_uri}}?state={{request.query.state}}&code=${patient.cidUserSession.authCode!!}")
    }
}