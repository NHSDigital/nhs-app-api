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
                nhsNumbers = listOf("5785445875"),
                connectionToken = "{}",
                rosuAccountId = "",
                apiKey = "",
                patientId = "",
                accountId = "MICROTEST_ACCOUNT_ID",
                linkageKey = "MICROTEST_LINKAGE_KEY",
                im1ConnectionToken = Im1ConnectionToken()
        )
    }
}