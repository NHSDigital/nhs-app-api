package mocking.nhsAzureSearchService

class NhsAzureSearchServiceMappingBuilder{

    fun nhsAzureSearchOrganisationRequest(requestBody: NhsAzureSearchOrganisationRequestBody) =
            NhsAzureOrganisationResultsBuilder(requestBody)

    fun nhsAzureSearchPostcodesAndPlacesRequest(requestBody: NhsAzureSearchPostcodesAndPlacesRequestBody) =
            NhsAzurePostcodesAndPlacesResultsBuilder(requestBody)

}
