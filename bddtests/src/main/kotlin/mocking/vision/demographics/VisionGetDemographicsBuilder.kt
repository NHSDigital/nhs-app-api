package mocking.vision.demographics

import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionDirectServicesResponse
import mocking.vision.VisionDirectServicesErrorResponses.getAccessDeniedError
import mocking.vision.VisionDirectServicesErrorResponses.getUnknownError
import mocking.vision.VisionDirectServicesMappingBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionGetDemographicsBuilder(var userSession: VisionUserSession,
                                   var serviceDefinition: ServiceDefinition) :
    VisionDirectServicesMappingBuilder(orgId = userSession.odsCode, path = "demographics") {

    init {
        requestBuilder
            .andBody("<vision:rosuAccountId>${userSession.rosuAccountId}</vision:rosuAccountId>", "contains")
            .andBody("<vision:apiKey>${userSession.apiKey}</vision:apiKey>", "contains")
            .andBody("<vision:provider>${userSession.provider}</vision:provider>", "contains")
            .andBody("<vision:accountId>${userSession.accountId}</vision:accountId>", "contains")
            .andBody("<vision:patientId>${userSession.patientId}</vision:patientId>", "contains")
    }

    fun respondWithSuccess(demographics: Demographics): Mapping {
        val jaxbContext = JAXBContext.newInstance(Demographics::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(demographics, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionDirectServicesResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }

    fun respondWithUnknownError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(getUnknownError(serviceDefinition)).build()
        }
    }

    fun respondWithAccessDeniedError(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) {
            andXmlBody(getAccessDeniedError(serviceDefinition)).build()
        }
    }
}
