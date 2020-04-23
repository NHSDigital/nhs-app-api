package mocking.citizenId.login

import config.Config
import mocking.citizenId.CitizenIdMappingBuilder
import mocking.models.Mapping
import mocking.stubs.StubbedEnvironment
import models.Patient
import org.apache.http.HttpStatus
import utils.SerenityHelpers
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
                .andQueryParameter("vtr", "[\"P5.Cp.Cd\", \"P5.Cp.Ck\", \"P5.Cm\"]")
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
                    <head>
                        <style>
                            ul {
                                display: flex;
                                flex-wrap: wrap;
                                list-style-type: none;
                                padding: 0;
                            }
                            li {
                                margin: 0 10px;
                                padding: 0 10px;
                                border: solid 1px;
                                border-radius: 10px;
                                cursor: hand;
                            }
                            li:hover {
                                background-color: #77DD77;
                            }
                            h3 {
                                text-align: center;
                            }
                        </style>
                    </head>
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
                            <input placeholder="patient object hash" type="text" name="mock_patient" id="mock_patient">
                            <input type="submit" value="complete login">
                        </form>
                        ${userList()}
                    </body>
                </html>
            """.trimIndent())
            }
        }
    }

    private fun userList(): String {
        val patient = SerenityHelpers.getPatientOrNull()
        return when {
            patient != null ->
                """
                    <h2>Integration Test Patient</h2>
                    <ul>
                        ${patientInfo(patient, patient.hashCode().toString())}
                    </ul>
                """
            else -> {
                val patientList = StubbedEnvironment
                        .getPatientList()
                        .joinToString("") { patientInfo(it, it.surname) }
                """
                    <h2>Dev Stubs Patients</h2>
                    <ul>
                        $patientList
                    </ul>
                """
            }
        }
    }

    private fun patientInfo(patient: Patient, loginId: String): String =
            """
                <li onclick="document.getElementById('mock_patient').value = '$loginId'">
                    <h3>$loginId</h3>
                    <dl>
                        <dt>First Name</di>
                        <dd>${patient.firstName}</dd>
                        <dt>Surname</di>
                        <dd>${patient.surname}</dd>
                    </dl>
                </li>
            """
}
