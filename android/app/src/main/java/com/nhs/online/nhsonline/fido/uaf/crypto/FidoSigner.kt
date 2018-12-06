package com.nhs.online.nhsonline.fido.uaf.crypto

import java.security.KeyPair

interface FidoSigner {

    fun sign(dataToSign: ByteArray, keyPair: KeyPair?): ByteArray
}
