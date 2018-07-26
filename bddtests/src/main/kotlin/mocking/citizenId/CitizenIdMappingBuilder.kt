package mocking.citizenId

import config.Config
import mocking.MappingBuilder
import mocking.citizenId.login.CompleteLoginRequestBuilder
import mocking.citizenId.login.InitialLoginRequestBuilder
import mocking.citizenId.login.AccountRegistrationRequestBuilder
import mocking.citizenId.login.TokenRequestBuilder
import mocking.citizenId.login.UserInfoRequestBuilder
import mocking.defaults.MockDefaults
import models.Patient

open class CitizenIdMappingBuilder(method: String, relativePath: String)
    : MappingBuilder(method, "/citizenid$relativePath") {

    init {
        // no generic additions to the request
    }

    fun initialLoginRequest(redirectUri: String, clientId: String) = InitialLoginRequestBuilder(redirectUri, clientId)

    fun createAccountRequest(redirectUri: String = Config.instance.cidRedirectUri, clientId: String = Config.instance.cidClientId) =
            AccountRegistrationRequestBuilder(redirectUri, clientId)

    fun completeLoginRequest(patient: Patient = MockDefaults.patient) = CompleteLoginRequestBuilder(patient)

    fun tokenRequest(codeVerifier: String, authCode: String? = null) = TokenRequestBuilder(codeVerifier, authCode)

    fun userInfoRequest(bearerToken: String) = UserInfoRequestBuilder(bearerToken)
}