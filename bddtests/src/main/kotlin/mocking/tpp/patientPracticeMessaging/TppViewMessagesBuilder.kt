package mocking.tpp.patientPracticeMessaging

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.MessagesViewReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller


class TppViewMessagesBuilder(tppUserSession: TppUserSession) : TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "MessagesView"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//MessagesView[" +
                                              "@apiVersion='${apiVersion}' and " +
                                              "@patientId='${tppUserSession.patientId}' and " +
                                              "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                                              "@unitId='${tppUserSession.unitId}']")
    }

    fun respondWithSuccess(messagesViewReply: MessagesViewReply): Mapping {
        val suidHeader = "suid"
        val suidValue = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        val jaxbContext = JAXBContext.newInstance(MessagesViewReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(messagesViewReply, stringWriter)
        }

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }

        return resp
    }

    fun respondWithError(errorBody: Error): Mapping {
        val responseBody = Error(
                errorBody.errorCode,
                errorBody.userFriendlyMessage,
                errorBody.uuid
                                )

        val jaxbContext = JAXBContext.newInstance(Error::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .build()
        }
    }
}