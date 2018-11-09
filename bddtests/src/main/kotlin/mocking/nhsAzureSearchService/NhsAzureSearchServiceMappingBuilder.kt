package mocking.nhsAzureSearchService

class NhsAzureSearchServiceMappingBuilder{

    fun nhsAzureSearchRequest(requestBody: NhsAzureSearchRequestBody) =
            NhsAzureSearchResultsBuilder(requestBody)

}
