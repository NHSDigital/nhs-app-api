package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.signingKeys.SuceededResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class SigningKeysRequestBuilder()
    : CitizenIdMappingBuilder("GET", "/.well-known/jwks.json") {

    fun respondWithSuccess(
            signingKeys: SuceededResponse): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(signingKeys)
        }
    }
}
