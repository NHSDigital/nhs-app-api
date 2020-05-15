package mocking.stubs.patientPracticeMessaging

import mocking.MockingClient
import mocking.data.patientPracticeMessaging.EmisMessagingData
import mocking.emis.patientPracticeMessaging.EmisMessagingRecipientsBuilder
import mocking.stubs.EmisStubsPatientFactory
import mocking.stubs.InputResponse
import models.Patient

class PatientPracticeMessagingRecipientsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val recipientsData = EmisMessagingData.getDefaultMessageRecipients()
        val stubs = InputResponse<Patient, EmisMessagingRecipientsBuilder>()
        stubs.addResponse(EmisStubsPatientFactory.goodPatientEMIS) {
            builder -> builder.respondWithSuccess(recipientsData)
        }
        stubs.listResponse().forEach { scenario ->
            mockingClient.forEmis.mock {
                scenario.getResponse(messaging.getRecipientsRequest(scenario.forMatcher))
            }
        }
    }
}