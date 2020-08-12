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

class TppLinkagePOSTBuilder(linkAccount: LinkAccount) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "LinkAccount"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//LinkAccount[" +
                        "@apiVersion='${linkAccount.apiVersion}']")
    }


    fun respondWithSuccessfullyCreated(linkage: LinkageInformationFacade): Mapping {
        val reply = LinkAccountReply(
                passphrase = TppMockDefaults.DEFAULT_TPP_PASSPHRASE,
                uuid = TppMockDefaults.DEFAULT_TPP_UUID,
                accountId = linkage.accountId,
                passphraseToLink = "passphraseToLink"
        )
        val body = JSonXmlConverter.toXML(reply)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(body).build()
        }
    }

    fun respondWithInternalServerError(): Mapping {
        return respondWithBody("Internal Server Error", HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    fun respondWithPatientNonCompetentOrUnderMinumumAge(): Mapping {
        val disabledTppError = Error(errorCode = "8")
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(JSonXmlConverter.toXML(disabledTppError))
        }
    }
}

