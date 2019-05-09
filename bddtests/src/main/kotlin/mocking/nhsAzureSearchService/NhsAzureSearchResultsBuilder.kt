package mocking.nhsAzureSearchService

import mocking.models.Mapping
import org.apache.http.HttpStatus

class NhsAzureOrganisationResultsBuilder(requestBody: NhsAzureSearchOrganisationRequestBody?) :
        NhsAzureSearchOrganisationMappingBuilder("POST") {
    init {
        if(requestBody != null) {
            requestBuilder
                    .andJsonBody(requestBody)
        }
    }

    fun respondWithSuccess(model: NHSAzureSearchOrganisationReply): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }
}

class NhsAzurePostcodesAndPlacesResultsBuilder(requestBody: NhsAzureSearchPostcodesAndPlacesRequestBody) :
        NhsAzureSearchPostcodesAndPlacesMappingBuilder("POST") {
    init {
        requestBuilder
                .andJsonBody(requestBody)
    }

    fun respondWithSuccess(model: NHSAzureSearchPostcodesAndPlacesReply): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }
}