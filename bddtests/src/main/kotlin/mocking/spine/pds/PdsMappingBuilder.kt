package mocking.spine.pds

import mocking.spine.SpineMappingBuilder

open class PdsMappingBuilder(soapAction: String)
    : SpineMappingBuilder(method="POST", relativePath= "/syncservice-pds/pds", soapAction = soapAction) {

    init {
        requestBuilder
    }
}
