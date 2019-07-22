package mocking.stubs

import models.Patient
import models.patients.TppPatients

class TppStubsPatientFactory {
    companion object {
        private val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private val CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH = 2

        val goodPatientTPP = generatePatientData("6", "goodpatientTPP",
                CONNECTION_TOKEN_SUFFIX_LENGTH)

        val TppPatientList = listOf(
                goodPatientTPP
        )
        
        private fun generatePatientData(uniqueId: String, loginID: String, length: Int):
                Patient {
            val pad = uniqueId.padStart(length, '0')
            //do not add end user session id here

            return TppPatients.kevinBarry.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    connectionToken = "00000000-0000-0000-0000-$pad"
            )
        }
    }
}