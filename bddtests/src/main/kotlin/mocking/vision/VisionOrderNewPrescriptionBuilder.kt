package mocking.vision

import mocking.models.Mapping
import mocking.vision.VisionConstants.getVisionResponse
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus

class VisionOrderNewPrescriptionBuilder(
        val userSession: VisionUserSession,
        val serviceDefinition: ServiceDefinition,
        val orderNewPrescriptionRequest: OrderNewPrescriptionRequest) : VisionMappingBuilder("POST") {

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
                .andBody(serviceDefinition.version, "contains")

        if (!orderNewPrescriptionRequest.repeat.isEmpty()) {
            for (repeat in orderNewPrescriptionRequest.repeat) {
                requestBuilder.andBody("<vision:repeat id=\"${repeat.id}\" />", "contains")
            }
        }
    }

    fun respondWithSuccess(orderNewPrescriptionResponseText: String): Mapping {
        val resp = respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionResponse(orderNewPrescriptionResponseText, serviceDefinition)).build()
        }

        return resp
    }
}
