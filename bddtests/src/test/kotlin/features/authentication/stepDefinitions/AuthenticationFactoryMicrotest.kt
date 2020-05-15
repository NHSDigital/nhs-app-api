package features.authentication.stepDefinitions

import constants.Supplier
import models.Patient

class AuthenticationFactoryMicrotest : AuthenticationFactory(Supplier.MICROTEST) {

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forMicrotest.mock {
            demographics.demographicsRequest(patient).respondWithServiceUnavailable()
        }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        throw NotImplementedError("Not implemented for Microtest")
    }

    override fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long) {
        throw NotImplementedError("Not implemented for Microtest")
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