package mocking

import mocking.models.Mapping

class ExternalSupplierMockingClient<T>(private val mappingBuilder :  T, private val wiremockHelper: WiremockHelper){

    fun mock(resolver: T.() -> Mapping) {
        val mapping = mappingBuilder.resolver()
        wiremockHelper.postMapping(mapping)
    }
}