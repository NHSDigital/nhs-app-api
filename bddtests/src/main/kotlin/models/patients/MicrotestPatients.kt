package models.patients

import mocking.defaults.MicrotestMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import models.PatientAge
import models.PatientContactDetails
import models.PatientName
import org.openqa.selenium.InvalidArgumentException
import worker.models.patient.Im1ConnectionToken

class MicrotestPatients {

    companion object : PatientHandler() {

        override fun getDefault(): Patient {
            return eldsonBuleck
        }

        override fun getPatientWithLinkedProfiles(): Patient {
            throw InvalidArgumentException("Not implemented for MICROTEST")
        }

        override fun getPatientWithNoLinkedProfiles(): Patient {
            throw InvalidArgumentException("Not implemented for MICROTEST")
        }

        override fun setOdsCode(patient: Patient, provider: String) {
            throw InvalidArgumentException("Not implemented for MICROTEST")
        }

        private val eldsonBuleckIm1ConnectionToken = Im1ConnectionToken(
                "6pqW/zJEGD5kZ7Zo9J8z1qeIi8LgU7kibAU40CtvjIjWcmQlELqVGhrDZBiAmogsR6LAy9CM4rKVn9nxWrCYmw==",
                nhsNumber = MicrotestMockDefaults.DEFAULT_NHS_NUMBER
        )

        fun postLinkageUserDetails(accountId: String, linkageKey: String): Patient {
            return Patient(
                    name = PatientName(title = "Mr",
                            firstName = "Eldson",
                            surname = "Bulbeck"),
                    age = PatientAge(dateOfBirth = "1986-04-10"),
                    sex = Sex.Male,
                    contactDetails = PatientContactDetails(
                            address = Address(
                                    houseNameFlatNumber = "28 Central Path",
                                    numberStreet = "Troy Avenue",
                                    village = "Horsforth",
                                    town = "Leeds",
                                    county = "West Yorkshire",
                                    postcode = "LS18 5TN"
                            )),
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

        private val eldsonBuleck = Patient(
                name = PatientName(title = "Mr",
                        firstName = "Eldson",
                        surname = "Bulbeck"),
                age = PatientAge(dateOfBirth = "1986-04-10"),
                sex = Sex.Male,
                contactDetails = PatientContactDetails(address = Address(
                        houseNameFlatNumber = "28 Central Path",
                        numberStreet = "Troy Avenue",
                        village = "Horsforth",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                )),
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
