package mocking.tpp.patientpracticemessaging

import mocking.defaults.TppMockDefaults.Companion.DEFAULT_TPP_SESSION_ID
import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.MessageRecipientsReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppPatientPracticeMessagingRecipientsBuilder(
        tppUserSession: TppUserSession) : TppMappingBuilder() {
    init {
        val typeHeader = "type"
        val typeValue = "MessageRecipients"
        val apiVersion = "1"

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//MessageRecipients[" +
                                              "@apiVersion='${apiVersion}' and " +
                                              "@patientId='${tppUserSession.patientId}' and " +
                                              "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                                              "@unitId='${tppUserSession.unitId}']")
    }

    fun respondWithSuccess(messageRecipientsReply: MessageRecipientsReply): Mapping {
        val suidHeader = "suid"
        val suidValue = DEFAULT_TPP_SESSION_ID

        val jaxbContext = JAXBContext.newInstance(MessageRecipientsReply::class.java)
        val marshaller = jaxbContext.createMarshaller()

        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()

        stringWriter.use {
            marshaller.marshal(messageRecipientsReply, stringWriter)
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
