package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus

class AccountRegistrationRequestBuilder(redirectUri: String, clientId: String)
: CitizenIdMappingBuilder("GET", "/cicauth/realms/NHS/protocol/openid-connect/registrations") {

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
                        <a id="register_new_account" href="complete-login?redirect_uri={{request.query.redirect_uri}}&state={{request.query.state}}&code_challenge=3">Register new account</a>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}