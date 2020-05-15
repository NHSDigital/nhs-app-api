package features.patientPracticeMessaging.factories

import mocking.data.patientPracticeMessaging.EmisMessagingData
import mocking.emis.practices.SettingsResponseModel
import mocking.patientPracticeMessaging.MessagesResponseModel
import mocking.patientPracticeMessaging.PatientMessageSummary
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import models.ExpectedMessage
import models.Patient
import utils.getOrNull
import utils.setIfNotAlreadySet
import worker.models.patientPracticeMessaging.CreateMessageRequest

class PracticePatientMessagingFactoryEmis: PracticePatientMessagingFactory() {

    override fun enabled(patient: Patient){
        val response = SettingsResponseModel()
        response.services.practicePatientCommunicationSupported = true

        mockingClient.forEmis.mock {
            practiceSettingsRequest(patient)
                    .respondWithSuccess(response)
        }
    }

    override fun disabled(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.viewMyMessagesRequest(patient)
                    .respondWithExceptionWhenNotEnabled()
        }
    }

    override fun patientHasNoMessages(patient: Patient){
        mockingClient.forEmis.mock{
            messaging.viewMyMessagesRequest(patient)
                    .respondWithSuccess(MessagesResponseModel(mutableListOf()))
        }
    }

    override fun unknownErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.viewMyMessagesRequest(patient).respondWithBadRequest()
        }
    }

    override fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.viewMyMessagesRequest(patient).respondWithForbidden()
        }
    }

    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.viewConversationRequest(patient).respondWithBadRequest()
        }
    }

    override fun noRecipients(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.getRecipientsRequest(patient)
                    .respondWithSuccess(EmisMessagingData.getEmptyRecipients())
        }
    }

    override fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient) {
        mockingClient.forEmis.mock {
            messaging.deleteConversationRequest(patient).respondWithBadRequest()
        }
    }
    
    override fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean,
                                                     hasAttachment: Boolean, unitRecipient: Boolean){
        setUpMessageDataAndStubs(patient, hasUnread)
    }

    override fun enabledWithPatientPracticeMessagingFromGP(patient: Patient, hasUnread: Boolean) {
        throw NotImplementedError("Not implemented")
    }

    override fun patientSuccessfullySendsAMessage(patient: Patient, createMessageRequest: CreateMessageRequest) {
        mockingClient.forEmis.mock {
            messaging.sendMessageRequest(patient, createMessageRequest).respondWithSuccess()
        }
    }

    override fun errorSendingAMessage(patient: Patient, createMessageRequest: CreateMessageRequest) {
        mockingClient.forEmis.mock {
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
                "When will my test results be ready", recipients.MessageRecipients[0].recipientGuid!!)

        PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES
            .setIfNotAlreadySet(getExpectedMessages(messages.messages))
        PatientPracticeMessagingSerenityHelpers.AVAILABLE_MESSAGE
            .setIfNotAlreadySet(messageDetails)
        PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE
            .setIfNotAlreadySet(createMessageRequest)


        mockingClient.forEmis.mock {
            messaging.viewMyMessagesRequest(patient)
                    .respondWithSuccess(messages)
        }
        mockingClient.forEmis.mock {
            messaging.viewConversationRequest(patient)
                    .respondWithSuccess(messageDetails)
        }
        mockingClient.forEmis.mock {
            messaging.updateReadStatusRequest(patient)
                    .respondWithSuccess(EmisMessagingData.getUpdatedResponse())
        }
        mockingClient.forEmis.mock{
            messaging.getRecipientsRequest(patient)
                    .respondWithSuccess(recipients)
        }
        mockingClient.forEmis.mock {
            messaging.sendMessageRequest(patient, createMessageRequest)
                    .respondWithSuccess()
        }
        mockingClient.forEmis.mock {
            messaging.deleteConversationRequest(patient)
                    .respondWithSuccess(EmisMessagingData.getDeleteResponse())
        }
    }

    companion object {
        const val REPLY_COUNT = 3
    }

    override fun enabledWithInvalidAttachmentOnMessage(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
}