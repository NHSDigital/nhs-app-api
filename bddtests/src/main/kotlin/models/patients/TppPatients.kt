package models.patients

import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import worker.models.patient.Im1ConnectionToken
import java.time.temporal.ChronoUnit

class TppPatients{

    companion object {

        fun getDefault(): Patient {
            return kevinBarry
        }

        fun getPatientWithLinkedProfiles(): Patient {
            // TODO - see if anythine else needs to be done as the tpp proxy functionality grows

            lisaHankey.tppUserSession!!.onlineUserId = lisaHankey.onlineUserId
            lisaHankey.tppUserSession!!.patientId = lisaHankey.patientId

            lisaHankey.linkedAccounts.forEach {
                linkedAccount ->
                linkedAccount.tppUserSession!!.patientId = linkedAccount.patientId
                linkedAccount.tppUserSession!!.onlineUserId = linkedAccount.onlineUserId
            }

            return lisaHankey
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
                title = "Mr",
                firstName = "Kevin",
                surname = "Barry",
                dateOfBirth = "1985-05-29",
                ageMonths = Patient.getAgePart("1985-05-29", ChronoUnit.MONTHS),
                ageYears = Patient.getAgePart("1985-05-29", ChronoUnit.YEARS),
                sex = Sex.Male,
                address = Address(
                        houseNameFlatNumber = "28 Central Path",
                        numberStreet = "Troy Road",
                        village = "Horsforth",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
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
                title = "Mr",
                firstName = "Joe",
                surname = "Wicks",
                dateOfBirth = "1980-01-01",
                ageMonths = Patient.getAgePart("1980-01-01", ChronoUnit.MONTHS),
                ageYears = Patient.getAgePart("1980-01-01", ChronoUnit.YEARS),
                sex = Sex.Male,
                address = Address(
                        houseNameFlatNumber = "100 High Street",
                        numberStreet = "Get Fit Road",
                        village = "Fitstown",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
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
                title = "Miss",
                firstName = "Lisa",
                surname = "Hankey",
                dateOfBirth = "1985-05-29",
                ageMonths = Patient.getAgePart("1985-05-29", ChronoUnit.MONTHS),
                ageYears = Patient.getAgePart("1985-05-29", ChronoUnit.YEARS),
                sex = Sex.Female,
                address = Address(
                        houseNameFlatNumber = "28 Kleenex Road",
                        numberStreet = "Tissue Avenue",
                        village = "Cushelle",
                        town = "Leeds",
                        county = "West Yorkshire",
                        postcode = "LS18 5TN"
                ),
                telephoneFirst = Patient.defaultTelephoneFirst,
                telephoneSecond = Patient.defaultTelephoneSecond,
                emailAddress = Patient.defaultEmailAddress,
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
                linkedAccounts = setOf(joeWicks, kevinBarry)
        )
    }
}