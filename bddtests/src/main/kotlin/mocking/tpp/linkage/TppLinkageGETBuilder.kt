package mocking.tpp.linkage

import mocking.JSonXmlConverter
import mocking.defaults.TppMockDefaults
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.LinkAccount
import mocking.tpp.models.LinkAccountReply
import mockingFacade.linkage.LinkageInformationFacade
import org.apache.http.HttpStatus

class TppLinkageGETBuilder(linkAccount: LinkAccount) : TppMappingBuilder("POST", "/tpp/") {

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "LinkAccount"

        val dateOfBirthInCorrectFormat =
            if (linkAccount.dateOfBirth.endsWith("T00:00:00")) linkAccount.dateOfBirth
            else linkAccount.dateOfBirth.plus("T00:00:00")

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//LinkAccount[" +
                        "@apiVersion='${linkAccount.apiVersion}' and " +
                        "@lastName='${linkAccount.lastName}' and " +
                        "@dateOfBirth='${dateOfBirthInCorrectFormat}' and " +
                        "@nhsNumber='${linkAccount.nhsNumber}' and " +
                        "@retrieveOnly='y' and " +
                        "@organisationCode='${linkAccount.organisationCode}']")
    }

    fun respondWithSuccessfullyRetrieved(linkage: LinkageInformationFacade): Mapping {
        val reply = LinkAccountReply(
                uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                accountId = linkage.accountId,
                passphraseToLink = linkage.linkageKey
        )

        val body = JSonXmlConverter.toXML(reply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(body).build()
        }
    }

    fun respondWithError(errorCode: String): Mapping {
        val reply = Error(
                errorCode = errorCode
        )

        val body = JSonXmlConverter.toXML(reply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(body).build()
        }
    }

    fun respondWithNotFound(): Mapping {
        return respondWith(HttpStatus.SC_NOT_FOUND){
            andXmlBody("").build()
        }
    }
}
