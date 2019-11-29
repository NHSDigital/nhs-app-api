package features.linkage.stepDefinitions

import constants.DateTimeFormats
import constants.Supplier
import features.linkage.LinkageResult
import mocking.defaults.TppMockDefaults
import mocking.models.Mapping
import mocking.tpp.linkage.TppLinkageGETBuilder
import mocking.tpp.linkage.TppLinkagePOSTBuilder
import mocking.tpp.models.LinkAccount
import mockingFacade.linkage.LinkageInformationFacade
import models.Patient

class LinkageFactoryTpp:  LinkageFactory(Supplier.TPP) {
    override val validOtherLinkageDetails = LinkageInformationFacade(
            odsCode =  TppMockDefaults.DEFAULT_ODS_CODE_TPP,
            linkageKey = "anotherPassphraseToLink",
            accountId = "123456789",
            nhsNumber = "1234567890",
            identityToken = "abc",
            emailAddress = "ab@cd.com",
            surname = "Thompson",
            dateOfBirth = "2000-01-01")

    override val validLinkageDetails = LinkageInformationFacade(
            odsCode =  patient.odsCode,
            linkageKey = patient.linkageKey,
            accountId = patient.accountId,
            nhsNumber = "3434234345",
            identityToken = "abc",
            emailAddress = "ab@cd.com",
            surname = "Thompson",
            dateOfBirth = "2000-01-01")

    override val linkageDateOfBirthFormat = DateTimeFormats.backendDateTimeFormatWithoutTimezone

    override fun mockLinkagePostResult(linkageInformationFacade: LinkageInformationFacade,
                                       linkageResult: LinkageResult, delay: Long?) {

        val linkageToPostRequestResponse = hashMapOf(
                LinkageResult.SuccessfullyCreated to successfulPost(linkageInformationFacade),
                LinkageResult.SuccessfullyRetrieved to successfulPost(linkageInformationFacade),
                LinkageResult.InternalServerError to { post -> post.respondWithInternalServerError() },
                LinkageResult.PatientNonCompetentOrUnderMinimumAge to {post -> post
                        .respondWithPatientNonCompetentOrUnderMinumumAge()}
        )

        val response = responseFromMap(linkageToPostRequestResponse, linkageResult)
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))

        if (response != null) {
            mockingClient.forTpp {
                response(authentication.linkageKeyPOSTRequest(linkAccount))
            }
        }
    }

    private fun successfulPost(linkage: LinkageInformationFacade):((TppLinkagePOSTBuilder) -> Mapping)? {
        return { post -> post.respondWithSuccessfullyCreated(linkage) }
    }

    override fun mockLinkageGetResult(linkageInformationFacade: LinkageInformationFacade,
                                      linkageResult: LinkageResult) {

        val linkageToGetRequestResponse = hashMapOf(
                LinkageResult.SuccessfullyRetrieved to successfulGet()
        )

        val response = responseFromMap(linkageToGetRequestResponse, linkageResult)
        val linkAccount = LinkAccount.forPatient(Patient.getDefault(gpSystem))

        if (response != null) {
            mockingClient.forTpp {
                response(authentication.linkageKeyGetRequest(linkAccount))
            }
        }
    }

    private fun successfulGet():((TppLinkageGETBuilder) -> Mapping)? {
        return { post -> post.respondWithNotFound() }
    }
}
