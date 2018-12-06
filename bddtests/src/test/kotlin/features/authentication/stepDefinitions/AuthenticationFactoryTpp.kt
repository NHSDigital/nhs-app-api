package features.authentication.stepDefinitions

import constants.ErrorResponseCodeTpp
import mocking.defaults.TppMockDefaults
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import models.Patient
import java.time.Duration

private const val DELAY_BY_SECONDS = 31L

class AuthenticationFactoryTpp : AuthenticationFactory("TPP") {

    override fun patientWithIncompleteResponse(patient: Patient) {
        val response = AuthenticateReply()
        response.onlineUserId = ""
        response.patientId = ""

        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    .respondWithSuccess(response)
        }
    }

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
                    .delayedBy(Duration.ofSeconds(DELAY_BY_SECONDS))
        }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(
                            errorCode = ErrorResponseCodeTpp.LOGIN_PROBLEM,
                            userFriendlyMessage = "There was a problem logging on"))

        }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forTpp {
            authentication.authenticateRequest(Authenticate())
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(
                            errorCode = ErrorResponseCodeTpp.SERVICE_UNAVAILABLE,
                            userFriendlyMessage = "Service Unavailable"))
        }
    }

    private fun createInvalidLinkageTest(patient: Patient) {
        mockingClient.forTpp {
            authentication.linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
        }
    }
}
