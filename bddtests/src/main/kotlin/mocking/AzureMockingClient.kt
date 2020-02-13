package mocking

import mocking.models.Mapping
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationMappingBuilder
import mocking.nhsAzureSearchService.NhsAzureSearchPostcodesMappingBuilder

class AzureMockingClient(configuration: MockingConfiguration): WiremockHelper(configuration) {

    fun forSearchOrganisation(method: String = "POST", resolver:
    NhsAzureSearchOrganisationMappingBuilder.() -> Mapping) {
        val mappingBuilder = NhsAzureSearchOrganisationMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }

    fun forSearchPostcodes(method: String = "GET", resolver:
    NhsAzureSearchPostcodesMappingBuilder.() -> Mapping) {
        val mappingBuilder = NhsAzureSearchPostcodesMappingBuilder(method)
        val mapping: Mapping = mappingBuilder.resolver()

        this.postMapping(mapping)
    }
}
