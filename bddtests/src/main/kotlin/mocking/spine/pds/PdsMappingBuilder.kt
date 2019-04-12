package mocking.spine.pds

import mocking.spine.SpineMappingBuilder

open class PdsMappingBuilder(soapAction: String)
    : SpineMappingBuilder(method="POST", relativePath= "/sync-service", soapAction = soapAction) {

    init {
        requestBuilder
    }
}
