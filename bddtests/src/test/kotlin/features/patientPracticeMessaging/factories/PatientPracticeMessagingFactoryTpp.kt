package features.patientPracticeMessaging.factories

import constants.ErrorResponseCodeTpp
import constants.TppConstants
import mocking.data.TppListServiceAccessesData
import mocking.data.patientPracticeMessaging.DateHelpers
import mocking.data.patientPracticeMessaging.MessageDateFormat
import mocking.data.patientPracticeMessaging.TppMessagingData
import mocking.patientPracticeMessaging.PatientPracticeMessagingSerenityHelpers
import mocking.tpp.models.Error
import mocking.tpp.models.Message
import mocking.tpp.models.MessagesViewReply
import mocking.patientPracticeMessaging.DateAndFormat
import models.ExpectedMessage
import models.Patient
import utils.SerenityHelpers
import utils.getOrNull
import utils.setIfNotAlreadySet
import worker.models.patientPracticeMessaging.CreateMessageRequest

const val UNREAD_COUNT = 4
class PatientPracticeMessagingFactoryTpp: PracticePatientMessagingFactory() {

    override fun disabled(patient: Patient) {
        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }

    override fun enabled(patient: Patient){
        mockingClient.forTpp {
            val listServicesAccessReply = TppListServiceAccessesData()
                    .enableService(TppConstants.Im1MessagingListServiceAccessDescription,
                                   TppConstants.Im1MessagingListServiceAccessCode,
                                   TppConstants.ListServiceAccessesAvailableStatus,
                                   TppConstants.ListServiceAccessesAvailableStatusDescription)
            listServiceAccesses(SerenityHelpers.getPatient())
                    .respondWithSuccess(listServicesAccessReply)
        }
    }

    override fun enabledWithPatientPracticeMessaging(patient: Patient,
                                                     hasUnread: Boolean) {

        val messagesFromData = TppMessagingData.getDefaultTppMessages(hasUnread)

        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(messagesFromData)
        }

        mockingClient.forTpp {
            patientPracticeMessaging.requestRecipientsRequest(patient.tppUserSession!!)
                    .respondWithSuccess(TppMessagingData.getDefaultTppRecipients())
        }

        val expectedMessages = getExpectedMessages(messagesFromData.Message.toList(),
                                                           hasUnread)

        val firstMessage = messagesFromData.Message.toList()[1]
        val replyList = mutableListOf<Message>()

        messagesFromData.Message.toList().forEachIndexed(fun (_, messageObject) {
            if (messageObject.conversationId == firstMessage.conversationId) {
                replyList.add(messageObject)
            }
        })
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

    override fun enabledWithPatientPracticeMessagingFromGP(patient: Patient, hasUnread: Boolean) {

        val messagesFromData = TppMessagingData.getTppMessagesInitialFromGp()

        mockingClient.forTpp {
            patientPracticeMessaging
                    .viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(messagesFromData)
        }
        val expectedMessages = getExpectedMessages(
                messagesFromData
                .Message.toList(),
                hasUnread)

        val firstMessage = messagesFromData
                .Message
                .toList()[0]
        val replyList = mutableListOf<Message>()

        messagesFromData
                .Message.toList()
                .forEachIndexed(fun (_, messageObject) {
            if (messageObject.conversationId != firstMessage.conversationId) {
                replyList.add(messageObject)
            }
        })
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
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun patientHasNoMessages(patient: Patient) {
        mockingClient.forTpp{
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithSuccess(
                            MessagesViewReply( Message = mutableListOf())
                    )
        }
    }

    private fun getExpectedMessages(messages: List<Message>, hasUnread: Boolean =
                                            false):
            List<ExpectedMessage> {
        var unreadCount = 0
        if (hasUnread) {
            unreadCount = UNREAD_COUNT
        }
        val expectedInboxMessageDates = PatientPracticeMessagingSerenityHelpers
                .EXPECTED_REPLY_MESSAGE_DATES
                .getOrNull<List<DateAndFormat>>()!!.sortedByDescending { it.date }
        return messages.mapIndexed(fun(_, message): ExpectedMessage {
            return ExpectedMessage(
                    message.messageId,
                    message.conversationId,
                    messageText = message.messageText,
                    lastMessageDateTime = DateHelpers().getExpectedFormattedMessageDate(
                            expectedInboxMessageDates[0].date, getInboxDateFormatFromReply(
                            expectedInboxMessageDates[0].format)),
                    sender = message.sender,
                    recipient = message.recipient,
                    hasUnreadReplies = hasUnread,
                    unreadCount = unreadCount)
        })
    }

    override fun forbiddenErrorWithPatientPracticeMessaging(patient: Patient) {
        mockingClient.forTpp {
            patientPracticeMessaging.viewMessagesRequest(patient.tppUserSession!!)
                    .respondWithError(Error(ErrorResponseCodeTpp.NO_ACCESS,
                                            "Im1 messaging access is disabled by the practice",
                                            "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
        }
    }


    override fun errorWithPatientPracticeMessagingMessageDetails(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun errorWithPatientPracticeMessagingConversationDelete(patient: Patient) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun patientSuccessfullySendsAMessage(patient: Patient,
                                                  createMessageRequest: CreateMessageRequest) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun errorSendingAMessage(patient: Patient,
                                      createMessageRequest: CreateMessageRequest) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun noRecipients(patient: Patient) {
        mockingClient.forTpp {
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

}