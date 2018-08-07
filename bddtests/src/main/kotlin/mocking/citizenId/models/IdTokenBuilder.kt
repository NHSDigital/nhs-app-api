package mocking.citizenId.models

import com.nimbusds.jose.JOSEObjectType
import com.nimbusds.jose.JWSAlgorithm
import com.nimbusds.jose.JWSHeader
import com.nimbusds.jose.crypto.RSASSASigner
import com.nimbusds.jwt.JWTClaimsSet
import com.nimbusds.jwt.SignedJWT
import models.Patient
import java.util.*

private const val DELAY_IN_SECONDS: Int = 1008000
private const val EXPIRATION_TIME_MULTIPLIER = 1000

class IdTokenBuilder(issuer: String, audience: String) {
    private val usedIssuer: String = issuer
    private val usedAudience: String = audience

    private fun createExpirationDate(): Date {
         return Date(Date().time + DELAY_IN_SECONDS * EXPIRATION_TIME_MULTIPLIER)
    }

    private fun getHeader(): JWSHeader {
        return JWSHeader.Builder(JWSAlgorithm.RS512)
                .type(JOSEObjectType.JWT)
                .customParam("sub","3ad631b4-7a7a-434d-8a7b-1c8ac3c56132")
                .customParam("aud", usedAudience)
                .customParam("iss", usedIssuer)
                .customParam("exp", createExpirationDate().toInstant().epochSecond)
                .customParam("iat", Date().toInstant().epochSecond)
                .customParam("jti", "2581a97f-13ba-4bd5-89d4-099c70531db2")
                .build()

    }

    private fun getClaims(patient: Patient): JWTClaimsSet {

        return JWTClaimsSet.Builder()
                .subject("3ad631b4-7a7a-434d-8a7b-1c8ac3c56132")
                .issuer(usedIssuer)
                .audience(usedAudience)
                .expirationTime(createExpirationDate())
                .issueTime(Date(Date().time))
                .claim("auth_time", Date(Date().time -1 * EXPIRATION_TIME_MULTIPLIER))
                .claim("ods_code", patient.odsCode)
                .claim("email", patient.contactDetails.emailAddress)
                .claim("email_verified",true)
                .claim("birthdate", patient.dateOfBirth)
                .claim("nhs_number", patient.nhsNumbers.firstOrNull())
                .claim("im1_token", patient.connectionToken)
                .claim("id_status","verified")
                .claim("token_use","id")
                .claim("surname", patient.surname)
                .build()
    }

    fun getSignedToken(signer: RSASSASigner, patient: Patient): SignedJWT {
        var signedJWT = SignedJWT(
                getHeader(),
                getClaims(patient))

        signedJWT.sign(signer)

        return signedJWT
    }
}