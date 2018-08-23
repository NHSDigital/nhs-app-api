package mocking.citizenId.login

import mocking.citizenId.CitizenIdMappingBuilder
import mocking.defaults.MockDefaults
import mocking.models.Mapping
import models.Patient
import config.Config
import org.apache.http.HttpStatus

class InitialLoginRequestBuilder(
        val patient: Patient = MockDefaults.patient,
        redirectUri: String,
        clientId: String,
        matcherToUse:String? = null)
    : CitizenIdMappingBuilder("GET", "/authorize") {

    init {
            val matcherString = matcherToUse?: "equalTo"
            requestBuilder
                    .andQueryParameter("redirect_uri", redirectUri,matcherString)
                    .andQueryParameter("client_id", clientId)
                    .andQueryParameter("response_type", "code")

    }

    fun respondWithLoginPage(): Mapping {
        if(Config.instance.autoLogin == "true") {
            return redirectTo(
                    "{{request.query.redirect_uri}}?state={{request.query.state}}&code=" +
                            "${patient.cidUserSession.authCode!!}")
        }
        else {
            return respondWith(HttpStatus.SC_OK) {
                andTemplatedHtmlBody("""
                <html>
                    <body>
                        <form action="complete-login" method="get">
                            <input value="clientId" type="hidden" name="client_id" >
                            <input value="3" type="hidden" name="code_challenge">
                            <input value ="codeMethod" type="hidden" name="code_challenge_method" >
                            <input value="openid" type="hidden" name="scope">
                            <input value="{{request.query.redirect_uri}}" type="hidden" name="redirect_uri">
                            <input value="{{request.query.state}}" type="hidden" name="state">
                            <input value="responseType" type="hidden" name="response_type">
                            <input placeholder="patient object hash" type="text" name="mock_patient" >
                            <input type="submit" value="complete login">
                        </form>
                    </body>
                </html>
            """.trimIndent())
            }
        }
    }
}
