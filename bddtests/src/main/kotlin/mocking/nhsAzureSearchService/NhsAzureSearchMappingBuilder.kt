package mocking.nhsAzureSearchService

import config.Config
import mocking.MappingBuilder

const val HEADER_API_KEY = "subscription-key"
const val HEADER_API_CONTENT_TYPE = "Content-Type"

open class NhsAzureSearchOrganisationMappingBuilder(method: String)
    : MappingBuilder(method, "/indexes/organisationlookup/docs/search") {

    init {
            requestBuilder
                    .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                    .andHeader(HEADER_API_CONTENT_TYPE, "application/json")
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}

open class NhsAzureSearchPostcodesAndPlacesMappingBuilder(method: String)
    : MappingBuilder(method, "/indexes/postcodesandplaces/docs/search") {

    init {
        requestBuilder
                .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                .andHeader(HEADER_API_CONTENT_TYPE, "application/json")
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}
