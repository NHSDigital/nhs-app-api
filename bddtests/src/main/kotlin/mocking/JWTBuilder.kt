package mocking

import com.nimbusds.jose.JOSEObjectType
import com.nimbusds.jose.JWSAlgorithm
import com.nimbusds.jose.JWSHeader
import com.nimbusds.jwt.JWTClaimsSet
import com.nimbusds.jwt.SignedJWT
import config.Config
import models.Patient
import java.util.*

private const val DELAY_IN_SECONDS: Int = 1008000
private const val EXPIRATION_TIME_MULTIPLIER = 1000

abstract class JWTBuilder {
    protected var issuer = Config.instance.cidJwtIssuer
    protected var audience = Config.instance.cidClientId

    abstract fun getClaims(patient: Patient): JWTClaimsSet

    fun getSignedToken(patient: Patient): SignedJWT {
        val signedJWT = SignedJWT(
                getHeader(patient),
                getClaims(patient))

        signedJWT.sign(Config.keyStore.signer)

        return signedJWT
    }

    private fun getHeader(patient: Patient): JWSHeader {
        return JWSHeader.Builder(JWSAlgorithm.RS512)
                .type(JOSEObjectType.JWT)
                .customParam("sub", patient.subject)
                .customParam("aud", audience)
                .customParam("iss", issuer)
                .customParam("exp", createExpirationDate().toInstant().epochSecond)
                .customParam("iat", Date().toInstant().epochSecond)
                .customParam("jti", "2581a97f-13ba-4bd5-89d4-099c70531db2")
                .build()
    }

    protected fun createExpirationDate(): Date {
        return Date(Date().time + DELAY_IN_SECONDS * EXPIRATION_TIME_MULTIPLIER)
    }

    protected fun createAuthTime(): Date {
        return Date(Date().time - 1 * EXPIRATION_TIME_MULTIPLIER)
    }
}