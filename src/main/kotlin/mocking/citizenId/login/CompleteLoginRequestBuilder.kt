package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping

class CompleteLoginRequestBuilder : CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/complete-login") {

    fun respondWithRedirectResponse(code: String): Mapping {
        return redirectTo("{{request.query.redirect_uri}}?state={{request.query.state}}&code=$code")
    }

    fun respondWithRedirectResponse(): Mapping {
        return respondWithRedirectResponse("uss.K8g8HL1MGeInMeGZtkzPypDM6GXjTExK1EP4pVMMTlk.e25ea653-e480-41c4-aab2-ae17dae0eb00.8d4c0a21-6483-4a52-9d47-6bcd737c634e")
    }
}