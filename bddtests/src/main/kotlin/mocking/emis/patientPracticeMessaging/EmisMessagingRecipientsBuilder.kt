package mocking.emis.patientPracticeMessaging

import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.models.Mapping
import mocking.sharedModels.MessageRecipientsResponseModel
import wiremock.org.apache.http.HttpStatus

class EmisMessagingRecipientsBuilder(configuration: EmisConfiguration?,
                                     linkToken: String,
                                     apiEndUserSessionId: String,
                                     apiSessionId: String) : EmisMappingBuilder(
        configuration, "GET", "/messagerecipients") {
    init {
        requestBuilder
                .andHeader(mocking.emis.HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(mocking.emis.HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(recipients: MessageRecipientsResponseModel): Mapping {
        return respondWith(HttpStatus.SC_OK){
            andJsonBody(recipients).build()
        }
    }
}