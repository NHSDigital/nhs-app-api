package features.linkage.stepDefinitions

import constants.DateTimeFormats
import constants.Supplier
import features.linkage.LinkageResult
import mocking.models.Mapping
import mocking.defaults.VisionMockDefaults
import mocking.vision.linkage.VisionLinkageGETBuilder
import mocking.vision.linkage.VisionLinkagePOSTBuilder
import mocking.vision.models.linkage.LinkageKeyGetResponse
import mocking.vision.models.linkage.LinkageKeyPostRequest
import mockingFacade.linkage.LinkageInformationFacade

class LinkageFactoryVision : LinkageFactory(Supplier.VISION) {

    override val validLinkageDetails = LinkageInformationFacade(
            odsCode =  VisionMockDefaults.DEFAULT_ODS_CODE_VISION,
            linkageKey = "mw2cktkzypqhr32yt9w4qccnmtcva24gwhfd2ughgj4tuq8rbqe4ahbvfp4dfp8p",
            accountId = "028738",
            nhsNumber = "8747384738",
            emailAddress = "visionuser@test.com",
            surname = "Smyth",
            dateOfBirth = "1997-05-01",
            apiKey = "82628-98721")

    override val validOtherLinkageDetails = validLinkageDetails

    override val linkageDateOfBirthFormat = DateTimeFormats.dateWithoutTimeFormat

    override fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade,
                                       linkageResult: LinkageResult, delay: Long?) {
        val linkageToPostRequestResponse = hashMapOf<LinkageResult, ((VisionLinkagePOSTBuilder) -> Mapping)?>(
                LinkageResult.SuccessfullyCreated to { get ->
                    get.respondWithSuccessfullyCreated(successfulLinkageResponse(linkageInformationFacade))
                },
                LinkageResult.InvalidNhsNumber to { get ->
                    get.respondWithErrorInvalidNhsNumber()
                },
                LinkageResult.PatientRecordNotFound to { get ->
                    get.respondWithErrorPatientRecordNotFound()
                },
                LinkageResult.LinkageKeyAlreadyExists to { get ->
                    get.respondWithErrorLinkageKeyAlreadyExists()
                },
                LinkageResult.InternalServerError to { get ->
                    get.respondWithErrorInternalServerError()
                }
        )

        val response = responseFromMap(linkageToPostRequestResponse, linkageResult)

        if (response != null) {
            mockingClient.forVision {
                response(VisionLinkagePOSTBuilder(
                        linkageInformationFacade.odsCode,
                        linkageKeyPostRequest(linkageInformationFacade)))
            }
        }
    }

    override fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {
        val linkageToGetRequestResponse = hashMapOf<LinkageResult, ((VisionLinkageGETBuilder) -> Mapping)?>(
                LinkageResult.SuccessfullyRetrieved to { get ->
                    get.respondWithSuccessfullyRetrieved(successfulLinkageResponse(linkageInformationFacade))
                },
                LinkageResult.InvalidNhsNumber to { get ->
                    get.respondWithErrorInvalidNhsNumber()
                },
                LinkageResult.PatientRecordNotFound to { get ->
                    get.respondWithErrorPatientRecordNotFound()
                },
                LinkageResult.LinkageKeyRevoked to { get ->
                    get.respondWithErrorLinkageKeyRevoked()
                },
                LinkageResult.NoUserAssociatedWithNHSNumber to { get ->
                    get.respondWithErrorNoUserAssociatedWithNHSNumber()
                },
                LinkageResult.NoApiKeyAssociatedWithNHSNumber to { get ->
                    get.respondWithErrorNoApiKeyAssociatedWithNHSNumber()
                },
                LinkageResult.InternalServerError to { get ->
                    get.respondWithErrorInternalServerError()
                }
        )

        val response = responseFromMap(linkageToGetRequestResponse, linkageResult)

        if (response != null) {
            mockingClient.forVision {
                response(VisionLinkageGETBuilder(linkageInformationFacade.odsCode, linkageInformationFacade.nhsNumber))
            }
        }
    }

    fun linkageKeyPostRequest(linkageInformationFacade: LinkageInformationFacade): LinkageKeyPostRequest {
        return LinkageKeyPostRequest(
                linkageInformationFacade.surname,
                linkageInformationFacade.dateOfBirth,
                linkageInformationFacade.nhsNumber
        )
    }

    private fun successfulLinkageResponse(linkageInformationFacade: LinkageInformationFacade): LinkageKeyGetResponse {
        return LinkageKeyGetResponse(
                odsCode =  linkageInformationFacade.odsCode,
                linkageKey = linkageInformationFacade.linkageKey,
                accountId = linkageInformationFacade.accountId,
                surname = linkageInformationFacade.surname,
                dateOfBirth = linkageInformationFacade.dateOfBirth,
                apiKey = linkageInformationFacade.apiKey)
    }
}
