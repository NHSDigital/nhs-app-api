package models

import constants.Supplier
import mocking.AccessTokenBuilder
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Sex
import models.patients.PatientHandler
import worker.models.demographics.TppUserSession
import worker.models.patient.Im1ConnectionToken

data class Patient(
        var name: PatientName = PatientName(),
        var age : PatientAge = PatientAge("1972-04-12"),
        var contactDetails: PatientContactDetails = PatientContactDetails(),
        val sex: Sex = Sex.NotSpecified,
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
        val linkedAccounts: Set<Patient> = setOf(),
        val identityProofingLevel: IdentityProofingLevel = IdentityProofingLevel.P9) {
    var accessToken: String = AccessTokenBuilder().getSignedToken(this).serialize()
    var refreshToken: String? = null

    val tppUserSession: TppUserSession? by lazy {
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

    fun formattedNHSNumber(): String {
        return NhsNumberFormatter.format(nhsNumbers.first())
    }

    fun formattedFullName(includeTitle: Boolean = true): String {
        return name.formattedFullName(identityProofingLevel, includeTitle)
    }

    fun updateOdsCodes(targetOdsCode: String) {
        odsCode = targetOdsCode
        linkedAccounts.forEach { e -> e.odsCode = targetOdsCode }
    }

    companion object {
        fun getDefault(gpSystem: Supplier): Patient {
            return PatientHandler.getForSupplier(gpSystem).getDefault()
        }
    }
}
