package mocking.stubs

import models.Patient
import models.patients.TppPatients

class TppStubsPatientFactory {
    companion object {
        private const val CONNECTION_TOKEN_SUFFIX_LENGTH = 12

        val goodPatientTPP = generatePatientData(
                PatientUniqueId.GoodPatientTPP,
                "goodpatientTPP",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val TppPatientList = listOf(
                goodPatientTPP
        )

        private fun generatePatientData(uniqueId: PatientUniqueId, loginID: String, length: Int): Patient {
            val pad = uniqueId.Id.padStart(length, '0')
            //do not add end user session id here
            return TppPatients.kevinBarry.copy(
                    subject = "subject-${uniqueId.name}",
                    firstName = "You are logged in as",
                    surname = loginID,
                    connectionToken = "00000000-0000-0000-0000-$pad"
            )
        }
    }
}
