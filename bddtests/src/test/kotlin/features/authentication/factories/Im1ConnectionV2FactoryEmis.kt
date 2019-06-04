package features.authentication.factories

import constants.DateTimeFormats
import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.AddVerificationRequest
import mocking.emis.models.AddVerificationResponse
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mockingFacade.linkage.LinkageInformationFacade

class Im1ConnectionV2FactoryEmis : Im1ConnectionV2Factory("EMIS") {
    override fun errorIm1Register(httpStatusCode: Int, errorCode: String, message: String?) {
        endUserSessionRequest()
        sessionRequest()
        demographicsRequest()
        mockingClient
                .forEmis {
                    authentication.meApplicationsRequest(patient,
                            LinkApplicationRequestModel(
                                    surname = patient.surname,
                                    dateOfBirth = patient.dateOfBirth.plus("T00:00:00"),
                                    linkageDetails = LinkageDetailsModel(
                                            accountId = patient.accountId,
                                            linkageKey = patient.linkageKey,
                                            nationalPracticeCode = patient.odsCode
                                    )
                            )
                    ).respondWithError(httpStatusCode, errorCode, message)
                }
    }

    private fun endUserSessionRequest() {
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
    }

    private fun sessionRequest() {
        mockingClient.forEmis {
            authentication.sessionRequest(patient)
                    .respondWithSuccess(patient, associationType = AssociationType.Self)
        }
    }

    private fun demographicsRequest() {
        mockingClient.forEmis {
            myRecord.demographicsRequest(patient)
                    .respondWithSuccess(patient,
                            patientIdentifiers =
                            patient.nhsNumbers.map {
                                PatientIdentifier(
                                        identifierType = IdentifierType.NhsNumber,
                                        identifierValue = it
                                )
                            }.toTypedArray()
                    )
        }
    }

    override fun successfulIm1Register(linkageFacade: LinkageInformationFacade) {
        endUserSessionRequest()
        sessionRequest()
        demographicsRequest()
        mockingClient
                .forEmis {
                    authentication.meApplicationsRequest(patient,
                            LinkApplicationRequestModel(
                                    surname = linkageFacade.surname,
                                    dateOfBirth = linkageFacade.dateOfBirth.plus("T00:00:00"),
                                    linkageDetails = LinkageDetailsModel(
                                            accountId = linkageFacade.accountId,
                                            linkageKey = linkageFacade.linkageKey,
                                            nationalPracticeCode = linkageFacade.odsCode
                                    )
                            )
                    ).respondWithSuccess(identityToken)
                }
    }

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone

    override fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder) -> Mapping) {
        // end user session setup always required
        endUserSessionSetup()
        mockingClient.forEmis {
            action(authentication.linkageKeyPOSTRequest(
                    AddNhsUserRequest(
                            linkageInformationFacade.odsCode,
                            linkageInformationFacade.nhsNumber,
                            linkageInformationFacade.emailAddress)))

        }
    }

    override fun linkageGet(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder) -> Mapping) {
        // end user session setup always required
        endUserSessionSetup()
        mockingClient.forEmis {
            action(authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade)))
        }
    }

    override fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade) {
        endUserSessionSetup()
        mockingClient.forEmis {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrieved(verificationResponse(linkageInformationFacade))
        }
    }

    fun successfulGetFirstTime(linkageInformationFacade: LinkageInformationFacade) {
        endUserSessionSetup()
        mockingClient.forEmis {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrievedFirstTime(verificationResponse(linkageInformationFacade))
        }
    }

    override fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade) {
        mockingClient.forEmis {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrieved(verificationResponse(linkageInformationFacade))
        }
        endUserSessionSetup()
        mockingClient.forEmis {
            authentication.linkageKeyPOSTRequest(
                    AddNhsUserRequest(
                            linkageInformationFacade.odsCode,
                            linkageInformationFacade.nhsNumber,
                            linkageInformationFacade.emailAddress))
                    .respondWithSuccessfullyCreated(AddNhsUserResponse(EmisMockDefaults.patientEmis.connectionToken))
        }
    }

    private fun endUserSessionSetup() {
        mockingClient.forEmis {
            authentication.endUserSessionRequest().respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
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
                linkageInformationFacade.accountId)
    }
}
