package mocking.stubs.patientPracticeMessaging

import mocking.MockingClient
import mocking.data.patientPracticeMessaging.MessagingData
import mocking.emis.patientPracticeMessaging.EmisDeleteConversationBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import models.Patient

class PatientPracticeDeleteConversationStubs(private val mockingClient: MockingClient) {

    fun generateEMISStubs() {
        val messageDeleteResponseLoader = MessagingData.getDeleteResponse()
        val mapEMISPatientPracticeMessageDeleteRequestStubs =
                InputResponse<Patient, EmisDeleteConversationBuilder>()
                        .addResponse(EmisStubsPatientFactory.goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(messageDeleteResponseLoader)
                        }
        mapEMISPatientPracticeMessageDeleteRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(messaging.deleteConversationRequest(scenario.forMatcher)) }
        }
    }
}