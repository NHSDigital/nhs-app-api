package features.patientPracticeMessaging.factories

import mocking.data.messaging.MessagingData
import mocking.emis.models.PatientPracticeMessagingTypes
import mocking.emis.patientPracticeMessaging.MessagesResponseModel
import mocking.emis.patientPracticeMessaging.PatientMessageSummary
import models.ExpectedMessage
import models.Patient
import utils.SerenityHelpers
import worker.models.patientPracticeMessaging.CreateMessageRequest

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

    override fun unknownErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forEmis {
            messaging.viewMyMessagesRequest(patient).respondWithBadRequest()
        }
    }

    override fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forEmis {
            messaging.viewMyMessagesRequest(patient).respondWithForbidden()
        }
    }

    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        mockingClient.forEmis {
            messaging.viewConversationRequest(patient).respondWithBadRequest()
        }
    }

    override fun noRecipients(patient: Patient) {
        mockingClient.forEmis {
            messaging.getRecipientsRequest(patient)
                    .respondWithSuccess(MessagingData.getEmptyRecipients())
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean){
        val messages = MessagingData.getDefaultMessagesData(REPLY_COUNT, hasUnread)
        setUpMessageDataAndStubs(patient, messages)
    }

    override fun patientSuccessfullySendsAMessage(patient: Patient, createMessageRequest: CreateMessageRequest) {
        mockingClient.forEmis {
            messaging.sendMessageRequest(patient, createMessageRequest).respondWithSuccess()
        }
    }

    override fun errorSendingAMessage(patient: Patient, createMessageRequest: CreateMessageRequest) {
        mockingClient.forEmis {
            messaging.sendMessageRequest(patient, createMessageRequest).respondWithBadRequest()
        }
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
        val recipients = MessagingData.getDefaultMessageRecipients()
        val createMessageRequest = CreateMessageRequest("Test Results",
                "When will my test results be ready", recipients.MessageRecipients[0].recipientGuid!!)

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.EXPECTED_MESSAGES, getExpectedMessages(messages.messages))

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.AVAILABLE_MESSAGE, MessagingData.getMessagesWithReplies())

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.AVAILABLE_RECIPIENTS, MessagingData.getDefaultMessageRecipients())

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(PatientPracticeMessagingTypes.SENT_MESSAGE,
                createMessageRequest)


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
                    .respondWithSuccess(recipients)
        }
        mockingClient.forEmis {
            messaging.sendMessageRequest(patient, createMessageRequest)
                    .respondWithSuccess()
        }
    }

    companion object {
        const val REPLY_COUNT = 3;
    }
}