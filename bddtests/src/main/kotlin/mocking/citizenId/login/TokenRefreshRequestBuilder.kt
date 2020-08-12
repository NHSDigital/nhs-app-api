package mocking.citizenId.login

import com.thoughtworks.xstream.InitializationException
import constants.SessionConstants
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.TokenRefreshRequest
import mocking.citizenId.models.login.token.SucceededResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.io.UnsupportedEncodingException
import java.net.URLEncoder

class TokenRefreshRequestBuilder(refreshToken: String = SessionConstants.RefreshToken,
                                 customRefreshTokenRequest: TokenRefreshRequest? = null)
    : CitizenIdMappingBuilder("POST", "/token") {

    init {
        requestBuilder
                .andHeader("Content-Type", "application/x-www-form-urlencoded")

        // add token query parameters
        val tokenRefreshRequest = customRefreshTokenRequest ?: TokenRefreshRequest(refresh_token =  refreshToken)
        val body = tokenRequestToQueryParams(tokenRefreshRequest)
        requestBuilder.andBody(body, "matches")
    }

    private fun tokenRequestToQueryParams(tokenRequest: TokenRefreshRequest): String {
        val map = LinkedHashMap<String, String>()
        map["grant_type"] = tokenRequest.grantType
        map["client_assertion"] = ".*"
        map["client_assertion_type"] = tokenRequest.client_assertion_type
        map["refresh_token"] = tokenRequest.refresh_token

        val stringBuilder = StringBuilder()

        for (key in map.keys) {
            if (stringBuilder.isNotEmpty()) {
                stringBuilder.append("&")
            }
            val value = map[key]
            try {
                stringBuilder.append(key)
                stringBuilder.append("=")
                stringBuilder.append(if (value != null) URLEncoder.encode(value, "UTF-8") else "")
            } catch (e: UnsupportedEncodingException) {
                throw InitializationException("This method requires UTF-8 encoding support", e)
            }
        }

        return stringBuilder.toString()
    }

    fun respondWithSuccess(accessToken: String): Mapping {
        val expiresIn = "90"
        val scope = "openid profile nhs_app_credentials gp_integration_credentials"
        val tokenType = "Bearer"
        val idToken = ""
        val refreshToken = SessionConstants.RefreshToken
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(SucceededResponse(accessToken, tokenType, expiresIn, scope, idToken, refreshToken))
        }
    }

    fun respondWithBadRequest(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) { build() }
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }
}
