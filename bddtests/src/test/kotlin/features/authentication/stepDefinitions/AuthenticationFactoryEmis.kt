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
        mockingClient.forEmis { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
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
        mockingClient.forEmis {
            practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel())
                    .delayedBy(Duration.ofSeconds(delayBySeconds))
        }
        mockingClient.forEmis {
            authentication.endUserSessionRequest()
                    .respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(delayBySeconds))
        }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithSuccess(patient, associationType) }
    }

    override fun validOAuthDetailsCidConnectionTokenFailsToAuthenticate() {
        mockingClient.forEmis { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithForbidden() }
    }

    override fun validOAuthDetailsAndGpSystemUnavailable() {
        mockingClient.forEmis { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithServiceUnavailable() }
        mockingClient.forEmis { authentication.sessionRequest(patient).respondWithSuccess(patient, associationType) }
    }

    private fun createInvalidLinkageTest( patient: Patient, emisResponse: (EmisMeApplicationsBuilder.() -> Mapping)) {
        mockingClient.forEmis { practiceSettingsRequest(patient).respondWithSuccess(SettingsResponseModel()) }
        mockingClient.forEmis { emisResponse(authentication
                .meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))) }
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
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
