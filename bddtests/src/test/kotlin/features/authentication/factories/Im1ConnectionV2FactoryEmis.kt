package features.authentication.factories

import constants.DateTimeFormats
import constants.Supplier
import mocking.defaults.EmisMockDefaults
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
import java.time.Duration

class Im1ConnectionV2FactoryEmis : Im1ConnectionV2Factory(Supplier.EMIS) {
    override fun errorIm1Register(httpStatusCode: Int, errorCode: String, message: String?) {
        endUserSessionRequest()
        sessionRequest()
        demographicsRequest()
        mockingClient.forEmis.mock {
                    authentication.meApplicationsRequest(patient,
                            LinkApplicationRequestModel(
                                    surname = patient.name.surname,
                                    dateOfBirth = patient.age.dateOfBirth.plus("T00:00:00"),
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
        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
    }

    private fun sessionRequest() {
        mockingClient.forEmis.mock {
            authentication.sessionRequest(patient)
                    .respondWithSuccess(patient, associationType = AssociationType.Self)
        }
    }

    private fun demographicsRequest() {
        mockingClient.forEmis.mock {
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

    override fun successfulIm1Register(linkageFacade: LinkageInformationFacade, delay: Duration?) {
        endUserSessionRequest()
        sessionRequest()
        demographicsRequest()
        mockingClient.forEmis.mock {
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
                    ).respondWithSuccess(identityToken).delayedBy(delay)
                }
    }

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone

    override fun linkagePost(linkageInformationFacade: LinkageInformationFacade,
                            action: (IErrorMappingBuilder) -> Mapping) {
        // end user session setup always required
        endUserSessionSetup()
        mockingClient.forEmis.mock {
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
        mockingClient.forEmis.mock {
            action(authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade)))
        }
    }

    override fun successfulLinkageGet(linkageInformationFacade: LinkageInformationFacade) {
        endUserSessionSetup()
        mockingClient.forEmis.mock {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrieved(verificationResponse(linkageInformationFacade))
        }
    }

    fun successfulGetFirstTime(linkageInformationFacade: LinkageInformationFacade) {
        endUserSessionSetup()
        mockingClient.forEmis.mock {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrievedFirstTime(verificationResponse(linkageInformationFacade))
        }
    }

    override fun successfulLinkagePost(linkageInformationFacade: LinkageInformationFacade) {
        endUserSessionSetup()
        mockingClient.forEmis.mock {
            authentication.linkageKeyPOSTRequest(
                    AddNhsUserRequest(
                            linkageInformationFacade.odsCode,
                            linkageInformationFacade.nhsNumber,
                            linkageInformationFacade.emailAddress))
                    .respondWithSuccessfullyCreated(AddNhsUserResponse(EmisMockDefaults.patientEmis.connectionToken))
                    .inScenario("LinkageCreation")
                    .willSetStateTo("Linkage key created")
        }
        mockingClient.forEmis.mock {
            authentication.linkageKeyGetRequest(verificationRequest(linkageInformationFacade))
                    .respondWithSuccessfullyRetrieved(verificationResponse(linkageInformationFacade))
                    .inScenario("LinkageCreation")
                    .whenScenarioStateIs("Linkage key created")
        }
    }

    private fun endUserSessionSetup() {
        mockingClient.forEmis.mock {
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
