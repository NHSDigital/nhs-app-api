package com.nhs.online.nhsonline.biometrics.utils

import com.nhs.online.fidoclient.uaf.crypto.Base64url
import com.nhs.online.fidoclient.uaf.crypto.KeyCodec
import java.security.PublicKey

object BiometricMockTestData {

    fun getUAFMessage(): String {
        return """{"uafProtocolMessage":"[{\"fcParams\":\"eyJhcHBJRCI6IiIsImZhY2V0SUQiOiIifQ\",\"assertions\":[{\"assertionScheme\":\"UAFV1TLV\"}],\"header\":{\"appID\":\"\",\"serverData\":\"\"}}]"}"""
    }

    fun getSignature(): String {
        return "308201dd30820146020101300d06092a864886f70d010105050030373116301406035504030c0d416e64726f69642044656275673110300e060355040a0c07416e64726f6964310b3009060355040613025553301e170d3138303733313038323930365a170d3438303732333038323930365a30373116301406035504030c0d416e64726f69642044656275673110300e060355040a0c07416e64726f6964310b300906035504061302555330819f300d06092a864886f70d010101050003818d0030818902818100d68bb09bc83dcf88ef5d4120d753e2df881ba938358b865206380c40b5dff779ffa51e7244fb74edbfbff7e44cc4485849280d5c7299a872592ccacdf4daa1e09e0200ad74acbe4858ff320906034ef21c0fd467c71c0a0b1cb39ea58700d54f2b4976f2fbae6c381cea85d9379a825c70c139dbfe9daf25013407fc9e50f1bb0203010001300d06092a864886f70d010105050003818100a1cdabb8310ef0dac7cc688f1fc6f4de1d25c3b666c0f70211f836629603ea7241a458c9506bfd4677c7a2de67f38f5259dbb36ad4094154451985fe6fa00e7ac9c929b4762bb855ddbf245fd898051987de32feee42c6e586914d26854a0c5b1431302074e2c31075e2e8979b3c35b5daa664edd200ea82bd3bfed1c0568df2"
    }

    fun getKey(): PublicKey? {
        return KeyCodec.getPubKey(Base64url.decode(getPublicCert()))
    }

    fun getTestRegResponse(): String {
        return """[{"status": "success", "attestVerifiedStatus": "valid"}]""""
    }

    fun getAndroidHash(): String {
        return "android:apk-key-hash:fjyfTEjogJ4N2KzB7+gfe1F/riw"
    }

    private fun getPublicCert(): String {
        return "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEPIcOZR01F72d0s2PWq4GzjgQ9qZ-cwDOJm0ocEMgi-W0bJEy9x1T1j0MamFTPt6SbSSC2KjlrCDHeUW_4fA_hA"
    }
}