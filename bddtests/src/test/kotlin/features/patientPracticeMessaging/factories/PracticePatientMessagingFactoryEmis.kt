package features.patientPracticeMessaging.factories

import mocking.data.messaging.MessagingData
import mocking.emis.models.PatientPracticeMessagingMessageTypes
import mocking.emis.patientPracticeMessaging.MessagesResponseModel
import mocking.emis.patientPracticeMessaging.PatientMessageSummary
import models.ExpectedMessage
import models.Patient
import utils.SerenityHelpers

class PracticePatientMessagingFactoryEmis: PracticePatientMessagingFactory() {

    override fun disabled(patient: Patient) {
            mockingClient.forEmis {
            messaging.viewMyMessagesRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun patientHasNoMessages(patient: Patient){
        mockingClient.forEmis{
            messaging.viewMyMessagesRequest(patient)
                    .respondWithSuccess(MessagesResponseModel(mutableListOf<PatientMessageSummary>()))
        }
    }

    override fun errorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forEmis {
            messaging.viewMyMessagesRequest(patient).respondWithBadRequest()
        }
    }

    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        mockingClient.forEmis {
            messaging.viewConversationRequest(patient).respondWithBadRequest()
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient){
        val messages = MessagingData.getDefaultMessagesData()

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingMessageTypes.EXPECTED_MESSAGES, getExpectedMessages(messages.messages))

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingMessageTypes.AVAILABLE_MESSAGE, MessagingData.getMessagesWithReplies())


        mockingClient.forEmis{
            messaging.viewMyMessagesRequest(patient)
                    .respondWithSuccess(messages)
        }
        mockingClient.forEmis{
            messaging.viewConversationRequest(patient)
                    .respondWithSuccess(MessagingData.getMessagesWithReplies())
        }
    }

    override fun getExpectedMessages(expectedMessages: List<PatientMessageSummary>): List<ExpectedMessage>{
        return expectedMessages.map(fun(message): ExpectedMessage {
            return ExpectedMessage(
                    message.messageId,
                    message.subject,
                    "18 February 2018",
                    message.recipients.first().name)
        })
    }
}