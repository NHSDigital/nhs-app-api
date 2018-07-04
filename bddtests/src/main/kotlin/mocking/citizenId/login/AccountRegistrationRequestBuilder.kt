package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class AccountRegistrationRequestBuilder(redirectUri: String, clientId: String, patient: Patient)
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
                        <form action="complete-login" method="get">
                            <input value="{{request.query.redirect_uri}}" type="hidden" name="redirect_uri">
                            <input value="{{request.query.client_id}}" type="hidden" name="client_id" >
                            <input value="3" type="hidden" name="code_challenge">
                            <input value ="{{request.query.code_method}}" type="hidden" name="code_challenge_method" >
                            <input value="{{request.query.response_type}}" type="hidden" name="response_type">
                            <input value="{{request.query.state}}" type="hidden" name="state">
                            <input placeholder="patient object hash" type="text" name="mock_patient" >
                            <input type="submit" value="complete registration">
                          </form>
                    </body>
                </html>
            """.trimIndent())
        }
    }
}