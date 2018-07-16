package mocking.vision

import mocking.models.Mapping
import mocking.vision.VisionConstants.getInvalidRequestError
import mocking.vision.VisionConstants.getInvalidUserCredentialsError
import mocking.vision.VisionConstants.getUnkownError
import mocking.vision.VisionConstants.getVisionResponse
import mocking.vision.VisionConstants.securityHeaderErrorResponse
import mocking.vision.models.Configuration
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionGetConfigurationBuilder(var userSession: VisionUserSession,
                                    var serviceDefinition: ServiceDefinition) : VisionMappingBuilder("POST") {

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
    }
    fun respondWithSuccess(configuration: Configuration): Mapping {
        val jaxbContext = JAXBContext.newInstance(Configuration::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(configuration, stringWriter)
        }

        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionResponse(stringWriter.toString(), serviceDefinition)).build()
        }

        return resp
    }

    fun respondWithInvalidRequest(): Mapping {
        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getInvalidRequestError(serviceDefinition)).build()
        }
        return resp
    }

    fun respondWithSecurityHeaderError(): Mapping {
        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(securityHeaderErrorResponse).build()
        }
        return resp
    }

    fun respondWithUnknownError(): Mapping {
        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getUnkownError(serviceDefinition)).build()
        }
        return resp
    }

    fun respondWithServiceUnavailable(): Mapping {
        var resp = respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("").build()
        }
        return resp
    }

    fun respondWitInvalidUserCredentials(): Mapping {
        var resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getInvalidUserCredentialsError(serviceDefinition)).build()
        }
        return resp
    }
}



