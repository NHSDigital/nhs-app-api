package mocking

import com.nimbusds.jwt.JWTClaimsSet
import models.Patient
import java.util.*

class AccessTokenBuilder: JWTBuilder(){
    override fun getClaims(patient: Patient): JWTClaimsSet {
        return JWTClaimsSet.Builder()
                .subject(patient.subject)
                .issuer(issuer)
                .audience(audience)
                .expirationTime(createExpirationDate())
                .issueTime(Date(Date().time))
                .claim("nhs_number", patient.nhsNumbers.firstOrNull())
                .claim("version", 0)
                .claim("vtm", "https://auth.ext.signin.nhs.uk/trustmark/auth.ext.signin.nhs.uk")
                .claim("client_id", "nhs-online")
                .claim("requesting_patient", patient.nhsNumbers.firstOrNull())
                .claim("token_use", "access")
                .claim("auth_time", createAuthTime())
                .claim("scope", "openid profile nhs_app_credentials gp_integration_credentials")
                .claim("vot", "P9.Cp.Cd")
                .claim("reason_for_request", "patientaccess")
                .claim("jti", UUID.randomUUID())
                .build()
    }
}