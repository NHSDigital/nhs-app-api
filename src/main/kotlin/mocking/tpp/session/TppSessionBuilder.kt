package mocking.tpp.session

import mocking.models.Mapping
import mocking.tpp.TppMappingBuilder
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import org.apache.http.HttpStatus
import javax.xml.bind.JAXBContext
import java.io.StringWriter
import javax.xml.bind.Marshaller


class TppSessionBuilder(authenticate: Authenticate) : TppMappingBuilder("POST", "/tpp") {
    init {
        val jaxbContext = JAXBContext.newInstance(Authenticate::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(authenticate, stringWriter)
        }

        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml"
        val typeHeader = "type"
        val typeValue = "Authenticate"


        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andHeader(typeHeader, typeValue)
                .andXmlBody(stringWriter.toString())
    }

    fun respondWithSuccess(authenticateReply: AuthenticateReply): Mapping {
        val responseBody = AuthenticateReply(
                authenticateReply.patientId,
                authenticateReply.onlineUserId,
                authenticateReply.uuid,
                authenticateReply.user)

        val suidHeader = "suid"
        val suidValue = "alsdkfjLIKASDLIHUAJakjshdLIASKHDJALsdiojALSasIADJAISDioasjd"

        val jaxbContext = JAXBContext.newInstance(AuthenticateReply::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(responseBody, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(stringWriter.toString())
                    .andHeader(suidHeader, suidValue)
                    .build()
        }
    }

    fun respondWithSuccesssWithoutSuid(authenticateReply: AuthenticateReply): Mapping {
        val responseBody = AuthenticateReply(
                authenticateReply.patientId,
                authenticateReply.onlineUserId,
                authenticateReply.uuid,
                authenticateReply.user)

        val jaxbContext = JAXBContext.newInstance(AuthenticateReply::class.java)
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