package models

import config.Config
import constants.DateTimeFormats
import mocking.citizenId.models.IdTokenBuilder
import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.ContactDetails
import mocking.emis.demographics.Sex
import mocking.vision.VisionMockDefaults
import utils.DateConverter
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
                authCode = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPS" +
                           "ETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-76" +
                           "57b5e9e403.8d4c0a21-6483-4a52-9d47" +
                           "-6bcd737c634e",
                codeVerifier = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
                redirectUrl = Config.instance.cidRedirectUri),
        val accessToken: String ="access_token",
        val tppUserSession: TppUserSession? = null
) {

    fun formattedDateOfBirth(): String {
        return DateConverter.convertDateToDateTimeFormat(
                dateOfBirth,
                DateTimeFormats.dateWithoutTimeFormat,
                DateTimeFormats.frontendBasicDateFormat)
    }

    fun formattedFullName(): String {
        val fullName = "$title $firstName $surname";
        return fullName.trim()
    }

    fun formattedNHSNumber(): String {
        return formatNHSNumber(nhsNumbers.first())
    }

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

        private val idTokenBuilder = IdTokenBuilder(
                Config.instance.cidJwtIssuer,
                Config.instance.cidClientId
        )

        fun getIdToken(patient: Patient): String {
            return idTokenBuilder.getSignedToken(Config.keyStore.signer, patient).serialize()
        }

        fun getDefault(gpSystem: String): Patient {
            return when (gpSystem.toUpperCase()) {
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

        val montelFrye = Patient(
                title = "Mr",
                firstName = "Montel",
                surname = "Frye",
                dateOfBirth = "1972-04-12",
                address = defaultAddress,
                contactDetails = defaultContactDetails,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken = "7a3a3cf8-a797-4fcc-a4b9-629cdbe104fc",
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("0968764215"),
                accountId = "4140044939",
                linkageKey = "vVGO8bgV6fvPb",
                userPatientLinkToken = "gpSWtREiH9499bPzix8v5b"
        )

        val picaJones = Patient(
                title = "",
                firstName = "Pica",
                surname = "Jones",
                dateOfBirth = "1972-04-12",
                address = defaultAddress,
                contactDetails = defaultContactDetails,
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

        ////////// TPP PATIENTS /////////////

        val kevinBarry = Patient(
                title = "Mr",
                firstName = "Kevin",
                surname = "Barry",
                dateOfBirth = "1985-05-29",
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
                odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers = listOf("5785445875"),
                linkageKey = "?2sY3qyZp5gRB8*b",
                accountId = "520993083",
                patientId = "84df400000000000",
                onlineUserId = "84df400000000000",
                passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgL" +
                        "k0NFY15hIdI6aRytptqiBs6r/k+0Ov" +
                        "GEZfcEdMLJEMp/J4pkOGm2ViaSLca" +
                        "49ODQzz4y+Cu2xOxLaehq/SjEIwfls" +
                        "WeSwCvCAxroId1bXejTdNsV17fOAD0" +
                        "M5nAZF6X9TysOfRR/j5tuR+o=",
                connectionToken = TppMockDefaults.DEFAULT_TPP_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                tppUserSession = TppUserSession("ZT8wLjK6beFO" +
                        "dXoiNIHbD+TbPrl0Y3Km" +
                        "VXy4GYM253hQlxwp2qMKW" +
                        "7zgbjgTWJzCvTcZxb2BZN" +
                        "W5IdGtaWtahGkv" +
                        "qW6jK5QnkU2npQjTxAN9zVHgDp4raIxXc0gY+SB1hm/7XMgD" +
                        "4YHnmtlYK3WINs3gcAfC2l5B42vpSWULpCA=",
                        "84df400000000000",
                        TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                        "84df400000000000")
        )

        ////////// VISION PATIENTS /////////////
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
                contactDetails = defaultContactDetails,
                odsCode = VisionMockDefaults.DEFAULT_ODS_CODE_VISION,
                nhsNumbers = listOf("5785445875"),
                connectionToken = "{\"RosuAccountId\":\"104969\",\"" +
                        "ApiKey\":\"h4h9869kj3ytz6427y7" +
                        "rctkdy3zkpxcncnhvfph76g2h6p9" +
                        "gywjq484c9ghan8tt\"}",
                rosuAccountId = "104969",
                apiKey = "h4h9869kj3ytz6427y7rctkdy3zkpxcncnh" +
                        "vfph76g2h6p9gywjq484c9ghan8tt",
                patientId = "1017",
                accountId = "104969",
                linkageKey = "kWWG9kHfNMSjm"
        )

        public fun formatNHSNumber(nhsNumber: String): String {
            val number = nhsNumber.trim().replace(" ", "")
            return "${number.substring(firstNHSNumberIndex, firstNHSNumberFormattingBreak)} " +
                    "${number.substring(firstNHSNumberFormattingBreak, secondNHSNumberFormattingBreak)} " +
                    number.substring(secondNHSNumberFormattingBreak, finalNHSNumberBreak)
        }

        private const val firstNHSNumberIndex = 0
        private const val firstNHSNumberFormattingBreak = 3
        private const val secondNHSNumberFormattingBreak = 6
        private const val finalNHSNumberBreak = 10

    }
}
