package mocking.vision

import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getClinicalDataResponse
import mocking.vision.VisionMappingBuilder
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import org.apache.http.HttpStatus

class VisionGetPatientDataBuilder(private val userSession: VisionUserSession, private val serviceDefinition:
ServiceDefinition, private val view: String, private val responseFormat: String) : VisionMappingBuilder("POST"){

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
                .andBody(view, "contains")
                .andBody(responseFormat, "contains")

    }

    fun respondWithSuccess (clinicalData: String) : Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getClinicalDataResponse(clinicalData, serviceDefinition))
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