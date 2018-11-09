package mocking.nhsAzureSearchService

import mocking.MappingBuilder

const val HEADER_API_KEY = "api-key"
const val HEADER_API_CONTENT_TYPE = "Content-Type"
const val QUERY_PARAM_API_VERSION = "api-version"

open class NhsAzureSearchMappingBuilder(
                              method: String)
    : MappingBuilder(method, "/indexes/organisationlookup/docs/search") {

    init {
            requestBuilder
                    .andHeader(HEADER_API_KEY,"388E210ABC158F9CE12218173E2BBD4B")
                    .andHeader(HEADER_API_CONTENT_TYPE, "application/json")
    }


    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}
