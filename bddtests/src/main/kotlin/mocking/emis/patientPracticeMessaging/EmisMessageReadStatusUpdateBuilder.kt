package mocking.emis.patientPracticeMessaging

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import wiremock.org.apache.http.HttpStatus
import mocking.models.Mapping
import mocking.patientPracticeMessaging.MessageReadStatusUpdateResponse


class EmisMessageReadStatusUpdateBuilder(configuration: EmisConfiguration?,
                                      apiEndUserSessionId: String,
                                      apiSessionId: String) : EmisMappingBuilder(
        configuration, "PUT", "/messages") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
    }

    fun respondWithSuccess(messageStatusUpdateResponse: MessageReadStatusUpdateResponse): Mapping{
        return respondWith(HttpStatus.SC_OK){
            andJsonBody(messageStatusUpdateResponse).build()
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
