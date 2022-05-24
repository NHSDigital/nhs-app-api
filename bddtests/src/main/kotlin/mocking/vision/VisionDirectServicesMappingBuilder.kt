package mocking.vision

import mocking.MappingBuilder
import mocking.models.Mapping

abstract class VisionDirectServicesMappingBuilder(orgId: String, path: String) :
    MappingBuilder("POST", "/vision/v1/organisations/$orgId/onlineservices/$path") {

    fun respondWithThrottling(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }
}
