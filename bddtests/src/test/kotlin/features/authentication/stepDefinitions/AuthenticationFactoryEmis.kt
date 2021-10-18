package features.authentication.stepDefinitions

import constants.Supplier
import mocking.emis.me.EmisMeApplicationsBuilder
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.practices.SettingsResponseModel
import mocking.models.Mapping
import models.Patient
import java.time.Duration

class AuthenticationFactoryEmis  : AuthenticationFactory(Supplier.EMIS){

    override fun patientWithIncompleteResponse(patient: Patient) {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest()
                .respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis.mock {
            authentication.sessionRequest(patient).respondWithCorruptedContent()
        }
    }

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

    override fun validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds: Long) {
        mockingClient.forEmis.mock {
            practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel())
                    .delayedBy(Duration.ofSeconds(delayBySeconds))
        }
        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest()
                    .respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(delayBySeconds))
        }
        mockingClient.forEmis.mock { authentication.sessionRequest(patient)
                .respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsAndGpSystemReturnsError() {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest().respondWithCorruptedContent() }
        mockingClient.forEmis.mock { authentication.sessionRequest(patient)
            .respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest()
                .respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis.mock { authentication.sessionRequest(patient).respondWithForbidden() }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable(patient: Patient) {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest().respondWithServiceUnavailable() }
        mockingClient.forEmis.mock { authentication.sessionRequest(patient)
                .respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsAndGpSystemBadGateway() {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest().respondWithBadGateway() }
        mockingClient.forEmis.mock { authentication.sessionRequest(patient)
                .respondWithSuccess(patient, associationType) }
    }

    private fun createInvalidLinkageTest( patient: Patient, emisResponse: (EmisMeApplicationsBuilder.() -> Mapping)) {
        mockingClient.forEmis.mock { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis.mock { emisResponse(authentication
                .meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))) }
        mockingClient.forEmis.mock { authentication.endUserSessionRequest()
                .respondWithSuccess(patient.endUserSessionId) }
    }

    private fun createLinkApplicationRequestModel(patient: Patient): LinkApplicationRequestModel {
        return LinkApplicationRequestModel(
                surname = patient.name.surname,
                dateOfBirth = patient.age.dateOfBirth.plus("T00:00:00"),
                linkageDetails = LinkageDetailsModel(
                        accountId = patient.accountId,
                        nationalPracticeCode = patient.odsCode,
                        linkageKey = patient.linkageKey
                )
        )
    }
}
