package mocking.emis

import mocking.emis.patientPracticeMessaging.EmisMessagingBuilder
import mocking.emis.patientPracticeMessaging.EmisMessagingConverationBuilder
import models.Patient

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

}