package models.patients

import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import mocking.emis.demographics.Sex
import models.Patient
import worker.models.patient.Im1ConnectionToken

class EmisPatients {

    companion object {
        fun getDefault(): Patient {
            return montelFrye
        }

        val paulSmith = Patient(
                title = "Mr",
                firstName = "Paul",
                surname = "Smith",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                dateOfBirth = "1972-04-12",
                sessionId = "AJYF0ufQI6tTpdfwaXAt",
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273"))

        val jackJackson = Patient(
                title = "Mr",
                firstName = "Jack",
                surname = "Jackson",
                dateOfBirth = "1972-04-12",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "gY39SJJMEEg7rNbcsfF8",
                connectionToken = "efa22020-9221-46a6-a0f0-6c0340b8f44d",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273")
        )

        val alanCook = Patient(
                title = "Mr",
                firstName = "Alan",
                surname = "Cook",
                dateOfBirth = "1972-04-12",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "fbWgorZ8Fggk9c5PgKd7",
                connectionToken = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273")
        )

        val halleDawe = Patient(
                title = "Miss",
                firstName = "Halle",
                surname = "Dawe",
                dateOfBirth = "1994-02-21",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "4RDwmQVi3OfSbp47dbAnRF",
                connectionToken = "1da4fe9d-0fd2-45bc-90a9-014e57291d0f",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("2227007273"),
                accountId = "4937786121",
                linkageKey = "tTALtBP3rLR16",
                userPatientLinkToken = "DbLYlUrwyGpgZ65Mlk6601"
        )

        private val montelFryeIm1ConnectionToken = Im1ConnectionToken(
                "zL7i405lQKsEjB8201inpU0A17qCNETe30VPzP3anHXWd2Da9LQ/lfo6XHxq" +
                        "/redv0kOktvHpl5+fFsxBNHAog==",
                accessIdentityGuid = "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc"
        )

        val montelFrye = Patient(
                title = "Mr",
                firstName = "Montel",
                surname = "Frye",
                dateOfBirth = "1972-04-12",
                sex = Sex.Male,
                address = Patient.defaultAddress,
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken = montelFryeIm1ConnectionToken.accessIdentityGuid!!,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("0968764215"),
                accountId = "4140044939",
                linkageKey = "vVGO8bgV6fvPb",
                userPatientLinkToken = "gpSWtREiH9499bPzix8v5b",
                im1ConnectionToken = montelFryeIm1ConnectionToken
        )

        val picaJones = Patient(
                title = "",
                firstName = "Pica",
                surname = "Jones",
                dateOfBirth = "1972-04-12",
                address = Patient.defaultAddress,
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "4FIAcVogI4WI2jM47sZ0ic",
                connectionToken = "7a3a3cf8-4fcc-a797-a4b9-629cdbe104fc",
                endUserSessionId = "SY1iAcXGG8ZU7YjG1LYkOk",
                nhsNumbers = listOf("6421509687"),
                accountId = "4493941400",
                linkageKey = "V6fvPbvVGO8bg",
                userPatientLinkToken = "8v5bgpSW9bPzixtREiH949"
        )

        val johnSmith = Patient(
                title = "Mr",
                firstName = "John",
                surname = "Smith",
                dateOfBirth = "1919-12-24",
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                connectionToken = EmisMockDefaults.DEFAULT_CONNECTION_TOKEN,
                sessionId = "MT4vWCxTKXRYr7fFJWM3wB",
                endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
                accountId = "1195029928",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                nhsNumbers = listOf("7174450393")
        )
    }
}
