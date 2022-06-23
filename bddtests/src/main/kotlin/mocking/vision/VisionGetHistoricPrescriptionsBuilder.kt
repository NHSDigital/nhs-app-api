package mocking.vision

import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionDirectServicesResponse
import mocking.vision.VisionDirectServicesErrorResponses.getAccessDeniedError
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionGetHistoricPrescriptionsBuilder(var userSession: VisionUserSession,
                                   var serviceDefinition: ServiceDefinition) :
        VisionDirectServicesMappingBuilder(orgId = userSession.odsCode, path = "history") {

    init {
        requestBuilder
                .andBody("<vision:rosuAccountId>${userSession.rosuAccountId}</vision:rosuAccountId>", "contains")
                .andBody("<vision:apiKey>${userSession.apiKey}</vision:apiKey>", "contains")
                .andBody("<vision:provider>${userSession.provider}</vision:provider>", "contains")
                .andBody("<vision:accountId>${userSession.accountId}</vision:accountId>", "contains")
                .andBody("<vision:patientId>${userSession.patientId}</vision:patientId>", "contains")
    }

    fun respondWithSuccess(prescriptionHistory: PrescriptionHistory): Mapping {
        val jaxbContext = JAXBContext.newInstance(PrescriptionHistory::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(prescriptionHistory, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionDirectServicesResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }

    fun respondWithAccessDeniedError(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) {
            andXmlBody(getAccessDeniedError(serviceDefinition)).build()
        }
    }
}





