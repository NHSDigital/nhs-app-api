package features.authentication.stepDefinitions

import mocking.defaults.MockDefaults
import java.time.Duration

class AuthenticationFactoryVision  : AuthenticationFactory("VISION"){

    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient
                .forVision {
                    getConfigurationRequest(MockDefaults.visionUserSession, MockDefaults.visionGetConfiguration)
                            .respondWithSuccess(MockDefaults.visionConfigurationResponse).delayedBy(Duration.ofSeconds(31))
                }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient
                .forVision {
                    getConfigurationRequest(MockDefaults.visionUserSession, MockDefaults.visionGetConfiguration)
                            .respondWitInvalidUserCredentials()
                }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forVision {
            getConfigurationRequest(
                    MockDefaults.visionUserSession,
                    MockDefaults.visionGetConfiguration)
                    .respondWithServiceUnavailable()
        }
    }
}