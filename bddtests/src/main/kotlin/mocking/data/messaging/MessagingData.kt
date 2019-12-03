package mocking.data.messaging

import mocking.emis.patientPracticeMessaging.MessageResponseModel
import mocking.emis.patientPracticeMessaging.MessagesResponseModel
import mocking.emis.patientPracticeMessaging.PatientMessageSummary
import mocking.emis.patientPracticeMessaging.MessageDetails
import mocking.emis.patientPracticeMessaging.MessageReply
import mocking.emis.patientPracticeMessaging.Recipient

object MessagingData {

    private const val DEFAULT_NUMBER_OF_MESSAGES = 3
    private const val RECIPIENT = "Dr. Dolittle"

    fun getDefaultMessagesData(replyCount: Int = 0, hasUnreadReplies: Boolean = false): MessagesResponseModel {
        val messages = mutableListOf<PatientMessageSummary>()

        for (messageNumber in 1..DEFAULT_NUMBER_OF_MESSAGES){
            messages.add(PatientMessageSummary(
                    messageNumber,
                    "GP Practice information $messageNumber",
                    "2018-02-18T14:23:44.927Z",
                    mutableListOf(Recipient(RECIPIENT)),
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
                RECIPIENT,
                "2019-12-05T13:39:02.303"
        ))

        messageReplies.add(MessageReply(
                isLegacy,
                isUnread,
                "I need you to book an appointment to see me.",
                RECIPIENT,
                "2019-12-05T13:39:02.303"
        ))

        val message = MessageDetails("1", "GP Practice Information",
                mutableListOf(Recipient(RECIPIENT)),
                messageReplies, "When will my blood test results be ready?",
                "2019-12-05T13:39:02.303", "NHS App Messaging")

        return MessageResponseModel(message)
    }
}