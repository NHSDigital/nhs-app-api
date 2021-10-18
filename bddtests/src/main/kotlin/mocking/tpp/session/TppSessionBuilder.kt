package mocking.tpp.session

import mocking.defaults.TppMockDefaults
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller


class TppSessionBuilder(authenticate: Authenticate) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "Authenticate"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//Authenticate[" +
                        "@apiVersion='${authenticate.apiVersion}' and " +
                        "@accountId='${authenticate.accountId}' and " +
                        "@unitId='${authenticate.unitId}' and " +
                        "@passphrase='${authenticate.passphrase}']")
    }

    fun respondWithSuccess(authenticateReply: AuthenticateReply): Mapping {
        val suidHeader = "suid"
        val suidValue = TppMockDefaults.DEFAULT_TPP_SESSION_ID

        val jaxbContext = JAXBContext.newInstance(AuthenticateReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(authenticateReply, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }
    }

    fun respondWithError(errorBody: Error): Mapping {
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
        )
        return respondWith(responseBody)
    }
}
