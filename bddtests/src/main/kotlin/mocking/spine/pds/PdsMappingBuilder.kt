package mocking.spine.pds

import mocking.spine.SpineMappingBuilder

open class PdsMappingBuilder
    : SpineMappingBuilder(method="POST", relativePath= "/syncservice-pds/pds") {

    init {
        requestBuilder
    }
}
