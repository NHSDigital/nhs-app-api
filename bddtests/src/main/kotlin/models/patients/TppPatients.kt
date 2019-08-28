package models.patients

import mocking.defaults.TppMockDefaults
import mocking.emis.demographics.Address
import mocking.emis.demographics.Sex
import models.Patient
import worker.models.demographics.TppUserSession
import worker.models.patient.Im1ConnectionToken

class TppPatients{

    companion object {

        fun getDefault(): Patient {
            return kevinBarry
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
                im1ConnectionToken = kevinBarryIm1ConnectionToken
        )
    }
}