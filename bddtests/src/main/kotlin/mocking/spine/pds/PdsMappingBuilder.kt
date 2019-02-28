package mocking.spine.pds

import mocking.spine.SpineMappingBuilder

open class PdsMappingBuilder()
    : SpineMappingBuilder(method="POST", relativePath= "/syncservice-pds/pds") {

    companion object {
    }

    init {
        requestBuilder
    }
}
