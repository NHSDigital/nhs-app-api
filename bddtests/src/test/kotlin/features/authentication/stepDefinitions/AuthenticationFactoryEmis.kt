package features.authentication.stepDefinitions

import java.time.Duration

class AuthenticationFactoryEmis  : AuthenticationFactory("EMIS"){
    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(31)) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithForbidden() }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forEmis { endUserSessionRequest().respondWithServiceUnavailable() }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, associationType) }

    }
}