package features.patientPracticeMessaging.factories

import mocking.data.patientPracticeMessaging.EmisMessagingData
import mocking.sharedModels.PatientPracticeMessagingSerenityHelpers
import mocking.emis.practices.SettingsResponseModel
import mocking.sharedModels.MessagesResponseModel
import mocking.sharedModels.PatientMessageSummary
import models.ExpectedMessage
import models.Patient
import utils.getOrNull
import utils.setIfNotAlreadySet
import worker.models.patientPracticeMessaging.CreateMessageRequest

class PracticePatientMessagingFactoryEmis: PracticePatientMessagingFactory() {

    override fun enabled(patient: Patient){
        val response = SettingsResponseModel()
        response.services.practicePatientCommunicationSupported = true

        mockingClient.forEmis {
            practiceSettingsRequest(patient)
                    .respondWithSuccess(response)
        }
    }

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
                    .respondWithSuccess(EmisMessagingData.getEmptyRecipients())
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

    private fun getExpectedMessages(expectedMessages: List<PatientMessageSummary>): List<ExpectedMessage>{
        val expectedInboxMessageDates = PatientPracticeMessagingSerenityHelpers
            .EXPECTED_INBOX_MESSAGE_DATES
            .getOrNull<List<String>>()
        return expectedMessages.mapIndexed(fun(index, message): ExpectedMessage {
            return ExpectedMessage(
                    id = message.messageId,
                    subject = message.subject,
                    lastMessageDateTime = expectedInboxMessageDates!![index],
                    recipient = message.recipients.first().name!!,
                    hasUnreadReplies = message.hasUnreadReplies)
        })
    }

    private fun setUpMessageDataAndStubs(patient: Patient, hasUnread: Boolean) {
        val messages = EmisMessagingData.getDefaultMessagesData(REPLY_COUNT, hasUnread)
        val recipients = EmisMessagingData.getDefaultMessageRecipients()
        val messageDetails = EmisMessagingData.getDefaultMessageDetailsWithReplies()
        val createMessageRequest = CreateMessageRequest("Test Results",
                "When will my test results be ready", recipients.MessageRecipients[0].recipientIdentifier!!)

        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES
            .setIfNotAlreadySet(getExpectedMessages(messages.messages))
        PatientPracticeMessagingSerenityHelpers.AVAILABLE_MESSAGE
            .setIfNotAlreadySet(messageDetails)
        PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE
            .setIfNotAlreadySet(createMessageRequest)


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
                    .respondWithSuccess(EmisMessagingData.getUpdatedResponse())
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
                    .respondWithSuccess(EmisMessagingData.getDeleteResponse())
        }
    }

    companion object {
        const val REPLY_COUNT = 3
    }
}