package mocking.tpp

import mocking.tpp.patientpracticemessaging.TppViewMessagesBuilder
import worker.models.demographics.TppUserSession

class TppMappingBuilderPatientPracticeMessaging {
    fun viewMessagesRequest(tppUserSession: TppUserSession)  = TppViewMessagesBuilder(tppUserSession);
}