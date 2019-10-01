package models

import config.Config
import constants.DateTimeFormats
import mocking.AccessTokenBuilder
import mocking.IdTokenBuilder
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.patients.EmisPatients
import models.patients.MicrotestPatients
import models.patients.TppPatients
import models.patients.VisionPatients
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
        var subject: String = "3ad631b4-7a7a-434d-8a7b-1c8ac3c56132",
        val im1ConnectionToken: Im1ConnectionToken? = null,
        val organDonationRegistrationId: String = "AD02745157"
) {
    var accessToken: String = AccessTokenBuilder().getSignedToken(this).serialize()

    val tppUserSession : TppUserSession? by lazy {
        TppUserSession("ZT8wLjK6beFO" +
                "dXoiNIHbD+TbPrl0Y3Km" +
                "VXy4GYM253hQlxwp2qMKW" +
                "7zgbjgTWJzCvTcZxb2BZN" +
                "W5IdGtaWtahGkv" +
                "qW6jK5QnkU2npQjTxAN9zVHgDp4raIxXc0gY+SB1hm/7XMgD" +
                "4YHnmtlYK3WINs3gcAfC2l5B42vpSWULpCA=",
                "84df400000000000",
                if (odsCode == "") TppMockDefaults.DEFAULT_ODS_CODE_TPP else odsCode,
                "84df400000000000")
    }

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
        return NhsNumberFormatter.format(nhsNumbers.first())
    }

    companion object {

        const val defaultTelephoneFirst = "02837483567"
        const val defaultTelephoneSecond = "07737483567"
        const val defaultEmailAddress = "HalleD@fakeemail.com"

        val defaultAddress = Address(
                houseNameFlatNumber = "99",
                numberStreet = "Fake Street",
                village = "Fake village",
                town = "Fake town",
                county = "Fake county",
                postcode = "AA00 0AA"
        )

        fun getIdToken(patient: Patient): String {
            return IdTokenBuilder().getSignedToken(patient).serialize()
        }

        fun getDefault(gpSystem: String): Patient {
            return when (gpSystem.toUpperCase()) {
                "EMIS" -> EmisPatients.getDefault()
                "TPP" -> TppPatients.getDefault()
                "VISION" -> VisionPatients.getDefault()
                "MICROTEST" -> MicrotestPatients.getDefault()
                else -> throw IllegalArgumentException("$gpSystem not a valid supplier name.")
            }
        }

        fun getMicrotestPostLinkage(accountID: String, linkageKey: String): Patient {
            return MicrotestPatients.microtestPostLinkageUserDetails(accountID, linkageKey)
        }

    }
}

