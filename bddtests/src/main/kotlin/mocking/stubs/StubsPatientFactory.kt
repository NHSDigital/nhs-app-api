package mocking.stubs

import config.Config
import models.Patient
import worker.models.session.UserSessionRequest

class StubsPatientFactory {
    companion object {
        private val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private val CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH = 2

        val goodPatientEMIS = generateEMISPatientData("1", "goodpatient", CONNECTION_TOKEN_SUFFIX_LENGTH)
        val timeoutPatientEMIS = generateEMISPatientData("2", "timeoutpatient", CONNECTION_TOKEN_SUFFIX_LENGTH)
        val serviceNotEnabledPatientEMIS = generateEMISPatientData("3", "servicennotenabledpatient",
                                                                   CONNECTION_TOKEN_SUFFIX_LENGTH)
        val sessionErrorPatientEMIS = generateEMISPatientData("4", "sessionerrorpatient",
                                                              CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH)
        val serverErrorPatientEMIS = generateEMISPatientData("5", "servererrorpatient", CONNECTION_TOKEN_SUFFIX_LENGTH)

        val EMISPatientList = listOf(
                goodPatientEMIS,
                serviceNotEnabledPatientEMIS,
                sessionErrorPatientEMIS,
                timeoutPatientEMIS,
                serverErrorPatientEMIS
        )

        private fun generateEMISPatientData(uniqueId: String, loginID: String, length: Int): Patient {
            val pad = uniqueId.padStart(length, '0')
            //do not add end user session id here

            val patient = Patient.picaJones.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    cidUserSession = UserSessionRequest(
                            authCode = "authCode$pad",
                            codeVerifier = "codeVerifier$pad",
                            redirectUrl = Config.instance.cidRedirectUri
                    ),
                    accessToken = "accessToken$pad",
                    connectionToken = "00000000-0000-0000-0000-$pad",
                    userPatientLinkToken = "userPatientLinkToken$pad"
            )
            return patient
        }
    }
}