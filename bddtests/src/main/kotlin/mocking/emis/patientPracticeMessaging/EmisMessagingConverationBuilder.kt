package mocking.emis.patientPracticeMessaging

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import wiremock.org.apache.http.HttpStatus
import mocking.models.Mapping
import mocking.patientPracticeMessaging.MessageResponseModel

class EmisMessagingConverationBuilder(configuration: EmisConfiguration?,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String) : EmisMappingBuilder(
        configuration, "GET", "/messages/10/") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(messagesResponse: MessageResponseModel): Mapping{
        return respondWith(HttpStatus.SC_OK){
            andJsonBody(messagesResponse).build()
        }
    }

    fun respondWithBadRequest(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_BAD_REQUEST.toLong(),
                "Unknown error")
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }
}
