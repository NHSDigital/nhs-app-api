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
import worker.models.messages.SenderContext
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

    private val senderOneName = "Sender One"
    private val senderOneId = "12345";
    private val senderOneSenderContext = SenderContext(senderId = senderOneId)

    private val senderTwoName = "Sender Two"

    private val messageOne = "Message One"
    private val messageTwo = "Message Two"
    private val messageThree = "Message Three"

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
        MessagesSerenityHelpers.TARGET_SENDER_NAME.set(senderOneName)
        MessagesSerenityHelpers.TARGET_SENDER_ID.set(senderOneId)
        MessagesSerenityHelpers.TARGET_MESSAGE.set(messageOne)
        MessagesSerenityHelpers.TARGET_UNREAD_MESSAGE.set(messageTwo)
    }

    fun setUpMultipleMessagesInCache() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = SingleMessageFacade(
            "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessageTwo = SingleMessageFacade(
            "1.2", senderOneName, messageTwo, false, MongoDBConnection.mongoDateFormatter.format(oneMonthAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessageThree = SingleMessageFacade(
            "1.3", senderOneName, messageThree, true, MongoDBConnection.mongoDateFormatter.format(oneWeekAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessages = createSenderMessages(
            arrayListOf(senderOneMessageOne, senderOneMessageTwo, senderOneMessageThree)
        )
        val senderOneSummaryMessage = SenderFacade(senderOneName, 1, arrayListOf(senderOneMessageThree))

        val senderTwoMessageOne = SingleMessageFacade(
            "2.1", senderTwoName, messageOne, false, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
            MessageVersion.PLAIN_TEXT.value
        )
        val senderTwoMessages = createSenderMessages(arrayListOf(senderTwoMessageOne))
        val senderTwoSummaryMessage = SenderFacade(senderTwoName, 1, arrayListOf(senderTwoMessageOne))

        createMessagesInRepository(arrayListOf(senderOneMessages, senderTwoMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(
            arrayListOf(senderOneSummaryMessage, senderTwoSummaryMessage)
        )

        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderOneMessages)
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES.set(arrayListOf(senderOneMessageTwo))
        MessagesSerenityHelpers.EXPECTED_READ_MESSAGES.set(arrayListOf(senderOneMessageThree, senderOneMessageOne))
    }

    fun setUpMultipleMessagesInCacheV2() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = SingleMessageFacade(
            "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessageTwo = SingleMessageFacade(
            "1.2", senderOneName, messageTwo, false, MongoDBConnection.mongoDateFormatter.format(oneMonthAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessageThree = SingleMessageFacade(
            "1.3", senderOneName, messageThree, true, MongoDBConnection.mongoDateFormatter.format(oneWeekAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext
        )
        val senderOneMessages = createSenderMessagesV2(
            arrayListOf(senderOneMessageOne, senderOneMessageTwo, senderOneMessageThree)
        )
        val senderOneSummaryMessage = SenderFacade(senderOneName, 1, arrayListOf(senderOneMessageThree))

        createMessagesInRepository(arrayListOf(senderOneMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(
            arrayListOf(senderOneSummaryMessage)
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
            SingleMessageFacade(
                UUID.randomUUID().toString(), senderOneName, "message ${index + 1} ${prefixInternalLink(link)}", true,
                MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo.minusDays(index.toLong())),
                messageVersion.value,
                senderOneSenderContext
            )
        }

        val senderMessages = createSenderMessages(messages as ArrayList<SingleMessageFacade>)
        val senderMessage = SenderFacade(senderOneName, 0, arrayListOf(messages.first()))

        createMessagesInRepository(arrayListOf(senderMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(arrayListOf(senderMessage))
        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderMessages)
    }

    fun setUpSingleUnreadMessage() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val messageToPost = MessageRequest(
            senderOneName, messageOne, MessageVersion.PLAIN_TEXT.value,
            SenderContext(senderId = senderOneId)
        )

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

    private fun createSenderMessagesV2(messages: ArrayList<SingleMessageFacade>): SenderFacade {
        val distinctSender = messages.map { message -> message.senderContext!!.senderId }.distinct()
        Assert.assertEquals("Expected one distinct sender", 1, distinctSender.count())
        val unreadCount = messages.count { message -> !message.read }
        return SenderFacade(distinctSender.single()!!, unreadCount, messages)
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
