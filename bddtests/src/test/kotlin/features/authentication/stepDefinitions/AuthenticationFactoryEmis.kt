package features.authentication.stepDefinitions

import mocking.emis.me.EmisMeApplicationsBuilder

import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.models.Mapping
import models.Patient
import java.time.Duration

private const val DELAY_BY_SECONDS = 31L

class AuthenticationFactoryEmis  : AuthenticationFactory("EMIS"){

    override fun patientDoesNotExist(patient: Patient) {
     createInvalidLinkageTest(patient) { respondWithNoOnlineUserFound() }
    }

    override fun patientWithIncorrectLinkageKey(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithNoOnlineUserFound() }
    }

    override fun patientWithIncorrectSurname(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithIncorrectSurnameOrDateOfBirth() }
    }

    override fun patientWithIncorrectDOB(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithIncorrectSurnameOrDateOfBirth() }
    }

    override fun patientWithSurnameInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithBadRequest("The request is invalid.",
                "Surname") }
    }

    override fun patientWithAccountIDInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithBadRequest("The request is invalid.",
                "LinkageDetails.AccountId") }
    }

    override fun patientWithLinkageKeyInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithBadRequest("The request is invalid.",
                "LinkageDetails.LinkageKey") }
    }

    override fun patientWithDOBInWrongFormat(patient: Patient) {
        createInvalidLinkageTest(patient) { respondWithIncorrectSurnameOrDateOfBirth() }
    }

    override fun validOAuthDetailsAndGpSystemSlowToRespond() {
        mockingClient.forEmis { authentication.endUserSessionRequest()
                .respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(DELAY_BY_SECONDS)) }
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

    private fun createInvalidLinkageTest( patient: Patient, emisResponse: (EmisMeApplicationsBuilder.() -> Mapping)) {
        mockingClient.forEmis { emisResponse(authentication
                .meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))) }
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
    }

    private fun createLinkApplicationRequestModel(patient: Patient): LinkApplicationRequestModel {
        return LinkApplicationRequestModel(
                surname = patient.surname,
                dateOfBirth = patient.dateOfBirth.plus("T00:00:00"),
                linkageDetails = LinkageDetailsModel(
                        accountId = patient.accountId,
                        nationalPracticeCode = patient.odsCode,
                        linkageKey = patient.linkageKey
                )
        )
    }
}
