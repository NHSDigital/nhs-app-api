package mocking.citizenId

import config.Config
import mocking.MappingBuilder
import mocking.citizenId.login.AccountRegistrationRequestBuilder
import mocking.citizenId.login.CompleteLoginRequestBuilder
import mocking.citizenId.login.InitialLoginRequestBuilder
import mocking.citizenId.login.SigningKeysRequestBuilder
import mocking.citizenId.login.TokenRefreshRequestBuilder
import mocking.citizenId.login.TokenRequestBuilder
import mocking.citizenId.login.UserInfoRequestBuilder
import mocking.citizenId.models.TokenRefreshRequest
import mocking.citizenId.models.TokenRequest
import mocking.citizenId.notifications.ConfigurationRequestBuilder
import mocking.citizenId.settings.AccountSettingsRequestBuilder
import models.Patient

open class CitizenIdMappingBuilder(method: String ="GET", relativePath:String="")
    : MappingBuilder(method, "/citizenid$relativePath") {

    fun initialLoginRequest(patient: Patient,
                            redirectUri: String,
                            clientId: String,
                            customMatcher: String? = null) = InitialLoginRequestBuilder(
            patient, redirectUri, clientId, customMatcher)

    fun createAccountRequest(redirectUri: String = Config.instance.cidRedirectUri,
                             clientId: String = Config.instance.cidClientId) =
            AccountRegistrationRequestBuilder(redirectUri, clientId)

    fun completeLoginRequest(patient: Patient,
                             customIdForPatient: String? = null) = CompleteLoginRequestBuilder(
            patient, customIdForPatient)

    fun tokenRequest(codeVerifier: String, authCode: String? = null, redirectUri : String,
                     customTokenRequest: TokenRequest? = null) = TokenRequestBuilder(
            codeVerifier, authCode, redirectUri, customTokenRequest)

    fun refreshTokenRequest(refreshToken: String,
                     customTokenRequest: TokenRefreshRequest? = null) = TokenRefreshRequestBuilder(refreshToken,
            customTokenRequest)

    fun userInfoRequest(accessToken: String) = UserInfoRequestBuilder(accessToken)

    fun signingKeyRequest() = SigningKeysRequestBuilder()

    fun configurationRequest() = ConfigurationRequestBuilder()

    fun accountSettingsRequest() = AccountSettingsRequestBuilder()
}
