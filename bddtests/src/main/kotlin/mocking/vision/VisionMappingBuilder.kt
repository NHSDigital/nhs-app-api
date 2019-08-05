package mocking.vision

import constants.ErrorResponseCodeVision
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.ServiceDefinition
import org.apache.http.HttpStatus

abstract class VisionMappingBuilder(method: String = "POST") : MappingBuilder(method, "/vision/pfs/") {

    fun respondWithCorruptedContent(serviceDefinition: ServiceDefinition, content: String = ""): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            val corruptedResponse = VisionConstantsHelper
                    .getBaseVisionResponse(content, serviceDefinition)
                    .replace(">", "|")
                    .replace("}", "|")

            andBody(corruptedResponse, contentType = "text/xml")
        }
    }

    fun respondVisionErrorWhenServiceNotEnabled(serviceDefinition: ServiceDefinition):Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.ACCESS_DENIED)).build()
        }
    }

    fun respondWithUnknownVisionError(serviceDefinition: ServiceDefinition):Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.NON_VISION_ERROR_CODE)).build()
        }
    }
}
