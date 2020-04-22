package mocking.tpp.patientpracticemessaging

import mocking.defaults.TppMockDefaults.Companion.DEFAULT_TPP_SESSION_ID
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.MessageMarkAsReadReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppMarkMessagesAsReadBuilder(tppUserSession: TppUserSession) : TppMappingBuilder() {
    init {
        val typeHeader = "type"
        val typeValue = "MessageMarkAsRead"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//MessageMarkAsRead[" +
                                              "@apiVersion='${apiVersion}' and " +
                                              "@patientId='${tppUserSession.patientId}' and " +
                                              "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                                              "@unitId='${tppUserSession.unitId}']")
    }

    fun respondWithSuccess(messageMarkAsReadReply: MessageMarkAsReadReply): Mapping {
        val suidHeader = "suid"
        val suidValue = DEFAULT_TPP_SESSION_ID

        val jaxbContext = JAXBContext.newInstance(MessageMarkAsReadReply::class.java)
        val marshaller = jaxbContext.createMarshaller()

        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()

        stringWriter.use {
            marshaller.marshal(messageMarkAsReadReply, stringWriter)
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
                errorBody.uuid)

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