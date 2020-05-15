package features.authentication.factories

import constants.DateTimeFormats
import constants.Supplier
import mocking.defaults.VisionMockDefaults
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.vision.models.Account
import mocking.vision.models.Configuration
import mocking.vision.models.PatientNumber
import mocking.vision.models.Register
import mocking.vision.models.VisionUserSession
import mocking.vision.models.linkage.LinkageKeyGetResponse
import mocking.vision.models.linkage.LinkageKeyPostRequest
import mockingFacade.linkage.LinkageInformationFacade
import mocking.models.Mapping
import java.time.Duration

class Im1ConnectionV2FactoryVision : Im1ConnectionV2Factory(Supplier.VISION) {

    override fun successfulIm1Register(linkageFacade: LinkageInformationFacade, delay: Duration?) {
        configuration()
        mockingClient.forVision.mock {
                authentication.getRegisterRequest(
                        VisionMockDefaults.getVisionUserSession(patient),
                        patient)
                        .respondWithSuccess(
                                Register(
                                        rosuAccountId = patient.rosuAccountId,
                                        apiToken = patient.apiKey)
                        ).delayedBy(delay)
            }
    }

    override fun errorIm1Register(httpStatusCode: Int, errorCode: String, message: String?) {
        configuration()
        mockingClient.forVision.mock {
            authentication.getRegisterRequest(
                    VisionMockDefaults.getVisionUserSession(patient),
                    patient)
                    .respondWithError(httpStatusCode, errorCode, message)
        }
    }

    override val linkageDateOfBirthFormat = DateTimeFormats.dateWithoutTimeFormat

    override fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                             action: (IErrorMappingBuilder)-> Mapping) {
        mockingClient.forVision.mock {
            action( authentication.linkageKeyPostRequest(
                    linkageInformationFacade.odsCode,
                    linkageKeyPostRequest(linkageInformationFacade)))
        }
    }

    override fun linkageGet(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder)->  Mapping) {
        mockingClient.forVision.mock {
            action( authentication.linkageKeyGetRequest(
                    linkageInformationFacade.odsCode,
                    linkageInformationFacade.nhsNumber))
        }
    }

    override fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade) {
        mockingClient.forVision.mock {
            authentication.linkageKeyGetRequest(linkageInformationFacade.odsCode,
                    linkageInformationFacade.nhsNumber).respondWithSuccessfullyRetrieved(
                    successfulLinkageResponse(linkageInformationFacade))

        }
    }

    override fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade) {
        mockingClient.forVision.mock {
            authentication.linkageKeyPostRequest(linkageInformationFacade.odsCode,
                    linkageKeyPostRequest(linkageInformationFacade))
                    .respondWithSuccessfullyCreated(
                            successfulLinkageResponse(linkageInformationFacade))
        }
    }

    private fun linkageKeyPostRequest(linkageInformationFacade: LinkageInformationFacade): LinkageKeyPostRequest {
        return LinkageKeyPostRequest(
                linkageInformationFacade.surname,
                linkageInformationFacade.dateOfBirth,
                linkageInformationFacade.nhsNumber
        )
    }

    private fun successfulLinkageResponse(linkageInformationFacade: LinkageInformationFacade): LinkageKeyGetResponse {
        return LinkageKeyGetResponse(
                odsCode = linkageInformationFacade.odsCode,
                linkageKey = linkageInformationFacade.linkageKey,
                accountId = linkageInformationFacade.accountId,
                surname = linkageInformationFacade.surname,
                dateOfBirth = linkageInformationFacade.dateOfBirth,
                apiKey = linkageInformationFacade.apiKey)
    }

    private fun configuration(){
        mockingClient.forVision.mock {
                    authentication.getConfigurationRequest(
                            visionUserSession = VisionUserSession(
                                    patient.rosuAccountId,
                                    patient.apiKey,
                                    patient.odsCode,
                                    patient.patientId
                            ))
                            .respondWithSuccess(
                                    configuration = Configuration(account = Account(patient.patientId,
                                            patientNumber = listOf(PatientNumber(number = patient.nhsNumbers.first())),
                                            name = patient.formattedFullName()))
                            )
                }
    }
}
