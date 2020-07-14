package mocking.emis.patientPracticeMessaging

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.ExceptionResponse
import mocking.emis.models.PostPatientMessageRequest
import mocking.emis.models.PostPatientMessageResponse
import worker.models.patientPracticeMessaging.CreateMessageRequest
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisCreateMessageBuilder(configuration: EmisConfiguration?,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String,
                           createMessageRequest: CreateMessageRequest?) :
        EmisMappingBuilder(configuration, "POST", "/messages") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)

        if(createMessageRequest != null){
            val recipients = arrayListOf<String>()
            recipients.add(createMessageRequest.recipient)

            val request = PostPatientMessageRequest(createMessageRequest.subject, createMessageRequest.messageBody,
                   recipients, linkToken)

            requestBuilder.andJsonBody(request)
        }
    }

    fun respondWithSuccess(): Mapping {
        val responseBody = PostPatientMessageResponse(true)
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody, GsonFactory.asPascal)
        }
    }

    fun respondWithBadRequest(): Mapping {
        val exceptionResponse = ExceptionResponse(wiremock.org.apache.http.HttpStatus.SC_BAD_REQUEST.toLong(),
                "Unknown error")
        return respondWith(wiremock.org.apache.http.HttpStatus.SC_BAD_REQUEST) {
            andJsonBody(exceptionResponse, GsonFactory.asPascal)
        }
    }
}
