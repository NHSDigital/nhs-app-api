package mocking.vision

import mocking.models.Mapping
import mocking.vision.models.EligibleRepeats
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller

class VisionEligibleRepeatsBuilder(var userSession: VisionUserSession,
                                   var serviceDefinition: ServiceDefinition) :
        VisionDirectServicesMappingBuilder(orgId = userSession.odsCode, path = "eligibleRepeats") {

    init {
        requestBuilder
                .andBody("<vision:rosuAccountId>${userSession.rosuAccountId}</vision:rosuAccountId>", "contains")
                .andBody("<vision:apiKey>${userSession.apiKey}</vision:apiKey>", "contains")
                .andBody("<vision:provider>${userSession.provider}</vision:provider>", "contains")
                .andBody("<vision:accountId>${userSession.accountId}</vision:accountId>", "contains")
                .andBody("<vision:patientId>${userSession.patientId}</vision:patientId>", "contains")
    }
    fun respondWithSuccess(EligibleRepeats: EligibleRepeats): Mapping {
        val jaxbContext = JAXBContext.newInstance(EligibleRepeats::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(EligibleRepeats, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(
                    VisionConstants.getVisionDirectServicesResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }
}



