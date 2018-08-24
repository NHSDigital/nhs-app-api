package mocking.ndop

import mocking.MappingBuilder

open class NdopMappingBuilder(method: String)
    : MappingBuilder(method, "/ndop/ndopapp-build1.thunderbird.service.nhs.uk/createsession") {

    init {
        // no generic additions to the request
    }

    fun linkToNdopRequest() = NdopLinkRequestBuilder()
}