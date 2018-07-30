package models

import config.Config
import mocking.defaults.MockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.ContactDetails
import mocking.emis.demographics.Sex
import worker.models.demographics.TppUserSession
import worker.models.session.UserSessionRequest
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
        val onlineUserId: String = "",
        val rosuAccountId: String = "",
        val apiKey: String = "",
        val cidUserSession: UserSessionRequest= UserSessionRequest(
                authCode = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPSETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-7657b5e9e403.8d4c0a21-6483-4a52-9d47-6bcd737c634e",
                codeVerifier = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
                redirectUrl = Config.instance.cidRedirectUri),
        val accessToken: String ="access_token",
        val tppUserSession: TppUserSession? = null
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
                "VISION" -> aderynCanon
                else -> throw IllegalArgumentException("$gpSystem not a valid supplier name.")
            }
        }

        //////////// EMIS PATIENTS /////////////

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
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
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
                endUserSessionId =  MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers =  listOf("0968764215"),
                accountId =  "4140044939",
                linkageKey =  "vVGO8bgV6fvPb",
                userPatientLinkToken =  "gpSWtREiH9499bPzix8v5b"
        )

        val johnSmith = Patient(
                title = "Mr",
                firstName = "John",
                surname = "Smith",
                dateOfBirth = "1919-12-24T14:03:15.892Z",
                odsCode = MockDefaults.DEFAULT_ODS_CODE,
                connectionToken = MockDefaults.DEFAULT_CONNECTION_TOKEN,
                sessionId = "MT4vWCxTKXRYr7fFJWM3wB",
                endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
                accountId = "1195029928",
                linkageKey = "KjwzyFSEUAGj4",
                userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
                nhsNumbers = listOf("7174450393")
        )

        ////////// TPP PATIENTS /////////////

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
                linkageKey = "?2sY3qyZp5gRB8*b",
                accountId =  "520993083",
                patientId = "84df400000000000",
                onlineUserId = "84df400000000000",
                passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=",
                connectionToken =  "{\"accountId\":\"520993083\",\"passphrase\":\"c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15hIdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o=\"}",
                endUserSessionId =  MockDefaults.DEFAULT_END_USER_SESSION_ID,
                tppUserSession = TppUserSession("ZT8wLjK6beFOdXoiNIHbD+TbPrl0Y3KmVXy4GYM253hQlxwp2qMKW7zgbjgTWJzCvTcZxb2BZNW5IdGtaWtahGkv" +
                        "qW6jK5QnkU2npQjTxAN9zVHgDp4raIxXc0gY+SB1hm/7XMgD4YHnmtlYK3WINs3gcAfC2l5B42vpSWULpCA=",
                        "84df400000000000", "KGPD", "84df400000000000")
        )

        ////////// VISION PATIENTS /////////////
        val aderynCanon = Patient(
                title = "Mr",
                firstName = "Aderyn",
                surname = "Canon",
                dateOfBirth = "1986-04-10T00:00:00.0Z",
                sex = Sex.Male,
                address = Address(
                        houseNameFlatNumber = "28 Central Path",
                        numberStreet = "Troy Avenue",
                        village = "Horsforth",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                contactDetails = defaultContactDetails,
                odsCode = MockDefaults.DEFAULT_ODS_CODE_VISION,
                nhsNumbers = listOf("5785445875"),
                connectionToken = "{\"rosuAccountId\": \"104969\", \"apiKey\":\"h4h9869kj3ytz6427y7rctkdy3zkpxcncnhvfph76g2h6p9gywjq484c9ghan8tt\"}",
                rosuAccountId = "104969",
                apiKey = "h4h9869kj3ytz6427y7rctkdy3zkpxcncnhvfph76g2h6p9gywjq484c9ghan8tt"
        )
    }
}
