package mocking.nhsAzureSearchService

import mocking.models.Mapping
import org.apache.http.HttpStatus

class NhsAzureSearchResultsBuilder(requestBody: NhsAzureSearchRequestBody) : NhsAzureSearchMappingBuilder("POST") {
    init {
        requestBuilder
                .andJsonBody(requestBody)
    }

    fun respondWithSuccess(model: NHSAzureSearchReply): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }
}