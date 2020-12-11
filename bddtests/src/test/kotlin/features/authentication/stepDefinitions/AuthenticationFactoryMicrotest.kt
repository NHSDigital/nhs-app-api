package features.authentication.stepDefinitions

import constants.Supplier
import models.Patient
import java.time.Duration

class AuthenticationFactoryMicrotest : AuthenticationFactory(Supplier.MICROTEST) {

    override fun validOAuthDetailsAndGpSystemUnavailable(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithServiceUnavailable()
        }
    }

    override fun validOAuthDetailsAndGpSystemBadGateway() {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithBadGateway()
        }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithSuccess()
                    .delayedBy(Duration.ofSeconds(delayBySeconds))
        }
    }

    override fun validOAuthDetailsAndGpSystemReturnsError() {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithCorruptedContent()
        }
    }

    override fun patientDoesNotExist(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithIncorrectLinkageKey(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithIncorrectSurname(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithIncorrectDOB(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithSurnameInWrongFormat(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithAccountIDInWrongFormat(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithLinkageKeyInWrongFormat(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithDOBInWrongFormat(patient: Patient) {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun patientWithIncompleteResponse(patient: Patient) {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithCorruptedContent()
        }
    }
}
