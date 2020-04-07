package mocking.stubs

import models.IdentityProofingLevel
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

        private val sessionErrorPatientEMIS = generatePatientData(
                PatientUniqueId.SessionErrorPatientEMIS,
                "sessionerrorpatient",
                CONNECTION_TOKEN_SUFFIX_INVALID_LENGTH)

        private val serverErrorPatientEMIS = generatePatientData(
                PatientUniqueId.ServerErrorPatientEMIS,
                "servererrorpatient",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        private val p5PatientEMIS = generatePatientData(
                PatientUniqueId.P5PatientEMIS,
                "p5patientemis",
                CONNECTION_TOKEN_SUFFIX_LENGTH,
                IdentityProofingLevel.P5)

        val EMISPatientList = listOf(
                goodPatientEMIS,
                serviceNotEnabledPatientEMIS,
                sessionErrorPatientEMIS,
                timeoutPatientEMIS,
                serverErrorPatientEMIS,
                p5PatientEMIS
        )

        private fun generatePatientData(
                uniqueId: PatientUniqueId,
                loginID: String,
                length: Int,
                identityProofingLevel: IdentityProofingLevel = IdentityProofingLevel.P9)
                : Patient {
            val pad = uniqueId.Id.padStart(length, '0')
            //do not add end user session id here
            return EmisPatients.picaJones.copy(
                    subject = "subject-${uniqueId.name}",
                    firstName = "You are logged in as",
                    surname = loginID,
                    authCode = "authCode$pad",
                    codeVerifier = "codeVerifier$pad",
                    connectionToken = "00000000-0000-0000-0000-$pad",
                    userPatientLinkToken = "userPatientLinkToken$pad",
                    identityProofingLevel = identityProofingLevel)
        }
    }
}
