package mocking.citizenId.login

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

class InitialLoginRequestBuilder(
        val patient: Patient,
        redirectUri: String,
        clientId: String,
        matcherToUse: String? = null)
    : CitizenIdMappingBuilder("GET", "/authorize") {

    init {
        val matcherString = matcherToUse ?: "equalTo"
        requestBuilder
                .andQueryParameter("redirect_uri", redirectUri, matcherString)
                .andQueryParameter("client_id", clientId)
                .andQueryParameter("response_type", "code")
    }

    fun respondWithLoginPage(): Mapping {
        if (Config.instance.autoLogin == "true" || OptionManager.instance().isEnabled(NoJsOption::class)) {
            return redirectTo(
                    "{{request.query.redirect_uri}}?state={{request.query.state}}&code=" +
                            patient.authCode)
        } else {
            return respondWith(HttpStatus.SC_OK) {
                andTemplatedHtmlBody("""
                <html>
                    <body>
                        <form action="complete-login" method="get">
                            <input value="source" type="hidden" name="{{request.query.source}}" >
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
