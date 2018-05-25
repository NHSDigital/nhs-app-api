package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping

class CompleteLoginRequestBuilder : CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/complete-login") {

    fun respondWithRedirectResponse(code: String): Mapping {
        return redirectTo("{{request.query.redirect_uri}}?state={{request.query.state}}&code=$code")
    }
}