package mocking.tpp

import mocking.tpp.patientpracticemessaging.TppMarkMessagesAsReadBuilder
import mocking.tpp.patientpracticemessaging.TppPatientPracticeMessagingRecipientsBuilder
import mocking.tpp.patientpracticemessaging.TppViewMessagesBuilder
import mocking.tpp.documents.TppRequestBinaryDataBuilder
import mocking.tpp.patientpracticemessaging.TppPatientPracticeMessagingCreateMessageBuilder
import worker.models.demographics.TppUserSession
import worker.models.patientPracticeMessaging.CreateMessageRequest

class TppMappingBuilderPatientPracticeMessaging {
    fun viewMessagesRequest(tppUserSession: TppUserSession)  = TppViewMessagesBuilder(tppUserSession)
    fun requestRecipientsRequest(tppUserSession: TppUserSession) =
        TppPatientPracticeMessagingRecipientsBuilder(tppUserSession)
    fun markMessagesAsReadRequest(tppUserSession: TppUserSession) =
        TppMarkMessagesAsReadBuilder(tppUserSession)
    fun attachmentRequest(tppUserSession: TppUserSession) = TppRequestBinaryDataBuilder(tppUserSession)
    fun createMessageRequest(tppUserSession: TppUserSession, createMessageRequest: CreateMessageRequest) =
            TppPatientPracticeMessagingCreateMessageBuilder(tppUserSession, createMessageRequest)
}
