package mocking.stubs.patientPracticeMessaging

import mocking.MockingClient
import mocking.data.messaging.MessagingData
import mocking.emis.patientPracticeMessaging.EmisMessagingBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import models.Patient

class PatientPracticeMessagingStubs(private val mockingClient: MockingClient) {

    fun generateEMISStubs() {
        val messagesDataLoader = MessagingData.getDefaultMessagesData(REPLY_COUNT,true)
        val mapEMISPatientPracticeMessageRequestStubs =
            InputResponse<Patient, EmisMessagingBuilder>()
                .addResponse(EmisStubsPatientFactory.goodPatientEMIS) {
                    builder -> builder.respondWithSuccess(messagesDataLoader)
                }
        mapEMISPatientPracticeMessageRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis {
                scenario.getResponse(messaging.viewMyMessagesRequest(scenario.forMatcher))
            }
        }
    }

    companion object {
        const val REPLY_COUNT = 3;
    }
}