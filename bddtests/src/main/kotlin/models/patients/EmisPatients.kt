package models.patients

import mocking.defaults.EmisMockDefaults
import mocking.emis.demographics.Sex
import models.Patient
import models.PatientAge
import models.PatientName
import worker.models.patient.Im1ConnectionToken

class EmisPatients {

    companion object : PatientHandler(){
        override fun getDefault(): Patient {
            return montelFrye
        }

        override fun getPatientWithLinkedProfiles(): Patient {
            // Ensure the sessionId and endUserSessionId of the linked accounts
            // match the values in the main (logged in) account
            tonyStark.linkedAccounts.forEach { linkedAccount ->
                linkedAccount.sessionId = tonyStark.sessionId
                linkedAccount.endUserSessionId = tonyStark.endUserSessionId
            }
            return tonyStark
        }

        override fun getPatientWithNoLinkedProfiles(): Patient {
            return picaJones
        }

        override fun setOdsCode(patient: Patient, provider: String) {
            val targetOdsCode = when (provider.toUpperCase()) {
                "ECONSULT" -> EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT
                "IM1" -> EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_IM1
                "INFORMATICA" -> EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA
                "GPATHAND" -> EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND
                else -> throw IllegalArgumentException("$provider not a valid appointment provider name.")
            }
            patient.updateOdsCodes(targetOdsCode)
        }

        val paulSmith = Patient(
                name = PatientName(
                        title = "Mr",
                        firstName = "Paul",
                        surname = "Smith"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                age = PatientAge(dateOfBirth = "1972-04-12"),
                sessionId = "AJYF0ufQI6tTpdfwaXAt",
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273"))

        private val alanCook = Patient(
                name = PatientName(
                        title = "Mr",
                        firstName = "Alan",
                        surname = "Cook"),
                age = PatientAge(dateOfBirth = "1972-04-12"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "id83hdydGyo6kKl0gaRdRb",
                sessionId = "fbWgorZ8Fggk9c5PgKd7",
                connectionToken = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273")
        )

        private val montelFryeIm1ConnectionToken = Im1ConnectionToken(
                "zL7i405lQKsEjB8201inpU0A17qCNETe30VPzP3anHXWd2Da9LQ/lfo6XHxq" +
                        "/redv0kOktvHpl5+fFsxBNHAog==",
                accessIdentityGuid = "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc"
        )

        val montelFrye = Patient(
                name = PatientName(title = "Mr",
                        firstName = "Montel",
                        surname = "Frye"),
                age = PatientAge(dateOfBirth = "1972-04-12"),
                sex = Sex.Male,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken = montelFryeIm1ConnectionToken.accessIdentityGuid!!,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("0968764215"),
                accountId = "4140044939",
                linkageKey = "vVGO8bgV6fvPb",
                userPatientLinkToken = "gpSWtREiH9499bPzix8v5b",
                im1ConnectionToken = montelFryeIm1ConnectionToken,
                linkedAccounts = setOf(alanCook.copy(), paulSmith.copy())
        )

        val picaJones = Patient(
                name = PatientName(
                        title = "",
                        firstName = "Pica",
                        surname = "Jones"),
                age = PatientAge(dateOfBirth = "1972-04-12"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "4FIAcVogI4WI2jM47sZ0ic",
                connectionToken = "7a3a3cf8-4fcc-a797-a4b9-629cdbe104fc",
                endUserSessionId = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                nhsNumbers = listOf("6421509687"),
                accountId = "4493941400",
                linkageKey = "V6fvPbvVGO8bg",
                userPatientLinkToken = "8v5bgpSW9bPzixtREiH949"
        )

        val johnSmith = Patient(
                name = PatientName(
                        title = "Mr",
                        firstName = "John",
                        surname = "Smith"),
                age = PatientAge(dateOfBirth = "1919-12-24"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                sessionId = "MT4vWCxTKXRYr7fFJWM3wB",
                endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
                accountId = "1195029928",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                nhsNumbers = listOf("7174450393")
        )

        val karadanvers = Patient(
                name = PatientName(title = "Miss",
                        firstName = "Kara",
                        surname = "Danvers"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                age = PatientAge(dateOfBirth = "1972-04-12"),
                sessionId = "AJYF0ufQI6tTpdfwaXAt",
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273")
        )

        val lenaluthor = Patient(
                name = PatientName(
                        title = "Miss",
                        firstName = "Lena",
                        surname = "Luthor"),
                age = PatientAge(
                        dateOfBirth = "1972-04-12"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "id83hdydGyo6kKl0gaRdRb",
                sessionId = "fbWgorZ8Fggk9c5PgKd7",
                connectionToken = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273")
        )

        val tonyStark = Patient(
                name = PatientName(
                        title = "Mr",
                        firstName = "Tony",
                        surname = "Stark"),
                age = PatientAge(dateOfBirth = "1972-04-12"),
                sex = Sex.Male,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("0968764215"),
                accountId = "4140044939",
                linkageKey = "vVGO8bgV6fvPb",
                userPatientLinkToken = "gpSWtREiH9499bPzix8v5b",
                linkedAccounts = setOf(karadanvers.copy(), lenaluthor.copy())
        )
    }
}
