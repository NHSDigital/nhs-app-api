package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.citizenId.models.TokenRequest
import mocking.citizenId.models.login.token.SucceededResponse
import mocking.defaults.MockDefaults
import mocking.models.Mapping
import org.apache.http.HttpStatus
import java.io.UnsupportedEncodingException
import java.net.URLEncoder

class TokenRequestBuilder(codeVerifier: String, authCode: String?, customTokenRequest: TokenRequest? =null)
    : CitizenIdMappingBuilder("POST", "/token") {

    init {
        requestBuilder
                .andHeader("Authorization", "Basic", "contains")
                .andHeader("Content-Type", "application/x-www-form-urlencoded")

        // add token query parameters

        val tokenRequest = customTokenRequest ?: TokenRequest(codeVerifier, code = authCode)
        val body = tokenRequestToQueryParams(tokenRequest)
        requestBuilder.andBody(body, "matches")
    }

    @Suppress("TooGenericExceptionThrown")
    private fun tokenRequestToQueryParams(tokenRequest: TokenRequest): String {
        val map = LinkedHashMap<String, String>()
        map["grant_type"] = tokenRequest.grantType
        if (tokenRequest.code != null) {
            map["code"] = tokenRequest.code
        }
        map["redirect_uri"] = tokenRequest.redirectUri
        map["code_verifier"] = ".*"
        map["client_id"] = tokenRequest.clientId
        map["code_challenge_method"] = tokenRequest.code_challenge_method

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
                throw RuntimeException("This method requires UTF-8 encoding support", e)
            }
        }

        return stringBuilder.toString()
    }

    fun respondWithSuccess(
            accessToken: String = MockDefaults.patient.accessToken,
            expiresIn: String = "90",
            scope: String = "openid",
            tokenType: String = "Bearer",
            idToken: String = ""): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(SucceededResponse(accessToken, tokenType, expiresIn, scope, idToken))
        }
    }

    fun respondWithBadRequest(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) { build() }
    }

    fun respondWithServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
    }
}
