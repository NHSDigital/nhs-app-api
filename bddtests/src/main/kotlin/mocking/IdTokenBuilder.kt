package mocking

import com.nimbusds.jwt.JWTClaimsSet
import models.Patient
import worker.models.patient.Im1ConnectionToken
import java.util.*

class IdTokenBuilder: JWTBuilder(){

    override fun getClaims(patient: Patient,
                           issuerOverride: String?,
                           audienceOverride: String?,
                           expirationTimeOverride: Date?): JWTClaimsSet {
        var im1ConnectionToken = patient.im1ConnectionToken ?: patient.connectionToken
        if (im1ConnectionToken.javaClass == Im1ConnectionToken::class.java) {
            im1ConnectionToken = GsonFactory.asPascal.toJson(im1ConnectionToken)
        }

        return JWTClaimsSet.Builder()
                .subject(patient.subject)
                .issuer(issuer)
                .audience(audience)
                .expirationTime(createExpirationDate())
                .issueTime(Date(Date().time))
                .claim("auth_time", createAuthTime())
                .claim("ods_code", patient.odsCode)
                .claim("email", patient.contactDetails.emailAddress)
                .claim("email_verified", true)
                .claim("birthdate", patient.age.dateOfBirth)
                .claim("nhs_number", patient.nhsNumbers.firstOrNull())
                .claim("im1_token", im1ConnectionToken)
                .claim("id_status", "verified")
                .claim("token_use", "id")
                .claim("surname", patient.name.surname)
                .claim("jti", jwtId)
                .build()
    }
}
