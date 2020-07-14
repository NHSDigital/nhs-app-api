package mocking

import mocking.models.Mapping

class ExternalSupplierMockingClient<T>(private val mappingBuilder :  T, private val wiremockSetup: WiremockSetup){

    fun mock(resolver: T.() -> Mapping) {
        val mapping = mappingBuilder.resolver()
        wiremockSetup.postMapping(mapping)
    }
}
