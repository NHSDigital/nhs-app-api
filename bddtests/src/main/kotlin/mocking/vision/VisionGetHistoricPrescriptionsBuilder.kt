package mocking.vision

import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionResponse
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionGetHistoricPrescriptionsBuilder(var userSession: VisionUserSession,
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
                .andBody(userSession.patientId, "contains")
                .andBody(serviceDefinition.name, "contains")
    }

    fun respondWithSuccess(prescriptionHistory: PrescriptionHistory): Mapping {
        val jaxbContext = JAXBContext.newInstance(PrescriptionHistory::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(prescriptionHistory, stringWriter)
        }

        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionResponse(stringWriter.toString(), serviceDefinition)).build()
        }

        return resp
    }
}



