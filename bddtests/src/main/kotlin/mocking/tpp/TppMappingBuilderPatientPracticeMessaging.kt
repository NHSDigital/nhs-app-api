package mocking.tpp

import mocking.tpp.documents.TppRequestBinaryDataBuilder
import mocking.tpp.patientPracticeMessaging.TppPatientPracticeMessagingCreateMessageBuilder
import mocking.tpp.patientPracticeMessaging.TppPatientPracticeMessagingRecipientsBuilder
import mocking.tpp.patientPracticeMessaging.TppViewMessagesBuilder
import worker.models.demographics.TppUserSession
import worker.models.patientPracticeMessaging.CreateMessageRequest

class TppMappingBuilderPatientPracticeMessaging {
    fun viewMessagesRequest(tppUserSession: TppUserSession)  = TppViewMessagesBuilder(tppUserSession);
    fun requestRecipientsRequest(tppUserSession: TppUserSession) =
            TppPatientPracticeMessagingRecipientsBuilder(tppUserSession)
    fun attachmentRequest(tppUserSession: TppUserSession) = TppRequestBinaryDataBuilder(tppUserSession)

    fun createMessageRequest(tppUserSession: TppUserSession, createMessageRequest: CreateMessageRequest) =
            TppPatientPracticeMessagingCreateMessageBuilder(tppUserSession, createMessageRequest)
}