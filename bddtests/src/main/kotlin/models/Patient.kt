package models

import config.Config
import constants.DateTimeFormats
import mocking.citizenId.models.IdTokenBuilder
import mocking.defaults.EmisMockDefaults
import mocking.defaults.MicrotestMockDefaults
import mocking.defaults.MockDefaults
import mocking.defaults.TppMockDefaults
import mocking.defaults.VisionMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import utils.DateConverter
import worker.models.demographics.TppUserSession
import worker.models.patient.Im1ConnectionToken
import worker.models.session.UserSessionRequest

data class Patient(
        val title: String = "",
        val firstName: String = "",
        val surname: String = "",
        val callingName: String = "",
        val dateOfBirth: String = "",
        val sex: Sex = Sex.NotSpecified,
        var telephoneFirst: String = "",
        var telephoneSecond: String = "",
        var emailAddress: String = "",
        val address: Address = Address(),
        val accountId: String = "",
        var odsCode: String = "",
        val connectionToken: String = "",
        val sessionId: String = "",
        val endUserSessionId: String = "",
        val linkageKey: String = "",
        val userPatientLinkToken: String = "",
        val nhsNumbers: List<String> = emptyList(),
        val patientId: String = "",
        val passphrase: String = "",
        val onlineUserId: String = "",
        val rosuAccountId: String = "",
        val apiKey: String = "",
        val cidUserSession: UserSessionRequest = UserSessionRequest(
                authCode = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPS" +
                        "ETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-76" +
                        "57b5e9e403.8d4c0a21-6483-4a52-9d47" +
                        "-6bcd737c634e",
                codeVerifier = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
                redirectUrl = Config.instance.cidRedirectUri),
        val accessToken: String = "eyJzdWIiOiI0NTVmODBiYy02NTkyLTRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJhdWQiOiJuaHMtb25s" +
                "aW5lIiwia2lkIjoiYjcxNDk4NmFjNTI2ZWExMjY1NTVhMzdmMTY4NjU5ZmNlOGI5ZGIyNCIsImlzcyI6Imh0dHBzOi8vYXV0aC" +
                "5leHQuc2lnbmluLm5ocy51ayIsInR5cCI6IkpXVCIsImV4cCI6MTU2MjMxNTg4MSwiaWF0IjoxNTYyMzEyMjgxLCJhbGciOiJS" +
                "UzUxMiIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.eyJzdWIiOiI0NTVmODBiYy02NTkyL" +
                "TRmZTQtODVlMC0wZTdiOTdlYzFmYWQiLCJuaHNfbnVtYmVyIjoiOTc0NDM2Njc1MyIsImlzcyI6Imh0dHBzOi8vYXV0aC5leHQ" +
                "uc2lnbmluLm5ocy51ayIsInZlcnNpb24iOjAsInZ0bSI6Imh0dHBzOi8vYXV0aC5leHQuc2lnbmluLm5ocy51ay90cnVzdG1hc" +
                "msvYXV0aC5leHQuc2lnbmluLm5ocy51ayIsImNsaWVudF9pZCI6Im5ocy1vbmxpbmUiLCJyZXF1ZXN0aW5nX3BhdGllbnQiOiI" +
                "5NzQ0MzY2NzUzIiwiYXVkIjoibmhzLW9ubGluZSIsInRva2VuX3VzZSI6ImFjY2VzcyIsImF1dGhfdGltZSI6MTU2MjMxMjI3O" +
                "Swic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBuaHNfYXBwX2NyZWRlbnRpYWxzIGdwX2ludGVncmF0aW9uX2NyZWRlbnRpYWxzIiw" +
                "idm90IjoiUDkuQ3AuQ2QiLCJleHAiOjE1NjIzMTU4ODEsImlhdCI6MTU2MjMxMjI4MSwicmVhc29uX2Zvcl9yZXF1ZXN0Ijoic" +
                "GF0aWVudGFjY2VzcyIsImp0aSI6IjRlZTA0Mjc1LThhY2QtNGE2NS04MTY2LTRjM2FkOWJjM2FlNSJ9.AqVT_loj8Tzx46CYmo" +
                "LgdFhPflA2NSBxhdAeImYq93Rzx4Q6B0jh3Kd9XMRLi7rtoKFLqjPvWBcZeL-A58qibvN_CtTESXFRzQbTcXnPY0qyV48oxIvd" +
                "ghaw7eMuF2tJnFy-X2ozuqUn_4h-zsHrG80KG9bdu3htAT_hac4cXtY9GC519RQ0835ool-IV9Us7rVMKMCt_f_rNRWu_QhnJE" +
                "Hyc8YLcSZ2b3VRsMjTPIadVOdJeLq2vIbhhiDZ2a9GX5FnwcgE0pW241-5FbOQy9nvtW-gA-7dtQWXObg4QcAqxpRmr1rIbFlB" +
                "4FeK26UZs6IT9dZ-foYfIBl_4Eyggg",
        val tppUserSession: TppUserSession? = null,
        val im1ConnectionTokenAsJson: Im1ConnectionToken? = null,
        val organDonationRegistrationId: String = "AD02745157"
) {

    fun formattedDateOfBirth(): String {
        return DateConverter.convertDateToDateTimeFormat(
                dateOfBirth,
                DateTimeFormats.dateWithoutTimeFormat,
                DateTimeFormats.frontendBasicDateFormat)
    }

    fun dateOfBirthDigitsOnly(): String {
        return dateOfBirth.replace("-", "")
    }

    fun formattedFullName(): String {
        val fullName = "$title $firstName $surname"
        return fullName.trim()
    }

    fun formattedNHSNumber(): String {
        return formatNHSNumber(nhsNumbers.first())
    }

    companion object {

        private const val defaultTelephoneFirst = "02837483567"
        private const val defaultTelephoneSecond = "07737483567"
        private const val defaultEmailAddress = "HalleD@fakeemail.com"

        private val defaultAddress = Address(
                houseNameFlatNumber = "99",
                numberStreet = "Fake Street",
                village = "Fake village",
                town = "Fake town",
                county = "Fake county",
                postcode = "AA00 0AA"
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
                "MICROTEST" -> eldsonBuleck
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

        val montelFryeIm1ConnectionToken = Im1ConnectionToken(
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
                address = defaultAddress,
                telephoneFirst = defaultTelephoneFirst,
                telephoneSecond = defaultTelephoneSecond,
                emailAddress = defaultEmailAddress,
                odsCode = EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                sessionId = "2jM47sZ0ic4FIAcVogI4WI",
                connectionToken = montelFryeIm1ConnectionToken.accessIdentityGuid!!,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                nhsNumbers = listOf("0968764215"),
                accountId = "4140044939",
                linkageKey = "vVGO8bgV6fvPb",
                userPatientLinkToken = "gpSWtREiH9499bPzix8v5b",
                im1ConnectionTokenAsJson = montelFryeIm1ConnectionToken
        )

        val picaJones = Patient(
                title = "",
                firstName = "Pica",
                surname = "Jones",
                dateOfBirth = "1972-04-12",
                address = defaultAddress,
                telephoneFirst = defaultTelephoneFirst,
                telephoneSecond = defaultTelephoneSecond,
                emailAddress = defaultEmailAddress,
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

        val kevinBarryIm1ConnectionToken = Im1ConnectionToken(
                "6pqW/zJEGD5kZ7Zo9J8z1qeIi8LgU7kibAU40CtvjIjWcmQlELqVGhrDZBiAmogsR6LAy9CM4rKVn9nxWrCYmw==",
                accountId = "520993083",
                passphrase = "c2axhQ9VWB2/62XFxvKrNKh9JwgL" +
                        "k0NFY15hIdI6aRytptqiBs6r/k+0Ov" +
                        "GEZfcEdMLJEMp/J4pkOGm2ViaSLca" +
                        "49ODQzz4y+Cu2xOxLaehq/SjEIwfls" +
                        "WeSwCvCAxroId1bXejTdNsV17fOAD0" +
                        "M5nAZF6X9TysOfRR/j5tuR+o=",
                providerId = TppMockDefaults.DEFAULT_TPP_PROVIDER_ID
        )

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
                telephoneFirst = defaultTelephoneFirst,
                telephoneSecond = defaultTelephoneSecond,
                emailAddress = defaultEmailAddress,
                odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers = listOf("5785445875"),
                linkageKey = "passphraseToLink",
                accountId = kevinBarryIm1ConnectionToken.accountId!!,
                patientId = "84df400000000000",
                onlineUserId = "84df400000000000",
                passphrase = kevinBarryIm1ConnectionToken.passphrase!!,
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
                        "84df400000000000"),
                im1ConnectionTokenAsJson = kevinBarryIm1ConnectionToken
        )

        ////////// VISION PATIENTS /////////////

        val adreynCanonIm1ConnectionToken = Im1ConnectionToken(
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
                telephoneFirst = defaultTelephoneFirst,
                telephoneSecond = defaultTelephoneSecond,
                emailAddress = defaultEmailAddress,
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
                im1ConnectionTokenAsJson = adreynCanonIm1ConnectionToken
        )

        ////////// MICROTEST PATIENTS /////////////

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
                telephoneFirst = defaultTelephoneFirst,
                telephoneSecond = defaultTelephoneSecond,
                emailAddress = defaultEmailAddress,
                odsCode = MicrotestMockDefaults.DEFAULT_ODS_CODE_MICROTEST,
                nhsNumbers = listOf("5785445875"),
                connectionToken = "{}",
                rosuAccountId = "",
                apiKey = "",
                patientId = "",
                accountId = "MICROTEST_ACCOUNT_ID",
                linkageKey = "MICROTEST_LINKAGE_KEY",
                im1ConnectionTokenAsJson = Im1ConnectionToken()
        )

        fun formatNHSNumber(nhsNumber: String): String {
            val number = nhsNumber.trim().replace(" ", "")

            return when {
                number.isEmpty() -> ""
                number.length != lengthOfNHSNumber -> number
                else -> "${number.substring(firstNHSNumberIndex, firstNHSNumberFormattingBreak)} " +
                        "${number.substring(firstNHSNumberFormattingBreak, secondNHSNumberFormattingBreak)} " +
                        number.substring(secondNHSNumberFormattingBreak, finalNHSNumberBreak)
            }
        }

        private const val lengthOfNHSNumber = 10
        private const val firstNHSNumberIndex = 0
        private const val firstNHSNumberFormattingBreak = 3
        private const val secondNHSNumberFormattingBreak = 6
        private const val finalNHSNumberBreak = 10
    }
}
