package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.userInfo.SucceededResponse
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class UserInfoRequestBuilder(bearerToken: String)
    : CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/userinfo") {

    init {
        requestBuilder.andHeader("Authorization", bearerToken)
    }

    fun respondWithSuccess(patient: Patient): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(SucceededResponse(patient))
        }
    }
}