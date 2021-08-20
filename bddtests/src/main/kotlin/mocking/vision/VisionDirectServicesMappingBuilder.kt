package mocking.vision

import mocking.MappingBuilder

abstract class VisionDirectServicesMappingBuilder(orgId: String, path: String) :
        MappingBuilder(method = "POST", url = "/vision/v1/organisations/$orgId/onlineservices/$path")
