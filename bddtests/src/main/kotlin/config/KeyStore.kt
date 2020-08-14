package config

import com.nimbusds.jose.crypto.RSASSASigner
import com.nimbusds.jose.crypto.RSASSAVerifier
import com.nimbusds.jose.jwk.RSAKey
import java.security.interfaces.RSAPrivateKey

class KeyStore(keys: String) {

    var publicJwk: RSAKey
    var privateKey: RSAPrivateKey
    var signer: RSASSASigner
    var verifier: RSASSAVerifier

    init {
        val jwk = RSAKey.parse(keys)
        publicJwk = jwk.toPublicJWK()
        privateKey = jwk.toRSAPrivateKey()
        signer = RSASSASigner(privateKey)
        verifier = RSASSAVerifier(jwk.toRSAPublicKey())
    }
}
