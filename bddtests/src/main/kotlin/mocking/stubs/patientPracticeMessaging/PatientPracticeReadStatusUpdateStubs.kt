package mocking.stubs.patientPracticeMessaging

import mocking.MockingClient
import mocking.data.messaging.MessagingData
import mocking.emis.patientPracticeMessaging.EmisMessageReadStatusUpdateBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import models.Patient

class PatientPracticeReadStatusUpdateStubs (private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val messagesDataLoader = MessagingData.getUpdatedResponse()
        val mapEMISPatientPracticeMessageRequestStubs =
                InputResponse<Patient, EmisMessageReadStatusUpdateBuilder>()
                        .addResponse(EmisStubsPatientFactory.goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(messagesDataLoader)
                        }
        mapEMISPatientPracticeMessageRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(messaging.updateReadStatusRequest(scenario.forMatcher)) }
        }
    }
}