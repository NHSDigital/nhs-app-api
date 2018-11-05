package mocking.vision.demographics

import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionDemographicsResponse
import mocking.vision.VisionErrorResponses.getAccessDeniedError
import mocking.vision.VisionErrorResponses.getUnknownError
import mocking.vision.VisionMappingBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionDemographicsBuilder(var userSession: VisionUserSession,
                                var serviceDefinition: ServiceDefinition)
    : VisionMappingBuilder("POST") {

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
                .andBody(serviceDefinition.name, "contains")
                .andBody(userSession.patientId, "contains")
    }

    fun respondWithSuccess(demographics: Demographics): Mapping {
        val jaxbContext = JAXBContext.newInstance(Demographics::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(demographics, stringWriter)
        }

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionDemographicsResponse(stringWriter.toString(), serviceDefinition)).build()
        }

        return resp
    }

    fun respondWithUnknownError(): Mapping {
        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getUnknownError(serviceDefinition)).build()
        }
        return resp
    }

    fun respondWithAccessDeniedError(): Mapping {
        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getAccessDeniedError(serviceDefinition)).build()
        }
        return resp
    }
}



