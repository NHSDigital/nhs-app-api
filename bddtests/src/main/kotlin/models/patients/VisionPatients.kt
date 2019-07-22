package models.patients

import mocking.defaults.VisionMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import worker.models.patient.Im1ConnectionToken

class VisionPatients {

    companion object {

        fun getDefault(): Patient {
            return aderynCanon
        }

        private val adreynCanonIm1ConnectionToken = Im1ConnectionToken(
                im1CacheKey = "aHQMSopfOJlA5vk4zLJJfKEynO8ApadWcwZExxmhfXll0Eahv" +
                        "bnqSg6GEL6h2vjze3egwyTNIp68Q5GT5o3CAg==",
                rosuAccountId = "104969",
                apiKey = "h4h9869kj3ytz6427y7rctkdy3zkpxcncnh" +
                        "vfph76g2h6p9gywjq484c9ghan8tt"
        )

        val aderynCanon = Patient(
                title = "Mr",
                firstName = "Aderyn",
                surname = "Canon",
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
                odsCode = VisionMockDefaults.DEFAULT_ODS_CODE_VISION,
                nhsNumbers = listOf("5785445875"),
                connectionToken = "{\"RosuAccountId\":\"104969\",\"" +
                        "ApiKey\":\"h4h9869kj3ytz6427y7" +
                        "rctkdy3zkpxcncnhvfph76g2h6p9" +
                        "gywjq484c9ghan8tt\"}",
                rosuAccountId = adreynCanonIm1ConnectionToken.rosuAccountId!!,
                apiKey = adreynCanonIm1ConnectionToken.apiKey!!,
                patientId = "1017",
                accountId = "104969",
                linkageKey = "kWWG9kHfNMSjm",
                im1ConnectionToken = adreynCanonIm1ConnectionToken
        )
    }
}