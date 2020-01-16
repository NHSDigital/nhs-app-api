package features.patientPracticeMessaging.factories

import mocking.data.messaging.MessagingData
import mocking.emis.models.PatientPracticeMessagingTypes
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

    override fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean){
        val messages = MessagingData.getDefaultMessagesData(REPLY_COUNT, hasUnread)
        setUpMessageDataAndStubs(patient, messages)
    }

    override fun getExpectedMessages(expectedMessages: List<PatientMessageSummary>): List<ExpectedMessage>{
        return expectedMessages.map(fun(message): ExpectedMessage {
            return ExpectedMessage(
                    message.messageId,
                    message.subject,
                    "18 February 2018",
                    message.recipients.first().name!!,
                    message.hasUnreadReplies)
        })
    }

    private fun setUpMessageDataAndStubs(patient: Patient, messages: MessagesResponseModel) {
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.EXPECTED_MESSAGES, getExpectedMessages(messages.messages))

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.AVAILABLE_MESSAGE, MessagingData.getMessagesWithReplies())

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS, MessagingData.getDefaultMessageRecipients())


        mockingClient.forEmis {
            messaging.viewMyMessagesRequest(patient)
                    .respondWithSuccess(messages)
        }
        mockingClient.forEmis {
            messaging.viewConversationRequest(patient)
                    .respondWithSuccess(MessagingData.getMessagesWithReplies())
        }

        mockingClient.forEmis {
            messaging.updateReadStatusRequest(patient)
                    .respondWithSuccess(MessagingData.getUpdatedResponse())
        }
        mockingClient.forEmis{
            messaging.getRecipientsRequest(patient)
                    .respondWithSuccess(MessagingData.getDefaultMessageRecipients())
        }
    }

    companion object {
        const val REPLY_COUNT = 3;
    }
}