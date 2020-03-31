package mocking.defaults

import constants.Supplier
import mocking.tpp.models.Application
import mocking.tpp.models.Authenticate
import models.Patient

class TppMockDefaults {
    companion object {

        const val TPP_API_VERSION = "1"
        const val DEFAULT_TPP_UUID = "af0a8175-e6c2-4c49-883e-020b2b3600f9"
        const val DEFAULT_TPP_PROVIDER_ID = "b891fc3b51d5e7c1"
        const val DEFAULT_ODS_CODE_TPP: String = "A82648"
        const val TPP_ODS_CODE_NO_SJR_CONFIGURATION = "A33333"
        const val DEFAULT_TPP_SESSION_ID = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        const val DEFAULT_TPP_ACCOUNT_ID = "520993083"
        const val DEFAULT_TPP_PASSPHRASE = "c2axhQ9VWB2/62XFxvKrNKh9JwgLk0NFY15h" +
                "IdI6aRytptqiBs6r/k+0OvGEZfcEdMLJEMp/J4pkOGm2ViaSLca49ODQzz4y+Cu" +
                "2xOxLaehq/SjEIwflsWeSwCvCAxroId1bXejTdNsV17fOAD0M5nAZF6X9TysOfRR/j5tuR+o="

        const val DEFAULT_TPP_CONNECTION_TOKEN =
                "{\"AccountId\":\"$DEFAULT_TPP_ACCOUNT_ID\"" +
                        ",\"Passphrase\":\"$DEFAULT_TPP_PASSPHRASE\"" +
                        ",\"ProviderId\":\"$DEFAULT_TPP_PROVIDER_ID\"}"

        val patientTpp = Patient.getDefault(Supplier.TPP)

        val DEFAULT_TPP_APPLICATION = Application(
                name = "NhsApp",
                version = "1.0",
                providerId = DEFAULT_TPP_PROVIDER_ID,
                deviceType = "NhsApp"
        )

        val tppAuthenticateRequest = Authenticate(
                apiVersion = TPP_API_VERSION,
                accountId = patientTpp.accountId,
                passphrase = patientTpp.passphrase,
                unitId = DEFAULT_ODS_CODE_TPP,
                uuid = DEFAULT_TPP_UUID,
                application = DEFAULT_TPP_APPLICATION
        )

        const val ODS_CODE_SJR_LINKED_ACCOUNT_ECONSULT: String = "A20001"
        const val ODS_CODE_SJR_LINKED_ACCOUNT_IM1: String  = "A20002"
        const val ODS_CODE_SJR_LINKED_ACCOUNT_INFORMATICA: String  = "A20003"
        const val ODS_CODE_SJR_LINKED_ACCOUNT_GP_AT_HAND: String  = "A20004"
    }
}