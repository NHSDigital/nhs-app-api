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
    protected var jwtId = "2581a97f-13ba-4bd5-89d4-099c70531db2"

    fun getSignedToken(patient: Patient, expirationTimeOverride: Date? = null): SignedJWT {
        val signedJWT = SignedJWT(
                getHeader(patient),
                getClaims(patient, expirationTimeOverride = expirationTimeOverride))

        signedJWT.sign(Config.keyStore.signer)

        return signedJWT
    }

    abstract fun getClaims(patient: Patient,
                           issuerOverride: String? = null,
                           audienceOverride: String? = null,
                           expirationTimeOverride: Date? = null): JWTClaimsSet

    private fun getHeader(patient: Patient): JWSHeader {
        val headerParams = createHeaderParams(patient)
        val builder = JWSHeader.Builder(JWSAlgorithm.RS512).type(JOSEObjectType.JWT)
                .keyID("b714986ac526ea126555a37f168659fce8b9db24")
        headerParams.forEach { headerParam -> builder.customParam(headerParam.key, headerParam.value) }
        return builder.build()
    }

    private fun createHeaderParams(patient: Patient): Map<String, String> {
        return mapOf(
                "sub" to patient.subject,
                "aud" to audience,
                "iss" to issuer,
                "exp" to createExpirationDate().toInstant().epochSecond.toString(),
                "iat" to Date().toInstant().epochSecond.toString(),
                "jti" to jwtId)
    }

    protected fun createExpirationDate(): Date {
        return Date(Date().time + DELAY_IN_SECONDS * EXPIRATION_TIME_MULTIPLIER)
    }

    private fun createExpirationDateInPast(): Date {
        return Date(Date().time - DELAY_IN_SECONDS * EXPIRATION_TIME_MULTIPLIER)
    }

    protected fun createAuthTime(): Date {
        return Date(Date().time - 1 * EXPIRATION_TIME_MULTIPLIER)
    }

    fun getInvalidTokens(patient: Patient): Array<Pair<SignedJWT, String>> {
        return arrayOf(
                createInvalidPair("aud", patient, audienceOverride = ""),
                createInvalidPair("iss", patient, issuerOverride = ""),
                createInvalidPair("exp", patient, expirationTimeOverride = createExpirationDateInPast())
        )
    }

    private fun createInvalidPair(key: String,
                                  patient: Patient,
                                  issuerOverride: String? = null,
                                  audienceOverride: String? = null,
                                  expirationTimeOverride: Date? = null): Pair<SignedJWT, String> {
        val invalidToken = SignedJWT(
                getHeader(patient),
                getClaims(patient, issuerOverride, audienceOverride, expirationTimeOverride))
        invalidToken.sign(Config.keyStore.signer)
        return Pair(invalidToken, key)
    }
}