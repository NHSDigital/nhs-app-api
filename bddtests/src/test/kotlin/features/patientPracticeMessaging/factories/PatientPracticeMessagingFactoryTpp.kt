package features.patientPracticeMessaging.factories

import constants.ErrorResponseCodeTpp
import constants.TppConstants
import mocking.data.TppListServiceAccessesData
import mocking.data.myrecord.TppDocumentData
import mocking.data.patientPracticeMessaging.DateHelpers
import mocking.data.patientPracticeMessaging.MessageDateFormat
import mocking.data.patientPracticeMessaging.TppMessagingData
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import mocking.patientPracticeMessaging.DateAndFormat
import mocking.patientPracticeMessaging.Recipient
import mocking.tpp.models.Error
import mocking.tpp.models.Message
import mocking.tpp.models.MessageCreateReply
import mocking.tpp.models.MessageMarkAsReadReply
import mocking.tpp.models.MessagesViewReply
import models.ExpectedMessage
import models.Patient
import utils.SerenityHelpers
import utils.getOrNull
import utils.setIfNotAlreadySet
import worker.models.patientPracticeMessaging.CreateMessageRequest

const val UNREAD_COUNT = 4
class PatientPracticeMessagingFactoryTpp: PracticePatientMessagingFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forTpp.mock {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabled(patient: Patient){
        mockingClient.forTpp.mock {
            val listServicesAccessReply = TppListServiceAccessesData()
                    .enableService(TppConstants.Im1MessagingListServiceAccessDescription,
                                   TppConstants.Im1MessagingListServiceAccessCode,
                                   TppConstants.ListServiceAccessesAvailableStatus,
                                   TppConstants.ListServiceAccessesAvailableStatusDescription)
            listServiceAccesses(SerenityHelpers.getPatient())
                    .respondWithSuccess(listServicesAccessReply)
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient, hasUnread: Boolean,
                                                     hasAttachment: Boolean, unitRecipient: Boolean) {
        val messagesFromData = TppMessagingData.getDefaultTppMessages(hasUnread, hasAttachment)

        mockingClient.forTpp.mock {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(messagesFromData)
        }

        mockingClient.forTpp.mock {
            patientPracticeMessaging.requestRecipientsRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppMessagingData.getDefaultTppRecipients())
        }

        mockingClient.forTpp.mock {
            patientPracticeMessaging.markMessagesAsReadRequest(patient.tppUserSession!!)
                .respondWithSuccess(MessageMarkAsReadReply())
        }

        if (hasAttachment) {
            mockingClient.forTpp.mock {
                patientPracticeMessaging
                        .attachmentRequest(patient.tppUserSession!!)
                        .respondWithSuccess(TppDocumentData.getDocumentData("jpg"))
            }
        }

        val expectedMessages = getExpectedMessages(messagesFromData.Message, hasUnread)

        val firstMessage = messagesFromData.Message.first()
        val replyList = messagesFromData.Message.filter{
            m -> m.conversationId == firstMessage.conversationId
        }

        val messageDetails = TppMessagingData.getDefaultSelected(
                firstMessage,
                replyList
        )

        mockMessageCreate(unitRecipient)

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES,
                expectedMessages)

        PatientPracticeMessagingSerenityHelpers.AVAILABLE_MESSAGE
                .setIfNotAlreadySet(messageDetails)
    }

    override fun enabledWithInvalidAttachmentOnMessage(patient: Patient) {
        mockingClient.forTpp.mock {
            patientPracticeMessaging
                    .attachmentRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.FILE_SIZE_TOO_LARGE,
                                            "File exceeds 2MB"))
        }
    }

    override fun enabledWithPatientPracticeMessagingFromGP(patient: Patient, hasUnread: Boolean) {

        val messagesFromData = TppMessagingData.getTppMessagesInitialFromGp()

        mockingClient.forTpp.mock {
            patientPracticeMessaging
                    .viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(messagesFromData)
        }

        mockingClient.forTpp.mock {
            patientPracticeMessaging.markMessagesAsReadRequest(patient.tppUserSession!!)
                .respondWithSuccess(MessageMarkAsReadReply())
        }

        val expectedMessages = getExpectedMessages(messagesFromData.Message,
                hasUnread)

        val firstMessage = messagesFromData.Message.first()
        val replyList = messagesFromData.Message.filter{
            m -> m.conversationId != firstMessage.conversationId
        }

        val messageDetails = TppMessagingData.getDefaultSelected(
                firstMessage,
                replyList
        )

        SerenityHelpers.setSerenityVariableIfNotAlreadySet(
                PatientPracticeMessagingSerenityHelpers.EXPECTED_MESSAGES,
                expectedMessages)

        PatientPracticeMessagingSerenityHelpers.AVAILABLE_MESSAGE
                .setIfNotAlreadySet(messageDetails)
    }

    override fun unknownErrorWithPatientPracticeMessaging(patient: Patient) {
        TODO("not implemented")
    }

    override fun patientHasNoMessages(patient: Patient) {
        mockingClient.forTpp.mock{
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                .respondWithSuccess(
                    MessagesViewReply( Message = mutableListOf())
                )
        }
    }

    private fun getExpectedMessages(
        messages: Iterable<Message>,
        hasUnread: Boolean = false): List<ExpectedMessage> {
        val unreadCount = if (hasUnread) UNREAD_COUNT else 0

        val latestMessageDateTime = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_REPLY_MESSAGE_DATES
                .getOrNull<List<DateAndFormat>>()!!
                .sortedByDescending { it.date }
                .first()

        return messages.map { message ->
            ExpectedMessage(
                message.messageId,
                message.conversationId,
                messageText = message.messageText,
                lastMessageDateTime = DateHelpers().getExpectedFormattedMessageDate(
                    latestMessageDateTime.date, getInboxDateFormatFromReply(
                    latestMessageDateTime.format)),
                sender = message.sender,
                recipient = message.recipient,
                hasUnreadReplies = hasUnread,
                unreadCount = unreadCount)
        }
    }

    override fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forTpp.mock {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }


    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        TODO("not implemented")
    }

    override fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient) {
        TODO("not implemented")
    }

    override fun patientSuccessfullySendsAMessage(patient: Patient,
                                                  createMessageRequest: CreateMessageRequest) {
        TODO("not implemented")
    }

    override fun errorSendingAMessage(patient: Patient,
                                      createMessageRequest: CreateMessageRequest) {
        TODO("not implemented")
    }

    override fun noRecipients(patient: Patient) {
        mockingClient.forTpp.mock {
            patientPracticeMessaging.requestRecipientsRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppMessagingData.getTppEmptyRecipients())
        }
    }

    private fun getInboxDateFormatFromReply(format: MessageDateFormat) : MessageDateFormat {
        return when(format) {
            MessageDateFormat.DETAILS_TODAY -> {
                MessageDateFormat.INBOX_TIME_12_HR
            }
            MessageDateFormat.DETAILS_TODAY_AT_MIDDAY -> {
                MessageDateFormat.INBOX_MIDDAY
            }
            MessageDateFormat.DETAILS_TODAY_AT_MIDNIGHT-> {
                MessageDateFormat.INBOX_MIDNIGHT
            }
            MessageDateFormat.DETAILS_YESTERDAY -> {
                MessageDateFormat.INBOX_YESTERDAY
            }
            else -> {
                format
            }
        }
    }

    private fun mockMessageCreate(unitRecipient: Boolean) {
        val recipients = PatientPracticeMessagingSerenityHelpers
                .AVAILABLE_RECIPIENTS
                .getOrNull<List<Recipient>>()!!
        val recipient = if (unitRecipient) recipients[1] else recipients[0]
        val createMessageRequest = CreateMessageRequest(messageBody = "This is a message",
                                                        recipient = recipient.recipientIdentifier!!)
        mockingClient.forTpp.mock {
            patientPracticeMessaging.createMessageRequest(SerenityHelpers.getPatient().tppUserSession!!,
                                                          createMessageRequest)
                    .respondWithSuccess(MessageCreateReply())
        }

        PatientPracticeMessagingSerenityHelpers.SENT_MESSAGE
                .setIfNotAlreadySet(createMessageRequest)
    }
}
