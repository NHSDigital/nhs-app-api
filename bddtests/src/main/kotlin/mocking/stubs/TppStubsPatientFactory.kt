package mocking.stubs

import models.Patient
import models.patients.TppPatients

class TppStubsPatientFactory {
    companion object {
        private val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
        private val CONNECTION_TOKEN_SUFFIX_INVALIDLENGTH = 2

        val goodPatientTPP = generatePatientData(PatientUniqueId.GoodPatientTPP, "goodpatientTPP",
                CONNECTION_TOKEN_SUFFIX_LENGTH, GOOD_TOKEN_TPP)

        val TppPatientList = listOf(
                goodPatientTPP
        )
        
        private fun generatePatientData(uniqueId: PatientUniqueId, loginID: String, length: Int, accessToken: String):
                Patient {
            val pad = uniqueId.Id.padStart(length, '0')
            //do not add end user session id here

            val patient = TppPatients.kevinBarry.copy(
                    firstName = "You are logged in as",
                    surname = loginID,
                    connectionToken = "00000000-0000-0000-0000-$pad"
            )
            patient.accessToken = accessToken
            return patient
        }
    }
}

private const val GOOD_TOKEN_TPP: String = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlM" +
        "C0wZTdiOTdlYz" +
        "FmYWQiLCJuaHNfbnVtYmVyIjoiNTc4NTQ0NTg3NSIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ayIs" +
        "InZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hcmsvYXV0aC5leHQuc2" +
        "lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI1Nzg1NDQ1ODc1" +
        "IiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3OSwic2NvcG" +
        "UiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiwidm90" +
        "IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MiwicmVhc29uX2Zvcl9yZXF1ZXN0IjoicG" +
        "F0aWVudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9._AA4BNb0nss2w9T" +
        "bp_lcEX6gVSNFKSV4CJaAAJrfqY0"
