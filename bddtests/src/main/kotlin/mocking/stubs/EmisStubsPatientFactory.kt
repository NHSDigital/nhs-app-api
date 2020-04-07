package mocking.stubs

import models.Patient
import models.patients.EmisPatients

class EmisStubsPatientFactory {
    companion object {
        private const val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private const val CONNECTION_TOKEN_SUFFIX_INVALID_LENGTH = 2

        val goodPatientEMIS = generatePatientData(
                PatientUniqueId.GoodPatientEMIS,
                "goodpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val timeoutPatientEMIS = generatePatientData(
                PatientUniqueId.TimeoutPatientEMIS,
                "timeoutpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val serviceNotEnabledPatientEMIS = generatePatientData(
                PatientUniqueId.ServiceNotEnabledPatientEMIS,
                "servicennotenabledpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val sessionErrorPatientEMIS = generatePatientData(
                PatientUniqueId.SessionErrorPatientEMIS,
                "sessionerrorpatient",
                CONNECTION_TOKEN_SUFFIX_INVALID_LENGTH)

        val serverErrorPatientEMIS = generatePatientData(
                PatientUniqueId.ServerErrorPatientEMIS,
                "servererrorpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val EMISPatientList = listOf(
                goodPatientEMIS,
                serviceNotEnabledPatientEMIS,
                sessionErrorPatientEMIS,
                timeoutPatientEMIS,
                serverErrorPatientEMIS
        )

        private fun generatePatientData(uniqueId: PatientUniqueId, loginID: String, length: Int): Patient {
            val pad = uniqueId.Id.padStart(length, '0')
            //do not add end user session id here
            return EmisPatients.picaJones.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    authCode = "authCode$pad",
                    codeVerifier = "codeVerifier$pad",
                    connectionToken = "00000000-0000-0000-0000-$pad",
                    userPatientLinkToken = "userPatientLinkToken$pad")
        }
    }
}
