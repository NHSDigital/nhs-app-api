package models.patients

import mocking.defaults.MicrotestMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import worker.models.patient.Im1ConnectionToken

class MicrotestPatients {

    companion object {

        fun getDefault(): Patient {
            return eldsonBuleck
        }

        private val eldsonBuleckIm1ConnectionToken = Im1ConnectionToken(
                "6pqW/zJEGD5kZ7Zo9J8z1qeIi8LgU7kibAU40CtvjIjWcmQlELqVGhrDZBiAmogsR6LAy9CM4rKVn9nxWrCYmw==",
                nhsNumber = MicrotestMockDefaults.DEFAULT_NHS_NUMBER
        )

        fun microtestPostLinkageUserDetails(accountId: String, linkageKey: String): Patient{
            return Patient(
                    title = "Mr",
                    firstName = "Eldson",
                    surname = "Bulbeck",
                    dateOfBirth = "1986-04-10",
                    sex = Sex.Male,
                    address = Address(
                            houseNameFlatNumber = "28 Central Path",
                            numberStreet = "Troy Avenue",
                            village = "Horsforth",
                            town = "Leeds",
                            county = "West Yorkshire",
                            postcode = "LS18 5TN"
                    ),
                    telephoneFirst = Patient.defaultTelephoneFirst,
                    telephoneSecond = Patient.defaultTelephoneSecond,
                    emailAddress = Patient.defaultEmailAddress,
                    odsCode = MicrotestMockDefaults.DEFAULT_ODS_CODE_MICROTEST,
                    nhsNumbers = listOf(MicrotestMockDefaults.DEFAULT_NHS_NUMBER),
                    connectionToken = MicrotestMockDefaults.DEFAULT_CONNECTION_TOKEN,
                    rosuAccountId = "",
                    apiKey = "",
                    patientId = "",
                    accountId = accountId,
                    linkageKey = linkageKey,
                    im1ConnectionToken = eldsonBuleckIm1ConnectionToken
            )
        }

        val eldsonBuleck = Patient(
                title = "Mr",
                firstName = "Eldson",
                surname = "Bulbeck",
                dateOfBirth = "1986-04-10",
                sex = Sex.Male,
                address = Address(
                        houseNameFlatNumber = "28 Central Path",
                        numberStreet = "Troy Avenue",
                        village = "Horsforth",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
                odsCode = MicrotestMockDefaults.DEFAULT_ODS_CODE_MICROTEST,
                nhsNumbers = listOf(MicrotestMockDefaults.DEFAULT_NHS_NUMBER),
                connectionToken = MicrotestMockDefaults.DEFAULT_CONNECTION_TOKEN,
                rosuAccountId = "",
                apiKey = "",
                patientId = "",
                accountId = "MICROTEST_ACCOUNT_ID",
                linkageKey = "MICROTEST_LINKAGE_KEY",
                im1ConnectionToken = eldsonBuleckIm1ConnectionToken
        )
    }
}