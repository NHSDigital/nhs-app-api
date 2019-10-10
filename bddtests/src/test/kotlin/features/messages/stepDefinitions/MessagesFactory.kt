package features.messages.stepDefinitions

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import org.junit.Assert
import utils.SerenityHelpers
import utils.set
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val SEVEN_DAYS: Long = 7
private const val TWO_MONTHS: Long = 2
private const val THREE_DAYS: Long = 3
class MessagesFactory {

    val mockingClient = MockingClient.instance

    private val frontendSummaryDateFormatter = DateTimeFormatter.ofPattern("dd/MM/yyyy")
    private val oneWeekAgo = ZonedDateTime.now().minusDays(SEVEN_DAYS)
    private val twoMonthsAgo = ZonedDateTime.now().minusMonths(TWO_MONTHS)
    private val threeDaysAgo = ZonedDateTime.now().minusDays(THREE_DAYS)

    private val senderOne = "Sender One"
    private val senderTwo = "Sender Two"

    fun setUpUser(gpSystem: String, patient: Patient? = null) {
        SerenityHelpers.setGpSupplier(gpSystem)
        val patientToUse = patient ?: Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patientToUse)
        MongoDBConnection.MessagesCollection.clearCache()
        MessagesSerenityHelpers.TARGET_SENDER.set(senderOne)
    }

    fun setUpMultipleMessagesInCache() {
        MongoDBConnection.MessagesCollection.clearCache()
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = createUnreadMessage(senderOne, "Message One", twoMonthsAgo)
        val senderOneMessageTwo = createUnreadMessage(senderOne, "Message Two", oneWeekAgo)
        val senderOneMessages = createSenderMessages(arrayListOf(senderOneMessageOne, senderOneMessageTwo))
        val senderOneSummaryMessage = MessagesSummaryFacade(senderOne, 2, arrayListOf(senderOneMessageTwo),
                lastMessageTime = frontendSummaryDateFormatter.format(oneWeekAgo))

        val senderTwoMessageOne = createUnreadMessage(senderTwo, "Message Three", twoMonthsAgo)
        val senderTwoMessages = createSenderMessages(arrayListOf(senderTwoMessageOne))
        val senderTwoSummaryMessage = MessagesSummaryFacade(senderTwo, 1, arrayListOf(senderTwoMessageOne),
                lastMessageTime = frontendSummaryDateFormatter.format(twoMonthsAgo))

        createMessagesInRepository(arrayListOf(senderOneMessages, senderTwoMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.set(
                arrayListOf(senderOneSummaryMessage, senderTwoSummaryMessage)
        )

        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderOneMessages)
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES.set(arrayListOf(senderOneMessageOne, senderOneMessageTwo))
        MessagesSerenityHelpers.EXPECTED_READ_MESSAGES.set(arrayListOf<MessagesSummaryFacade>())
    }

    private fun createUnreadMessage(sender: String, body: String, sentTime: ZonedDateTime): SingleMessageFacade {
        return SingleMessageFacade(sender,
                body,
                false,
                MongoDBConnection.mongoDateFormatter.format(sentTime)
        )
    }

    private fun createMessagesInRepository(messageFacadesToInsert: ArrayList<MessagesSummaryFacade>,
                                           nhsLoginId: String) {
        val messagesToInsert = messageFacadesToInsert
                .flatMap { senderMessage -> senderMessage.messages.map { message -> asJson(message, nhsLoginId) } }
        MongoDBConnection.MessagesCollection.clearAndInsertJson(messagesToInsert)
    }

    private fun createSenderMessages(messages: ArrayList<SingleMessageFacade>): MessagesSummaryFacade {
        val distinctSender = messages.map { message -> message.sender }.distinct()
        Assert.assertEquals("Expected one distinct sender", 1, distinctSender.count())
        val unreadCount = messages.count { message -> !message.read }
        return MessagesSummaryFacade(distinctSender.single(), unreadCount, messages)
    }

    // We cannot serialise an object to create this because the ISODate objects cannot be created like that.
    private fun asJson(message: SingleMessageFacade, nhsLoginId: String): String {
        val readString = if (!message.read) null else
            "ISODate(\"${MongoDBConnection.mongoDateFormatter.format(threeDaysAgo)}\")"
        return "{" +
                "\"_ts\" : ISODate(\"${message.sentTime}\")," +
                "\"NhsLoginId\" : \"${nhsLoginId}\"," +
                "\"Sender\" : \"${message.sender}\"," +
                "\"Version\" : 1," +
                "\"Body\" : \"${message.body}\"," +
                "\"Read\" : $readString" +
                "\"SentTime\" : ISODate(\"${message.sentTime}\")" +
                "},"
    }
}