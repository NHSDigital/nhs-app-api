package mocking.nhsAzureSearchService

import mocking.GsonFactory
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

    class NhsAzureSearchResultsBuilder(
                                    requestBody: NhsAzureSearchRequestBody) :
        NhsAzureSearchMappingBuilder
                ("POST") {
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

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "Requested record access is disabled by the practice")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}