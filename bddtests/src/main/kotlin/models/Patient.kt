package models

import constants.DateTimeFormats
import constants.Supplier
import mocking.AccessTokenBuilder
import mocking.IdTokenBuilder
import mocking.defaults.EmisMockDefaults
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
import java.time.LocalDate
import java.time.format.DateTimeFormatter
import java.time.temporal.ChronoUnit

data class Patient(
        val title: String = "",
        var ageMonths: Int = 0,
        var ageYears: Int = 0,
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
        var sessionId: String = "",
        var endUserSessionId: String = "",
        val linkageKey: String = "",
        var userPatientLinkToken: String = "",
        val nhsNumbers: List<String> = emptyList(),
        val patientId: String = "",
        val passphrase: String = "",
        var onlineUserId: String = "",
        val rosuAccountId: String = "",
        val apiKey: String = "",
        val authCode: String = "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPS" +
                "ETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-76" +
                "57b5e9e403.8d4c0a21-6483-4a52-9d47" +
                "-6bcd737c634e",
        val codeVerifier: String = "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
        var subject: String = "3ad631b4-7a7a-434d-8a7b-1c8ac3c56132",
        val im1ConnectionToken: Im1ConnectionToken? = null,
        val organDonationRegistrationId: String = "AD02745157",
        var linkedAccounts: Set<Patient> = setOf(),
        val identityProofingLevel: IdentityProofingLevel = IdentityProofingLevel.P9
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
                patientId,
                if (odsCode == "") TppMockDefaults.DEFAULT_ODS_CODE_TPP else odsCode,
                onlineUserId)
    }

    fun generateUserSessionRequest(redirectUrl: String): UserSessionRequest {
        return UserSessionRequest(
                authCode = this.authCode,
                codeVerifier = this.codeVerifier,
                redirectUrl = redirectUrl)
    }

    fun formattedDateOfBirth(): String {
        return DateConverter.convertDateToDateTimeFormat(
                dateOfBirth,
                DateTimeFormats.dateWithoutTimeFormat,
                DateTimeFormats.frontendBasicDateFormat)
    }

    fun formattedAge(): String {
        var formattedAge = ""
        if (ageYears == 0 && ageMonths == 0) {
            formattedAge = "Less than one month old"
        }
        if (ageYears == 0 && ageMonths == 1) {
            formattedAge = ageMonths.toString() + " month old"
        }
        if (ageYears == 0 && ageMonths > 1) {
            formattedAge = ageMonths.toString() + " months old"
        }
        if (ageYears == 1 && ageMonths >= 0) {
            formattedAge = ageYears.toString() + " year old"
        }
        if (ageYears > 1 && ageMonths >= 0) {
            formattedAge = ageYears.toString() + " years old"
        }

        return formattedAge
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
        const val monthsInYear = 12

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

        fun getDefault(gpSystem: Supplier): Patient {
            return when (gpSystem) {
                Supplier.EMIS -> EmisPatients.getDefault()
                Supplier.TPP -> TppPatients.getDefault()
                Supplier.VISION -> VisionPatients.getDefault()
                Supplier.MICROTEST -> MicrotestPatients.getDefault()
            }
        }

        fun getPatientWithLinkedProfiles(gpSystem: Supplier): Patient {
            return when (gpSystem) {
                Supplier.EMIS -> EmisPatients.getPatientWithLinkedProfiles()
                Supplier.TPP -> TppPatients.getPatientWithLinkedProfiles()
                else -> throw IllegalArgumentException("$gpSystem not a valid supplier name.")
            }
        }

        fun getPatientWithNoLinkedProfiles(gpSystem: Supplier): Patient {
            return when (gpSystem) {
                Supplier.EMIS -> EmisPatients.getPatientWithNoLinkedProfiles()
                else -> throw IllegalArgumentException("$gpSystem not a valid supplier name.")
            }
        }

        fun getMicrotestPostLinkage(accountID: String, linkageKey: String): Patient {
            return MicrotestPatients.microtestPostLinkageUserDetails(accountID, linkageKey)
        }

        fun setOdsCodeBasedOnAppointmentsProvider(supplier: Supplier, patient: Patient, provider: String) {
            return when (supplier) {
                Supplier.TPP -> setTppOdsCode(patient, provider)
                Supplier.EMIS -> setEmisOdsCode(patient, provider)
                else -> throw IllegalArgumentException("$provider not a valid appointment provider name.")
            }
        }

        fun setTppOdsCode(patient: Patient, provider: String) {
            return when (provider.toUpperCase()) {
                "ECONSULT" ->  {
                    updateOdsCodes(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT)
                    updateUnitId(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT)
                }
                "IM1" -> {
                    updateOdsCodes(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_IM1)
                    updateUnitId(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_IM1)
                }
                "INFORMATICA" -> {
                    updateOdsCodes(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA)
                    updateUnitId(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA)
                }
                "GPATHAND" -> {
                    updateOdsCodes(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND)
                    updateUnitId(patient, TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND)
                }
                else -> throw IllegalArgumentException("$provider not a valid appointment provider name.")
            }
        }

        fun setEmisOdsCode(patient: Patient, provider: String) {
            return when (provider.toUpperCase()) {
                "ECONSULT" -> updateOdsCodes(patient, EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT)
                "IM1" -> updateOdsCodes(patient, EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_IM1)
                "INFORMATICA" -> updateOdsCodes(patient, EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA)
                "GPATHAND" -> updateOdsCodes(patient, EmisMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND)
                else -> throw IllegalArgumentException("$provider not a valid appointment provider name.")
            }
        }

        fun updateOdsCodes(patient: Patient, odsCode: String) {
            patient.odsCode = odsCode
            patient.linkedAccounts.forEach {e -> e.odsCode = odsCode }
        }

        private fun updateUnitId(patient: Patient, odsCode: String) {
            patient.tppUserSession!!.unitId = odsCode
            patient.linkedAccounts.forEach {e -> e.tppUserSession!!.unitId = odsCode }
        }

        fun getAgePart(dob: String, unit: ChronoUnit): Int {
            val dateOfBirth = LocalDate.parse(dob, DateTimeFormatter.ISO_DATE)
            return when (unit) {
                ChronoUnit.YEARS -> unit.between(dateOfBirth, LocalDate.now()).toInt()
                ChronoUnit.MONTHS -> (unit.between(dateOfBirth, LocalDate.now()) % monthsInYear) .toInt()
                else -> throw IllegalArgumentException("Only years or months are supported")
            }
        }
    }
}

