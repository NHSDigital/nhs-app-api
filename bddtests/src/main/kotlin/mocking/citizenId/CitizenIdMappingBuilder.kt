package mocking.citizenId

import config.Config
import mocking.MappingBuilder
import mocking.citizenId.login.*
import mocking.citizenId.models.TokenRequest
import mocking.defaults.MockDefaults
import models.Patient

open class CitizenIdMappingBuilder(method: String, relativePath: String)
    : MappingBuilder(method, "/citizenid$relativePath") {

    init {

    }

    fun initialLoginRequest(redirectUri: String, clientId: String, customMatcher: String? = null) = InitialLoginRequestBuilder(redirectUri, clientId,customMatcher)

    fun createAccountRequest(redirectUri: String = Config.instance.cidRedirectUri, clientId: String = Config.instance.cidClientId) =
            AccountRegistrationRequestBuilder(redirectUri, clientId)

    fun completeLoginRequest(patient: Patient = MockDefaults.patient,customIdForPatient: String?= null) = CompleteLoginRequestBuilder(patient, customIdForPatient)

    fun tokenRequest(codeVerifier: String, authCode: String? = null, customTokenRequest:TokenRequest?=null) = TokenRequestBuilder(codeVerifier, authCode,customTokenRequest)

    fun signingKeyRequest() = SigningKeysRequestBuilder()
}
