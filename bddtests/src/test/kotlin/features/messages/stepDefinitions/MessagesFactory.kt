package features.messages.stepDefinitions

import com.mongodb.client.model.Filters.eq
import config.Config
import io.cucumber.datatable.DataTable
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryMessage
import org.bson.Document
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import utils.toSingleElementList
import worker.JsonPatch
import worker.JsonPatchOperation
import worker.models.messages.MessageRequest
import worker.models.messages.SenderFacade
import worker.models.messages.SingleMessageFacade
import java.time.ZoneId
import java.time.ZonedDateTime
import java.util.UUID

private const val SEVEN_DAYS: Long = 7
private const val TWO_MONTHS: Long = 2
private const val ONE_MONTH: Long = 1
private const val INVALID_MESSAGE_BODY: Int = 1234

class MessagesFactory {

    private val timeNow = ZonedDateTime.now(ZoneId.of("Europe/London"))

    private val twoMonthsAgo = timeNow.minusMonths(TWO_MONTHS)
    private val oneMonthAgo = timeNow.minusMonths(ONE_MONTH)
    private val oneWeekAgo = timeNow.minusDays(SEVEN_DAYS)

    private val senderOne = "Sender One"
    private val senderTwo = "Sender Two"
    private val messageOne = "Message One";
    private val messageTwo = "Message Two";

    fun setUpUser(patient: Patient? = null) {
        val patientToUse = patient
            ?: ServiceJourneyRulesMapper.findPatientForConfiguration(
                null,
                SJRJourneyType.MESSAGES_ENABLED
            )
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney().createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(SerenityHelpers.getGpSupplier())
            .createFor(patientToUse)
        MessagesSerenityHelpers.TARGET_SENDER.set(senderOne)
        MessagesSerenityHelpers.TARGET_MESSAGE.set(messageOne)
        MessagesSerenityHelpers.TARGET_UNREAD_MESSAGE.set(messageTwo)
    }

    fun setUpMultipleMessagesInCache() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = createReadMessage(
            "1.1", senderOne, messageOne, twoMonthsAgo, MessageVersion.PLAIN_TEXT
        )
        val senderOneMessageTwo = createUnreadMessage(
            "1.2", senderOne, messageTwo, oneMonthAgo, MessageVersion.PLAIN_TEXT
        )
        val senderOneMessageThree = createReadMessage(
            "1.3", senderOne, "Message Three", oneWeekAgo, MessageVersion.PLAIN_TEXT
        )
        val senderOneMessages = createSenderMessages(
            arrayListOf(senderOneMessageOne, senderOneMessageTwo, senderOneMessageThree)
        )
        val senderOneSummaryMessage = SenderFacade(senderOne, 1, arrayListOf(senderOneMessageThree))

        val senderTwoMessageOne = createUnreadMessage(
            "2.1", senderTwo, "Message Three", twoMonthsAgo, MessageVersion.PLAIN_TEXT
        )
        val senderTwoMessages = createSenderMessages(arrayListOf(senderTwoMessageOne))
        val senderTwoSummaryMessage = SenderFacade(senderTwo, 1, arrayListOf(senderTwoMessageOne))

        createMessagesInRepository(arrayListOf(senderOneMessages, senderTwoMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(
            arrayListOf(senderOneSummaryMessage, senderTwoSummaryMessage)
        )

        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderOneMessages)
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES.set(arrayListOf(senderOneMessageTwo))
        MessagesSerenityHelpers.EXPECTED_READ_MESSAGES.set(arrayListOf(senderOneMessageThree, senderOneMessageOne))
    }

    fun setUpMultipleMessagesWithContentInCache(table: DataTable, messageVersion: MessageVersion) {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val links = table.toSingleElementList()
        val messages = links.mapIndexed { index, link ->
            createReadMessage(
                UUID.randomUUID().toString(),
                senderOne,
                "message ${index + 1} ${prefixInternalLink(link)}",
                twoMonthsAgo.minusDays(index.toLong()),
                messageVersion
            )
        }

        val senderMessages = createSenderMessages(messages as ArrayList<SingleMessageFacade>)
        val senderMessage = SenderFacade(senderOne, 0, arrayListOf(messages.first()))

        createMessagesInRepository(arrayListOf(senderMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(arrayListOf(senderMessage))
        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderMessages)
    }

    fun setUpSingleUnreadMessage() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val messageToPost = MessageRequest(senderOne, messageOne, MessageVersion.PLAIN_TEXT.value)
        val response = MessagesApi.postSetup(messageToPost, nhsLoginId)
        MessagesSerenityHelpers.MESSAGE_ID.set(response?.messageId)
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(messageToPost)
    }

    fun setUpInvalidMessageInCache() {
        val targetMessage = MessagesSerenityHelpers.TARGET_MESSAGE.getOrFail<String>()
        MongoDBConnection.MessagesCollection.updateOne(
            eq("Body", targetMessage),
            Document("\$set", Document("Body", INVALID_MESSAGE_BODY))
        )
    }

    fun setUpValidMessageInCache() {
        val targetMessage = MessagesSerenityHelpers.TARGET_MESSAGE.getOrFail<String>()
        MongoDBConnection.MessagesCollection.updateOne(
            eq("Body", INVALID_MESSAGE_BODY),
            Document("\$set", Document("Body", targetMessage))
        )
    }

    private fun createReadMessage(
        id: String, sender: String, body: String, sentTime: ZonedDateTime, messageVersion: MessageVersion
    ): SingleMessageFacade {
        return SingleMessageFacade(
            id,
            sender,
            body,
            true,
            MongoDBConnection.mongoDateFormatter.format(sentTime),
            messageVersion.value
        )
    }

    private fun createUnreadMessage(
        id: String, sender: String, body: String, sentTime: ZonedDateTime, messageVersion: MessageVersion
    ): SingleMessageFacade {
        return SingleMessageFacade(
            id,
            sender,
            body,
            false,
            MongoDBConnection.mongoDateFormatter.format(sentTime),
            messageVersion.value
        )
    }

    private fun createMessagesInRepository(
        messageFacadesToInsert: ArrayList<SenderFacade>,
        nhsLoginId: String
    ) {
        val messagesToInsert = messageFacadesToInsert
            .flatMap { senderMessage ->
                senderMessage.messages.map { message -> MongoRepositoryMessage.createJson(message, nhsLoginId) }
            }
        MongoDBConnection.MessagesCollection.clearAndInsertJson(messagesToInsert)
    }

    private fun createSenderMessages(messages: ArrayList<SingleMessageFacade>): SenderFacade {
        val distinctSender = messages.map { message -> message.sender }.distinct()
        Assert.assertEquals("Expected one distinct sender", 1, distinctSender.count())
        val unreadCount = messages.count { message -> !message.read }
        return SenderFacade(distinctSender.single(), unreadCount, messages)
    }

    companion object {
        val patchToUpdateAsRead = JsonPatch(JsonPatchOperation.ADD, "/read", true)
    }

    private fun prefixInternalLink(link: String): String {
        if (link.startsWith("/")) {
            return "${Config.instance.url}$link"
        }
        return link
    }
}
