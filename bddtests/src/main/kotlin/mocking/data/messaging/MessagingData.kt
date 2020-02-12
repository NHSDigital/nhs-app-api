package mocking.data.messaging

import mocking.emis.patientPracticeMessaging.MessageDetails
import mocking.emis.patientPracticeMessaging.MessageRecipientsResponseModel
import mocking.emis.patientPracticeMessaging.MessageReply
import mocking.emis.patientPracticeMessaging.MessageResponseModel
import mocking.emis.patientPracticeMessaging.MessagesResponseModel
import mocking.emis.patientPracticeMessaging.PatientMessageSummary
import mocking.emis.patientPracticeMessaging.MessageReadStatusUpdateResponse
import mocking.emis.patientPracticeMessaging.Recipient

object MessagingData {

    private const val DEFAULT_NUMBER_OF_MESSAGES = 3
    private val RECIPIENT_1 = Recipient("Dr. Dolittle", "1234-12345678-1234-1234-1")
    private val RECIPIENT_2 = Recipient("Dr. NHS Online", "1234-12345678-1234-1234-2")

    fun getDefaultMessagesData(replyCount: Int = 0, hasUnreadReplies: Boolean = false): MessagesResponseModel {
        val messages = mutableListOf<PatientMessageSummary>()

        for (messageNumber in 1..DEFAULT_NUMBER_OF_MESSAGES){
            messages.add(PatientMessageSummary(
                    messageNumber,
                    "GP Practice information $messageNumber",
                    "2018-02-18T14:23:44.927Z",
                    mutableListOf(RECIPIENT_1),
                    replyCount,
                    hasUnreadReplies
            ))
        }

        return MessagesResponseModel(messages)
    }

    fun getMessagesWithReplies(isUnread: Boolean = false, isLegacy: Boolean = false): MessageResponseModel {
        val messageReplies = mutableListOf<MessageReply>()

        messageReplies.add(MessageReply(
                isLegacy,
                isUnread,
                "Yes, the results are ready.",
                RECIPIENT_1.name!!,
                "2019-12-05T13:39:02.303"
        ))

        messageReplies.add(MessageReply(
                isLegacy,
                isUnread,
                "I need you to book an appointment to see me.",
                RECIPIENT_1.name!!,
                "2019-12-05T13:39:02.303"
        ))

        messageReplies.add(MessageReply(
                isLegacy,
                isUnread,
                "I can see the appointment is booked, thank you..",
                RECIPIENT_1.name!!,
                "2019-12-05T13:39:02.303"
        ))

        val message = MessageDetails("1", "GP Practice Information",
                mutableListOf(RECIPIENT_1),
                messageReplies, "When will my blood test results be ready?",
                "2019-12-05T13:39:02.303", "NHS App Messaging")

        return MessageResponseModel(message)
    }

    fun getDefaultMessageRecipients(): MessageRecipientsResponseModel {
        return MessageRecipientsResponseModel(
            arrayListOf(RECIPIENT_1, RECIPIENT_2)
        )
    }

    fun getEmptyRecipients() : MessageRecipientsResponseModel {
        return MessageRecipientsResponseModel()
    }

    fun getUpdatedResponse(): MessageReadStatusUpdateResponse {
        return MessageReadStatusUpdateResponse("Updated")
    }
}