package models.patients

import mocking.defaults.EmisMockDefaults
import mocking.emis.demographics.Sex
import models.Patient
import models.PatientAge
import models.PatientContactDetails
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

        override fun getPatientUnder18(): Patient {
            return lenaluthor
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
                patientActivityContextGuid = "2a3c3d6d-3234-4e78-8953-96395f354671",
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
                patientActivityContextGuid = "1a05dd89-07b1-46e1-a842-44eb96f1b8fd",
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
                contactDetails = PatientContactDetails(),
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
                patientActivityContextGuid = "71c88d41-ec6c-46de-9a65-81aa0fbee3a4",
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
                userPatientLinkToken = "8v5bgpSW9bPzixtREiH949",
                patientActivityContextGuid = "0bc41dcb-84cb-4591-8cdf-3baa7f6862d5"
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
                patientActivityContextGuid = "0ab3bad4-b145-44d1-9f1f-7025b9928f69",
                nhsNumbers = listOf("7174450393")
        )

        val karadanvers = Patient(
                name = PatientName(title = "Miss",
                        firstName = "Kara",
                        surname = "Danvers"),
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                age = PatientAge(dateOfBirth = "2004-04-12"),
                patientActivityContextGuid = "6697842f-8ce8-4164-a73d-69477e394aea",
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
                patientActivityContextGuid = "f2f1c137-7f89-4ce8-b0c4-d886fb4352cf",
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
                patientActivityContextGuid = "249197ed-c6c8-4b52-b3aa-927b34072c33",
                linkedAccounts = setOf(karadanvers.copy(), lenaluthor.copy())
        )
    }
}
