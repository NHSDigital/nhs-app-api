package features.linkage.stepDefinitions

import features.linkage.LinkageResult
import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import mocking.emis.linkage.EmisLinkageGETBuilder
import mocking.emis.linkage.EmisLinkagePOSTBuilder
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.AddVerificationRequest
import mocking.emis.models.AddVerificationResponse
import mocking.models.Mapping
import mockingFacade.linkage.LinkageInformationFacade

class LinkageFactoryEmis : LinkageFactory("EMIS") {

    override val validLinkageDetails = LinkageInformationFacade(
                odsCode =  EmisMockDefaults.DEFAULT_ODS_CODE_EMIS,
                linkageKey = "tTALtBP3rLR16",
                accountId = "542343",
                nhsNumber = "3434234345",
                identityToken = "abc",
                emailAddress = "ab@cd.com",
                surname = "Edgar")

    override fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade,
                                       linkageResult: LinkageResult) {
        val linkageToPostRequestResponse = hashMapOf(
                LinkageResult.SuccessfullyRetrieved to successfulPost(linkageInformationFacade),
                LinkageResult.SuccessfullyCreated to successfulPost(linkageInformationFacade),
                LinkageResult.PatientAlreadyHasAnOnlineAccount to { get -> get
                        .respondWithPatientAlreadyHasAnOnlineAccount() },
                LinkageResult.NoRegisteredOnlineUserFound to { get -> get
                        .respondWithNoRegisteredOnlineUserFound() },
                LinkageResult.PatientNotRegisteredAtPractice to null,
                LinkageResult.PracticeNotLive to { get -> get
                        .respondWithPracticeNotLive() },
                LinkageResult.PatientMarkedAsArchived to { get -> get
                        .respondWithPatientMarkedAsArchived() },
                LinkageResult.PatientNonCompetentOrUnderMinimumAge to { get -> get
                        .respondWithPatientNonCompetentOrUnderMinumumAge() },
                LinkageResult.InternalServerError to { get -> get
                        .respondWithInternalServerError() }
        )

        // end user session setup always required
        endUserSessionSetup()

        val response = responseFromMap(linkageToPostRequestResponse, linkageResult)

        if (response != null) {
            mockingClient.forEmis {
                response(authentication.linkageKeyPOSTRequest(
                        AddNhsUserRequest(
                                linkageInformationFacade.odsCode,
                                linkageInformationFacade.nhsNumber,
                                linkageInformationFacade.emailAddress)))
            }
        }

    }

    private fun successfulPost(linkageInformationFacade: LinkageInformationFacade):
            (EmisLinkagePOSTBuilder) -> Mapping {

        mockingClient.forEmis {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrievedFirstTime(verificationResponse(linkageInformationFacade))
        }
        return { post -> post
                .respondWithSuccessfullyCreated(AddNhsUserResponse(EmisMockDefaults.patientEmis.connectionToken)) }
    }


    override fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {
        val linkageToGetRequestResponse = hashMapOf<LinkageResult, ((EmisLinkageGETBuilder) -> Mapping)?>(
                LinkageResult.SuccessfullyRetrievedFirstTime to successfulGet(linkageInformationFacade),
                LinkageResult.SuccessfullyRetrieved to successfulGet(linkageInformationFacade),
                LinkageResult.PatientNotRegisteredAtPractice to { get -> get
                        .respondWithPatientNotRegisteredAtPractice() },
                LinkageResult.NoRegisteredOnlineUserFound to { get -> get
                        .respondWithNoRegisteredOnlineUserFound() },
                LinkageResult.PracticeNotLive to { get -> get
                        .respondWithPracticeNotLive() },
                LinkageResult.PatientMarkedAsArchived to { get -> get
                        .respondWithPatientMarkedAsArchived() },
                LinkageResult.PatientNonCompetentOrUnderMinimumAge to { get -> get
                        .respondWithPatientNonCompetentOrUnderMinimumAge() },
                LinkageResult.AccountStatusInvalid to { get -> get
                        .respondWithAccountStatusInvalid() },
                LinkageResult.InternalServerError to { get -> get
                        .respondWithInternalServerError() }
        )

        // end user session setup always required
        endUserSessionSetup()
        val response = responseFromMap(linkageToGetRequestResponse, linkageResult)
        mockingClient.forEmis {
            response!!(authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade)))
        }
    }

    private fun endUserSessionSetup(){
        mockingClient.forEmis {
            authentication.endUserSessionRequest().respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
        }
    }

    private fun successfulGet(linkageInformationFacade: LinkageInformationFacade): (EmisLinkageGETBuilder) -> Mapping {
        return { get ->
            get.respondWithSuccessfullyRetrievedFirstTime(verificationResponse(linkageInformationFacade))
        }
    }

    private fun verificationRequest(linkageInformationFacade: LinkageInformationFacade): AddVerificationRequest {
        return AddVerificationRequest(
                linkageInformationFacade.nhsNumber,
                linkageInformationFacade.odsCode,
                linkageInformationFacade.identityToken)
    }

    private fun verificationResponse(linkageInformationFacade: LinkageInformationFacade): AddVerificationResponse {
        return AddVerificationResponse(
                linkageInformationFacade.odsCode,
                linkageInformationFacade.linkageKey,
                linkageInformationFacade.accountId!!)
    }
}
