package mocking.emis.documents

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisDocumentsBuilder(configuration: EmisConfiguration,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/record") {

    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
                .andQueryParameter("itemType", "Documents", "equalTo")
    }

    fun respondWithSuccess(documentsResponse: DocumentsResponseModel): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(documentsResponse)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "Requested record access is disabled by the practice")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }
}