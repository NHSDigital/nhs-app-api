package mocking.tpp.patientpracticemessaging

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Error
import mocking.tpp.models.MessageCreateReply
import org.apache.http.HttpStatus
import worker.models.demographics.TppUserSession
import worker.models.patientPracticeMessaging.CreateMessageRequest
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class TppPatientPracticeMessagingCreateMessageBuilder(tppUserSession: TppUserSession,
                                                      createMessageRequest: CreateMessageRequest) :
        TppMappingBuilder("POST", "/tpp/") {
    init {
        val typeHeader = "type"
        val typeValue = "MessageCreate"
        val apiVersion = "1"

        val recipientSplit = createMessageRequest.recipient.split(":")
        val type = recipientSplit[1]
        val recipientXml: String

        recipientXml =
                if (type == "UnitRecipient") {
                    "@unitRecipientId='${recipientSplit[0]}'"
                } else {
                    "@recipientId='${recipientSplit[0]}'"
                }

        requestBuilder
                .andHeader(typeHeader, typeValue)
                .andBodyMatchingXpath("//MessageCreate[" +
                                              "@apiVersion='${apiVersion}' and " +
                                              "@patientId='${tppUserSession.patientId}' and " +
                                              "@onlineUserId='${tppUserSession.onlineUserId}' and " +
                                              "@unitId='${tppUserSession.unitId}' and " +
                                              "$recipientXml and" +
                                              "@message='${createMessageRequest.messageBody}']")
    }

    fun respondWithSuccess(messageCreateReply: MessageCreateReply): Mapping {
        val suidHeader = "suid"
        val suidValue = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        val jaxbContext = JAXBContext.newInstance(MessageCreateReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(messageCreateReply, stringWriter)
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