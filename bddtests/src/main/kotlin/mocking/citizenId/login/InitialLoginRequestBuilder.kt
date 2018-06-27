package mocking.citizenId.login

import mocking.MockingClient
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class InitialLoginRequestBuilder(redirectUri: String, clientId: String)
    : CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/auth") {

    init {
        requestBuilder
                .andQueryParameter("redirect_uri", redirectUri)
                .andQueryParameter("client_id", clientId)
                .andQueryParameter("response_type", "code")
    }

    fun respondWithLoginPage(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andTemplatedHtmlBody("""
                <html>
                    <body>
                        <a id="complete_login" href="complete-login?redirect_uri={{request.query.redirect_uri}}&state={{request.query.state}}&code_challenge=3">Complete Login</a>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}