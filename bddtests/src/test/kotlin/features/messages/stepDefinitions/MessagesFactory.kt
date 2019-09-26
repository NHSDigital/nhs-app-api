package features.messages.stepDefinitions

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import utils.SerenityHelpers
import utils.set
import worker.models.messages.MessageRequest

class MessagesFactory {

    val mockingClient = MockingClient.instance

    fun setUpUser(gpSystem: String, patient: Patient? = null) {
        SerenityHelpers.setGpSupplier(gpSystem)
        val patientToUse = patient ?: Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patientToUse)
        MongoDBConnection.MessagesCollection.clearCache()
    }

    fun setUpMultipleMessagesInCache() {
        MongoDBConnection.MessagesCollection.clearCache()
        val nhsLoginId = SerenityHelpers.getPatient().subject
        val expectedMessages = arrayListOf(
                messageRequest("Message One", "Sender One"),
                messageRequest("Message Two", "Sender One"),
                messageRequest("Message Three", "Sender Two"))
        expectedMessages.forEach { message -> MessagesApi.post(message, nhsLoginId) }
        MessagesSerenityHelpers.EXPECTED_MESSAGES.set(expectedMessages)
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        MongoDBConnection.MessagesCollection.assertNumberOfDocuments(expectedMessages.count())
    }

    private fun messageRequest(message: String, sender: String): MessageRequest {
        return MessageRequest(
                sender = sender,
                body = message,
                version = 1)
    }
}