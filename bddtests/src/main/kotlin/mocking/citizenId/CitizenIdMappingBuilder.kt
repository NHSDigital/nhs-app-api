package mocking.citizenId

import config.Config
import mocking.MappingBuilder
import mocking.citizenId.login.AccountRegistrationRequestBuilder
import mocking.citizenId.login.CompleteLoginRequestBuilder
import mocking.citizenId.login.InitialLoginRequestBuilder
import mocking.citizenId.login.UserInfoRequestBuilder
import mocking.citizenId.login.SigningKeysRequestBuilder
import mocking.citizenId.login.TokenRequestBuilder
import mocking.citizenId.models.TokenRequest
import mocking.defaults.EmisMockDefaults
import models.Patient

open class CitizenIdMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, "/citizenid$relativePath") {

    fun initialLoginRequest(patient: Patient,
                            redirectUri: String,
                            clientId: String,
                            customMatcher: String? = null) = InitialLoginRequestBuilder(
            patient, redirectUri, clientId, customMatcher)

    fun createAccountRequest(redirectUri: String = Config.instance.cidRedirectUri,
                             clientId: String = Config.instance.cidClientId) =
            AccountRegistrationRequestBuilder(redirectUri, clientId)

    fun completeLoginRequest(patient: Patient = EmisMockDefaults.patientEmis,
                             customIdForPatient: String? = null) = CompleteLoginRequestBuilder(
            patient, customIdForPatient)

    fun tokenRequest(codeVerifier: String, authCode: String? = null,
                     customTokenRequest: TokenRequest? = null) = TokenRequestBuilder(
            codeVerifier, authCode, customTokenRequest)

    fun userInfoRequest(accessToken: String) = UserInfoRequestBuilder(accessToken)

    fun signingKeyRequest() = SigningKeysRequestBuilder()
}
