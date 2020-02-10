package features.patientPracticeMessaging.factories

import mocking.data.patientPracticeMessaging.MessagingData
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

    override fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient) {
        mockingClient.forEmis {
            messaging.deleteConversationRequest(patient).respondWithBadRequest()
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean){
        setUpMessageDataAndStubs(patient, hasUnread)
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
        val expectedInboxMessageDates = SerenityHelpers
                .getValueOrNull<List<String>>(PatientPracticeMessagingTypes.EXPECTED_INBOX_MESSAGE_DATES)
        return expectedMessages.mapIndexed(fun(index, message): ExpectedMessage {
            return ExpectedMessage(
                    message.messageId,
                    message.subject,
                    expectedInboxMessageDates!![index],
                    message.recipients.first().name!!,
                    message.hasUnreadReplies)
        })
    }

    private fun setUpMessageDataAndStubs(patient: Patient, hasUnread: Boolean) {
        val messages = MessagingData.getDefaultMessagesData(REPLY_COUNT, hasUnread)
        val recipients = MessagingData.getDefaultMessageRecipients()
        val messageDetails = MessagingData.getDefaultMessageDetailsWithReplies()
        val createMessageRequest = CreateMessageRequest("Test Results",
                "When will my test results be ready", recipients.MessageRecipients[0].recipientGuid!!)

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.EXPECTED_MESSAGES, getExpectedMessages(messages.messages))

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingTypes.AVAILABLE_MESSAGE, messageDetails)

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
                    .respondWithSuccess(messageDetails)
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

        mockingClient.forEmis {
            messaging.deleteConversationRequest(patient)
                    .respondWithSuccess(MessagingData.getDeleteResponse())
        }
    }

    companion object {
        const val REPLY_COUNT = 3
    }
}