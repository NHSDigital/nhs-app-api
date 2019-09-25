package mocking.nhsAzureSearchService

import config.Config
import mocking.CONTENT_TYPE_APPLICATION_JSON
import mocking.MappingBuilder

const val HEADER_API_KEY = "subscription-key"
const val HEADER_API_CONTENT_TYPE = "Content-Type"
const val API_VERSION_QUERY_NAME = "api-version"
const val API_VERSION_QUERY_VALUE = "1"

open class NhsAzureSearchOrganisationMappingBuilder(method: String)
    : MappingBuilder(method, "/nhs-search-indexes/service-search/search") {

    init {
            requestBuilder
                    .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                    .andHeader(HEADER_API_CONTENT_TYPE, CONTENT_TYPE_APPLICATION_JSON)
                    .andQueryParameter(API_VERSION_QUERY_NAME, API_VERSION_QUERY_VALUE)
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}

open class NhsAzureSearchPostcodesAndPlacesMappingBuilder(method: String)
    : MappingBuilder(method, "/nhs-search-indexes/service-search/postcodesandplaces/search") {

    init {
        requestBuilder
                .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                .andHeader(HEADER_API_CONTENT_TYPE, CONTENT_TYPE_APPLICATION_JSON)
                .andQueryParameter(API_VERSION_QUERY_NAME, API_VERSION_QUERY_VALUE)
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}

open class NhsAzureSearchPostcodesMappingBuilder(method: String)
    : MappingBuilder(method, "/nhs-search-indexes/service-search/postcodesandplaces/search?api-version=1") {

    init {
        requestBuilder
                .andHeader(HEADER_API_KEY, Config.instance.gpLookupApiKey)
                .andHeader(HEADER_API_CONTENT_TYPE, CONTENT_TYPE_APPLICATION_JSON)
                .andQueryParameter(API_VERSION_QUERY_NAME, API_VERSION_QUERY_VALUE)
    }

    var nhsAzureSearch = NhsAzureSearchServiceMappingBuilder()

}
