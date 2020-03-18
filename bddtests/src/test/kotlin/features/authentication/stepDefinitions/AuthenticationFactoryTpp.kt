package features.authentication.stepDefinitions

import constants.ErrorResponseCodeTpp
import constants.Supplier
import mocking.defaults.TppMockDefaults
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import mocking.tpp.models.Person
import models.Patient
import java.time.Duration

class AuthenticationFactoryTpp : AuthenticationFactory(Supplier.TPP) {

    override fun patientWithIncompleteResponse(patient: Patient) {
        val response = AuthenticateReply()
        response.onlineUserId = ""
        response.patientId = ""
        response.person =  mutableListOf<Person>()

        mockingClient.forTpp {
            authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
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

    override fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long) {
        mockingClient.forTpp {
            authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
                    .respondWithSuccess(AuthenticateReply())
                    .delayedBy(Duration.ofSeconds(delayBySeconds))
        }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forTpp {
            authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
                    // respond with error.  Unconfirmed format.
                    .respondWithError(Error(
                            errorCode = ErrorResponseCodeTpp.LOGIN_PROBLEM,
                            userFriendlyMessage = "There was a problem logging on"))

        }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forTpp {
            authentication.authenticateRequest(TppMockDefaults.tppAuthenticateRequest)
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
