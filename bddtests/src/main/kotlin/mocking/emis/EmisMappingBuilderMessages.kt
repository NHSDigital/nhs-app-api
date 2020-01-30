package mocking.emis

import mocking.emis.patientPracticeMessaging.EmisMessageReadStatusUpdateBuilder
import mocking.emis.patientPracticeMessaging.EmisCreateMessageBuilder
import mocking.emis.patientPracticeMessaging.EmisMessagingBuilder
import mocking.emis.patientPracticeMessaging.EmisMessagingConverationBuilder
import mocking.emis.patientPracticeMessaging.EmisMessagingRecipientsBuilder
import models.Patient
import worker.models.patientPracticeMessaging.CreateMessageRequest

class EmisMappingBuilderMessages(private var configuration: EmisConfiguration?) {
    fun viewMyMessagesRequest(patient: Patient) = EmisMessagingBuilder(
            configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId)

    fun viewConversationRequest(patient: Patient) = EmisMessagingConverationBuilder(
            configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId
    )

    fun updateReadStatusRequest(patient: Patient) = EmisMessageReadStatusUpdateBuilder(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId
    )

    fun getRecipientsRequest(patient: Patient) = EmisMessagingRecipientsBuilder(
            configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId
    )

    fun sendMessageRequest(patient: Patient, createMessageRequest: CreateMessageRequest)
            = EmisCreateMessageBuilder(
            configuration!!,
            patient.userPatientLinkToken,
            patient.endUserSessionId,
            patient.sessionId,
            createMessageRequest)

}