package mocking.nhsAzureSearchService

import mocking.GsonFactory
import mocking.models.Mapping
import org.apache.http.HttpStatus

class NhsAzureOrganisationResultsBuilder(requestBody: NhsAzureSearchOrganisationRequestBody?) :
        NhsAzureSearchOrganisationMappingBuilder("POST") {
    init {
        if(requestBody != null) {
            requestBuilder
                    .andJsonBody(requestBody, "equalToJson", GsonFactory.asIsSerializeNulls)
        }
    }

    fun respondWithSuccess(model: NhsAzureSearchOrganisationReply): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }
}

class NhsAzurePostcodeOrganisationResultsBuilder(requestBody: NhsAzureSearchOrganisationWithPostcodeRequestBody?) :
        NhsAzureSearchOrganisationMappingBuilder("POST") {
    init {
        if(requestBody != null) {
            requestBuilder
                    .andJsonBody(requestBody, "equalToJson", GsonFactory.asIsSerializeNulls)
        }
    }

    fun respondWithSuccess(model: NhsAzureSearchOrganisationReply): Mapping {
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
