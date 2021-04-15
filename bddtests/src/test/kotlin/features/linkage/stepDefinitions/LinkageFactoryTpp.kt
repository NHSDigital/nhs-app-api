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
            nhsNumber = patient.nhsNumbers.first().filter { !it.isWhitespace() },
            identityToken = "abc",
            emailAddress = patient.contactDetails.emailAddress,
            surname = patient.name.surname,
            dateOfBirth = patient.age.dateOfBirth
    )

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

        val linkAccount = LinkAccount(
            lastName = linkageInformationFacade.surname,
            dateOfBirth = linkageInformationFacade.dateOfBirth,
            organisationCode = linkageInformationFacade.odsCode,
            nhsNumber = linkageInformationFacade.nhsNumber,
            emailAddress = linkageInformationFacade.emailAddress,
            mobileNo = linkageInformationFacade.mobileNumber
        )

        if (response != null) {
            mockingClient.forTpp.mock {
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
                LinkageResult.SuccessfullyRetrieved to successfulGet(linkageInformationFacade)
        )

        val response = responseFromMap(linkageToGetRequestResponse, linkageResult)
        val linkAccount = LinkAccount(
                passphrase = linkageInformationFacade.linkageKey,
                lastName = linkageInformationFacade.surname,
                dateOfBirth = linkageInformationFacade.dateOfBirth,
                organisationCode = linkageInformationFacade.odsCode,
                nhsNumber = linkageInformationFacade.nhsNumber
        )

        if (response != null) {
            mockingClient.forTpp.mock {
                response(authentication.linkageKeyGetRequest(linkAccount))
            }
        }
    }

    private fun successfulGet(linkageInformationFacade: LinkageInformationFacade):((TppLinkageGETBuilder) -> Mapping)? {
        return { get -> get.respondWithSuccessfullyRetrieved(linkageInformationFacade) }
    }
}
