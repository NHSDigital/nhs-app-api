package mocking.citizenId

import mocking.MappingBuilder
import mocking.citizenId.login.CompleteLoginRequestBuilder
import mocking.citizenId.login.InitialLoginRequestBuilder
import mocking.citizenId.login.TokenRequestBuilder
import mocking.citizenId.login.UserInfoRequestBuilder

open class CitizenIdMappingBuilder(method: String, relativePath: String)
    : MappingBuilder(method, "/citizenid$relativePath") {

    init {
        // no generic additions to the request
    }

    fun initialLoginRequest(redirectUri: String, clientId: String) = InitialLoginRequestBuilder(redirectUri, clientId)

    fun completeLoginRequest() = CompleteLoginRequestBuilder()

    fun tokenRequest(codeVerifier: String, authCode: String? = null) = TokenRequestBuilder(codeVerifier, authCode)

    fun userInfoRequest(bearerToken: String) = UserInfoRequestBuilder(bearerToken)
}