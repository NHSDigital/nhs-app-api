package mocking.vision.allergies

import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getVisionAllergiesResponse
import mocking.vision.VisionMappingBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus

class VisionAllergiesBuilder(private val userSession: VisionUserSession, private val serviceDefinition:
        ServiceDefinition) : VisionMappingBuilder("POST"){

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
                .andBody(VisionConstants.ALLERGIES_VIEW, "contains")
                .andBody(VisionConstants.HTML_RESPONSE_FORMAT, "contains")
    }

    fun respondWithSuccess (allergies: String) : Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionAllergiesResponse(allergies, serviceDefinition))
        }
    }

    fun respondWithAccessDeniedError(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstants.getAccessDeniedError(serviceDefinition))
        }
    }

    fun respondWithUnknownError(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstants.getUnknownError(serviceDefinition))
        }
    }

}