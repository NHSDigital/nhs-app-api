package models.patients

import MockDefaults
import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import models.PatientAge
import models.PatientContactDetails
import models.PatientName
import worker.models.patient.Im1ConnectionToken

class TppPatients {

    companion object : PatientHandler() {

        override fun getDefault(): Patient {
            return kevinBarry
        }

        override fun getPatientWithLinkedProfiles(): Patient {
            lisaHankey.linkedAccounts.forEach { linkedAccount ->
                linkedAccount.tppUserSession!!.onlineUserId = lisaHankey.onlineUserId
                linkedAccount.onlineUserId = lisaHankey.onlineUserId
            }
            return lisaHankey
        }

        override fun getPatientWithNoLinkedProfiles(): Patient {
            return kevinBarry
        }

        override fun setOdsCode(patient: Patient, provider: String) {
            val targetOdsCode = when (provider.toUpperCase()) {
                "ECONSULT" -> TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT
                "IM1" -> TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_IM1
                "INFORMATICA" -> TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA
                "GPATHAND" -> TppMockDefaults.ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND
                else -> throw IllegalArgumentException("$provider not a valid appointment provider name.")
            }
            patient.updateOdsCodes(targetOdsCode)
            updateUnitId(patient, targetOdsCode)
        }

        private val kevinBarryIm1ConnectionToken = Im1ConnectionToken(
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

        private val lisaHankeyIm1ConnectionToken = Im1ConnectionToken(
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


        private val joeWicksIm1ConnectionToken = Im1ConnectionToken(
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
                name = PatientName(title = "Mr",
                        firstName = "Kevin",
                        surname = "Barry"),
                age = PatientAge(dateOfBirth = "1985-05-29"),
                sex = Sex.Male,
                contactDetails = PatientContactDetails(
                        address = Address(
                                houseNameFlatNumber = "28 Central Path",
                                numberStreet = "Troy Road",
                                village = "Horsforth",
                                town = "Leeds",
                                county = "West Yorkshire",
                                postcode = "LS18 5TN"
                        )),
                odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers = listOf("5785445875"),
                linkageKey = "passphraseToLink",
                accountId = kevinBarryIm1ConnectionToken.accountId!!,
                patientId = "84df400000000000",
                onlineUserId = "84df400000000000",
                passphrase = kevinBarryIm1ConnectionToken.passphrase!!,
                connectionToken = TppMockDefaults.DEFAULT_TPP_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                im1ConnectionToken = kevinBarryIm1ConnectionToken
        )

        val joeWicks = Patient(
                name = PatientName(
                        title = "Mr",
                        firstName = "Joe",
                        surname = "Wicks"),
                age = PatientAge(dateOfBirth = "1980-01-01"),
                sex = Sex.Male,
                contactDetails = PatientContactDetails(
                        address = Address(
                                houseNameFlatNumber = "100 High Street",
                                numberStreet = "Get Fit Road",
                                village = "Fitstown",
                                town = "Leeds",
                                county = "West Yorkshire",
                                postcode = "LS18 5TN"
                        )),
                odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers = listOf("5785445875"),
                linkageKey = "passphraseToLink",
                accountId = joeWicksIm1ConnectionToken.accountId!!,
                patientId = "84df400000000001",
                onlineUserId = "84df400000000001",
                passphrase = joeWicksIm1ConnectionToken.passphrase!!,
                connectionToken = TppMockDefaults.DEFAULT_TPP_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                im1ConnectionToken = joeWicksIm1ConnectionToken
        )

        val lisaHankey = Patient(
                name = PatientName(
                        title = "Miss",
                        firstName = "Lisa",
                        surname = "Hankey"),
                age = PatientAge(dateOfBirth = "1985-05-29"),
                sex = Sex.Female,
                contactDetails = PatientContactDetails(
                        address = Address(
                                houseNameFlatNumber = "28 Kleenex Road",
                                numberStreet = "Tissue Avenue",
                                village = "Cushelle",
                                town = "Leeds",
                                county = "West Yorkshire",
                                postcode = "LS18 5TN"
                        )),
                odsCode = TppMockDefaults.DEFAULT_ODS_CODE_TPP,
                nhsNumbers = listOf("5785445875"),
                linkageKey = "passphraseToLink",
                accountId = lisaHankeyIm1ConnectionToken.accountId!!,
                patientId = "84df400000000002",
                onlineUserId = "84df400000000002",
                passphrase = lisaHankeyIm1ConnectionToken.passphrase!!,
                connectionToken = TppMockDefaults.DEFAULT_TPP_CONNECTION_TOKEN,
                endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID,
                im1ConnectionToken = lisaHankeyIm1ConnectionToken,
                linkedAccounts = setOf(joeWicks.copy(), kevinBarry.copy())
        )

        private fun updateUnitId(patient: Patient, odsCode: String) {
            patient.tppUserSession!!.unitId = odsCode
            patient.linkedAccounts.forEach { e -> e.tppUserSession!!.unitId = odsCode }
        }
    }
}
