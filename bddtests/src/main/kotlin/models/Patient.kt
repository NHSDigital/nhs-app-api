package models

import mocking.defaults.MockDefaults
import worker.models.demographics.Address
import worker.models.demographics.ContactDetails
import worker.models.demographics.Sex

data class Patient(
        val title:String = "",
        val firstName:String = "",
        val surname:String = "",
        val callingName: String = "",
        val dateOfBirth:String = "",
        val sex: Sex = Sex.NotSpecified,
        val contactDetails: ContactDetails = ContactDetails(),
        val address: Address = Address(),
        val accountId:String = "",
        val odsCode:String = "",
        val connectionToken:String = "",
        val sessionId:String = "",
        val endUserSessionId:String = "",
        val linkageKey:String = "",
        val userPatientLinkToken:String = "",
        val nhsNumbers: List<String> = emptyList(),
        val patientId: String = "",
        val passphrase: String = "",
        val onlineUserId: String = ""
) {
    companion object {
        private val defaultAddress = Address(
                houseNameFlatNumber = "99",
                numberStreet = "Fake Street",
                village = "Fake village",
                town = "Fake town",
                county = "Fake county",
                postcode = "AA00 0AA"
        )

        private val defaultContactDetails = ContactDetails(
                telephoneNumber = "02837483567",
                mobileNumber = "07737483567",
                emailAddress = "HalleD@fakeemail.com"
        )

        fun getDefault(gpSystem: String): Patient {
            return when(gpSystem.toUpperCase()) {
                "EMIS" -> montelFrye
                "TPP" -> kevinBarry
                else -> throw IllegalArgumentException("$gpSystem not a valid supplier name.")
            }
        }

        val paulSmith = Patient(
                title = "Mr",
                firstName = "Paul",
                surname = "Smith",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                sessionId = MockDefaults.DEFAULT_SESSION_ID,
                connectionToken = MockDefaults.DEFAULT_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID)
        
        val jackJackson = Patient(
                title = "Mr",
                firstName = "Jack",
                surname = "Jackson",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "gY39SJJMEEg7rNbcsfF8",
                connectionToken = "efa22020-9221-46a6-a0f0-6c0340b8f44d",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID
        )
        
        val alanCook = Patient(
                title = "Mr",
                firstName = "Alan",
                surname = "Cook",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "fbWgorZ8Fggk9c5PgKd7",
                connectionToken = "7e14cfb4-eb7a-44c3-8603-28ee36c7a9bf",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID
        )

        val halleDawe = Patient(
                title = "Miss",
                firstName = "Halle",
                surname = "Dawe",
                dateOfBirth = "1994-02-21T00:00:00",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                sessionId = "4RDwmQVi3OfSbp47dbAnRF",
                connectionToken = "1da4fe9d-0fd2-45bc-90a9-014e57291d0f",
                endUserSessionId = "7YjG1LYkOkSY1iAcXGG8ZU",
                nhsNumbers = listOf("2227007273"),
                accountId = "4937786121",
                linkageKey = "tTALtBP3rLR16",
                userPatientLinkToken = "DbLYlUrwyGpgZ65Mlk6601"
        )
        
        val montelFrye = Patient(
                title =  "",
                firstName =  "Montel",
                surname =  "Frye",
                dateOfBirth =  "1972-04-12T00:00:00",
                address = defaultAddress,
                contactDetails = defaultContactDetails,
                odsCode =  MockDefaults.DEFAULT_ODS_CODE,
                sessionId =  "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken =  "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc",
                endUserSessionId =  "7YjG1LYkOkSY1iAcXGG8ZU",
                nhsNumbers =  listOf("0968764215"),
                accountId =  "4140044939",
                linkageKey =  "vVGO8bgV6fvPb",
                userPatientLinkToken =  "gpSWtREiH9499bPzix8v5b"
        )

        val kevinBarry = Patient(
                title =  "Mr",
                firstName =  "Kevin",
                surname =  "Barry",
                dateOfBirth =  "1985-05-29T00:00:00.0Z",
                sex = Sex.Male,
                address = Address(
                        houseNameFlatNumber = "28 Central Path",
                        numberStreet = "Troy Road",
                        village = "Horsforth",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                contactDetails = defaultContactDetails,
                odsCode =  MockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers =  listOf("5785445875"),
                accountId =  "520993083",
                patientId = "84df400000000000",
                onlineUserId = "84df400000000000",
                passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=",
                connectionToken =  "{\"accountId\": \"520993083\", \"passphrase\":\"c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=\"}",
                endUserSessionId =  MockDefaults.DEFAULT_END_USER_SESSION_ID
        )
    }
}
