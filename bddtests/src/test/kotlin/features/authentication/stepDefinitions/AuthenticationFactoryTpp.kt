package features.authentication.stepDefinitions

import mocking.defaults.TppMockDefaults
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import java.time.Duration

class AuthenticationFactoryTpp  : AuthenticationFactory("TPP"){

    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient.forTpp {
            authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
                    .respondWithSuccess(AuthenticateReply())
                    .delayedBy(Duration.ofSeconds(31))
        }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(errorCode = "9", userFriendlyMessage = "There was a problem logging on"))

        }}

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(errorCode = "0", userFriendlyMessage = "Service Unavailable"))
        }
    }
}