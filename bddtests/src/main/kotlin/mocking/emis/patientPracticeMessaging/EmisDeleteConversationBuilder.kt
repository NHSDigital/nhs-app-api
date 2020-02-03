package mocking.emis.patientPracticeMessaging

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.DeletePatientConversationRequest
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import wiremock.org.apache.http.HttpStatus

class EmisDeleteConversationBuilder(configuration: EmisConfiguration?,
                                    linkToken: String,
                                    apiEndUserSessionId: String,
                                    apiSessionId: String) : EmisMappingBuilder(
        configuration, "DELETE", "/messages/1") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)

        val request = DeletePatientConversationRequest(linkToken)

        requestBuilder.andJsonBody(request)
    }

    fun respondWithSuccess(conversationDeletedResponse: ConversationDeletedResponse): Mapping{
        return respondWith(HttpStatus.SC_NO_CONTENT){
            andJsonBody(conversationDeletedResponse).build()
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