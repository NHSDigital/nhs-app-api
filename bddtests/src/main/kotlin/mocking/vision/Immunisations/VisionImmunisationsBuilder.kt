package mocking.vision.Immunisations

import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getVisionImmunisationsResponse
import mocking.vision.VisionMappingBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus

class VisionImmunisationsBuilder(private val userSession: VisionUserSession, private val serviceDefinition:
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
                .andBody("PROCEDURES", "contains")
    }

    fun respondWithSuccess (immunisations: String) : Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionImmunisationsResponse(immunisations, serviceDefinition))
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
