package mocking.stubs.patientPracticeMessaging

import mocking.MockingClient
import mocking.data.patientPracticeMessaging.EmisMessagingData
import mocking.emis.patientPracticeMessaging.EmisMessagingConverationBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import models.Patient

class PatientPracticeConversationStubs(private val mockingClient: MockingClient) {

    fun generateEMISStubs() {
        val messagesDataLoader = EmisMessagingData.getDefaultMessageDetailsWithReplies()
        val mapEMISPatientPracticeMessageRequestStubs =
                InputResponse<Patient, EmisMessagingConverationBuilder>()
                        .addResponse(EmisStubsPatientFactory.goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(messagesDataLoader)
                        }
        mapEMISPatientPracticeMessageRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(messaging.viewConversationRequest(scenario.forMatcher)) }
        }
    }
}