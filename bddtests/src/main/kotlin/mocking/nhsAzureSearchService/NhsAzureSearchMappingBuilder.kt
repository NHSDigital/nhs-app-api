package mocking.nhsAzureSearchService

import config.Config
import mocking.MappingBuilder

const val HEADER_API_KEY = "subscription-key"
const val HEADER_API_CONTENT_TYPE = "Content-Type"

open class NhsAzureSearchMappingBuilder(
                              method: String)
    : MappingBuilder(method, "/indexes/organisationlookup/docs/search") {

    init {
            requestBuilder
                    .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                    .andHeader(HEADER_API_CONTENT_TYPE, "application/json")
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}
