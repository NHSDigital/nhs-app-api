package mocking.tpp.linkage

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.LinkAccount
import org.apache.http.HttpStatus

class TppLinkageGETBuilder(linkAccount: LinkAccount) : TppMappingBuilder("GET", "/tpp/") {

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val typeHeader = "type"
        val typeValue = "LinkAccount"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//LinkAccount[" +
                        "@apiVersion='${linkAccount.apiVersion}' and " +
                        "@organisationCode='${linkAccount.organisationCode}']")
    }

    fun respondWithNotFound(): Mapping {
        return respondWith(HttpStatus.SC_NOT_FOUND){
            andXmlBody("").build()
        }
    }
}