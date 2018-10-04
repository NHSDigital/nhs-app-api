package features.authentication.stepDefinitions

import mocking.defaults.TppMockDefaults
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import models.Patient
import java.time.Duration

class AuthenticationFactoryTpp  : AuthenticationFactory("TPP"){

    override fun patientDoesNotExist(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithIncorrectLinkageKey(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithIncorrectSurname(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithIncorrectDOB(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithSurnameInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithAccountIDInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithLinkageKeyInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

    override fun patientWithDOBInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient)
    }

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

        }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(errorCode = "0", userFriendlyMessage = "Service Unavailable"))
        }
    }

    private fun createInvalidLinkageTest( patient: Patient) {
        mockingClient.forTpp {
            authentication.linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
        }
    }
}
