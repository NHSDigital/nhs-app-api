package features.authentication.stepDefinitions

import java.time.Duration

class AuthenticationFactoryEmis  : AuthenticationFactory("EMIS"){
    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(31)) }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithForbidden() }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithServiceUnavailable() }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithSuccess(patient, associationType) }

    }
}