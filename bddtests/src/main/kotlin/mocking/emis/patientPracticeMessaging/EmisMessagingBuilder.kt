package mocking.emis.patientPracticeMessaging

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import wiremock.org.apache.http.HttpStatus
import mocking.models.Mapping
import mocking.sharedModels.MessagesResponseModel

class EmisMessagingBuilder(configuration: EmisConfiguration?,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String) : EmisMappingBuilder(configuration, "GET", "/messages") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(messagesResponse: MessagesResponseModel): Mapping{
        return respondWith(HttpStatus.SC_OK){
            andJsonBody(messagesResponse).build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "Patient practice messaging is disabled by the practice")
        return respondWithException(exceptionResponse)
    }

    fun respondWithBadRequest(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_BAD_REQUEST.toLong(),
                "Unknown error")
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }

    fun respondWithForbidden(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_FORBIDDEN.toLong(),
                "Forbidden error")
        return respondWith(HttpStatus.SC_FORBIDDEN) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }
}