package features.messages.stepDefinitions

import com.mongodb.client.model.Filters.eq
import config.Config
import cosmosSql.CosmosSqlConnection
import cosmosSql.SqlRepositoryCommsSender
import cosmosSql.SqlRepositoryRecordCommsSender
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
import worker.models.messages.Reply
import worker.models.messages.ReplyOption
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
private const val MESSAGE_RESPONSE = "SMOKE"
private const val MESSAGE_SECOND_RESPONSE = "NEVER";

class MessagesFactory {

    private val timeNow = ZonedDateTime.now(ZoneId.of("Europe/London"))

    private val twoMonthsAgo = timeNow.minusMonths(TWO_MONTHS)
    private val oneMonthAgo = timeNow.minusMonths(ONE_MONTH)
    private val oneWeekAgo = timeNow.minusDays(SEVEN_DAYS)

    private val senderOneId = "SENDERONEID"
    private val senderOneSenderContext = SenderContext(senderId = senderOneId)
    private val senderOneName = "Sender One"
    private val senderOneCanonicalName = "Sender One Canonical"

    private val senderTwoName = "Sender Two"

    private val messageOne = "Message One"
    private val messageTwo = "Message Two"
    private val messageThree = "Message Three"

    private val nhsAppSenderId = "Y0E3J"
    private val nhsAppSenderCanonicalName = "NHS APP"

    private val messageReplyWithQuestionnaireAndResponse = Reply(options = arrayListOf(
        ReplyOption(code = MESSAGE_RESPONSE, display = "SMOKE"),
        ReplyOption(code = MESSAGE_SECOND_RESPONSE, display = "NEVER"),
        ReplyOption(code = "No", display = "NO")
        ),
        response = MESSAGE_RESPONSE)

    private val messageReplyWithQuestionnaire = Reply(options = arrayListOf(
            ReplyOption(code = MESSAGE_RESPONSE, display = "SMOKE"),
            ReplyOption(code = MESSAGE_SECOND_RESPONSE, display = "NEVER"),
            ReplyOption(code = "No", display = "NO")
    ))

    private val unreadMessagesTitleId = "unreadCountMessagesTitle"
    private val readMessagesTitleId = "readMessagesTitle"

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
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES_COUNT.set(0)
        MessagesSerenityHelpers.UNREAD_MESSAGES_TITLE_ID.set(unreadMessagesTitleId)
        MessagesSerenityHelpers.READ_MESSAGES_TITLE_ID.set(readMessagesTitleId)
    }

    fun setUpMultipleMessagesInCache() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = SingleMessageFacade(
            "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, null
        )
        val senderOneMessageTwo = SingleMessageFacade(
            "1.2", senderOneName, messageTwo, false, MongoDBConnection.mongoDateFormatter.format(oneMonthAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, null
        )
        val senderOneMessageThree = SingleMessageFacade(
            "1.3", senderOneName, messageThree, true, MongoDBConnection.mongoDateFormatter.format(oneWeekAgo),
            MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, null
        )
        val senderOneMessages = createSenderMessages(
            arrayListOf(senderOneMessageOne, senderOneMessageTwo, senderOneMessageThree)
        )

        val senderTwoMessageOne = SingleMessageFacade(
            "2.1", senderTwoName, messageOne, false, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
            MessageVersion.PLAIN_TEXT.value, null, null
        )
        val senderTwoMessages = createSenderMessages(arrayListOf(senderTwoMessageOne))

        val expectedSenderOneSummaryMessage =
            SenderFacade(senderOneName, "1",
                arrayListOf(senderOneMessageThree.copy(sender = senderOneCanonicalName)))
        val expectedSenderOneMessages = senderOneMessages.copy(
            name = senderOneCanonicalName,
            messages = senderOneMessages.messages
                .map { m -> m.copy(sender = senderOneCanonicalName) }
        )

        val expectedSenderTwoSenderContext =
            SenderContext(senderId = nhsAppSenderId, supplierId = "278d3b75-3498-4d68-8991-506d0006e46f")
        val expectedSenderTwoSummaryMessage =
            SenderFacade(senderTwoName, "1",
                arrayListOf(
                    senderTwoMessageOne.copy(
                        sender = nhsAppSenderCanonicalName,
                        senderContext = expectedSenderTwoSenderContext
                    )
                )
            )

        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES_COUNT.set(2)

        createSendersIfNotExists(
            arrayListOf(
                SqlRepositoryRecordCommsSender(
                    senderOneId, senderOneId, SqlRepositoryCommsSender(senderOneId, senderOneCanonicalName,
                    CosmosSqlConnection.sqlApiDateFormatter.format(oneWeekAgo))),
                SqlRepositoryRecordCommsSender(
                    nhsAppSenderId, nhsAppSenderId, SqlRepositoryCommsSender(nhsAppSenderId, nhsAppSenderCanonicalName,
                    CosmosSqlConnection.sqlApiDateFormatter.format(oneWeekAgo))
                )
            )
        )

        createMessagesInRepository(arrayListOf(senderOneMessages, senderTwoMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(
            arrayListOf(
                expectedSenderOneSummaryMessage.copy(name = senderOneCanonicalName),
                expectedSenderTwoSummaryMessage.copy(name = "NHS APP")
            )
        )

        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(expectedSenderOneMessages)
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES.set(
            arrayListOf(senderOneMessageTwo.copy(sender = senderOneCanonicalName)))
        MessagesSerenityHelpers.EXPECTED_READ_MESSAGES.set(arrayListOf(
            senderOneMessageThree.copy(sender = senderOneCanonicalName),
            senderOneMessageOne.copy(sender = senderOneCanonicalName)))
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
                senderOneSenderContext,
                null
            )
        }

        val senderMessages = createSenderMessages(messages as ArrayList<SingleMessageFacade>)
        val senderMessage = SenderFacade(senderOneName, "0", arrayListOf(messages.first()))

        createMessagesInRepository(arrayListOf(senderMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SENDERS.set(arrayListOf(senderMessage.copy(name = senderOneCanonicalName)))
        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderMessages.copy(
            name = senderOneCanonicalName,
            messages = senderMessages.messages
                .map { m -> m.copy(sender = senderOneCanonicalName) }
            )
        )
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

    fun setUpSingleReadMessage() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        val message = SingleMessageFacade(
                "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(twoMonthsAgo),
                MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, null
        )

        val messageToInsert = MongoRepositoryMessage.createJson(message, nhsLoginId)
        MongoDBConnection.MessagesCollection.clearAndInsertJson(arrayListOf(messageToInsert))

        val insertedMessage = MongoDBConnection.MessagesCollection
                .getValues<MongoRepositoryMessage>(MongoRepositoryMessage::class.java)
                .first()
        MessagesSerenityHelpers.MESSAGE_ID.set(insertedMessage._id?.toHexString())
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(insertedMessage)
    }

    fun setUpSingleMessageWithQuestionnaire() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        val message = SingleMessageFacade(
                "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(timeNow),
                MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, messageReplyWithQuestionnaire
        )

        val messageToInsert = MongoRepositoryMessage.createJson(message, nhsLoginId)
        MongoDBConnection.MessagesCollection.clearAndInsertJson(arrayListOf(messageToInsert))

        val insertedMessage = MongoDBConnection.MessagesCollection
                .getValues<MongoRepositoryMessage>(MongoRepositoryMessage::class.java)
                .first()
        MessagesSerenityHelpers.MESSAGE_ID.set(insertedMessage._id?.toHexString())
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(insertedMessage)
    }

    fun setUpSingleMessageWithQuestionnaireAndResponse() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        val message = SingleMessageFacade(
                "1.1", senderOneName, messageOne, true, MongoDBConnection.mongoDateFormatter.format(timeNow),
                MessageVersion.PLAIN_TEXT.value, senderOneSenderContext, messageReplyWithQuestionnaireAndResponse
        )

        val messageToInsert = MongoRepositoryMessage.createJson(message, nhsLoginId)
        MongoDBConnection.MessagesCollection.clearAndInsertJson(arrayListOf(messageToInsert))

        val insertedMessage = MongoDBConnection.MessagesCollection
                .getValues<MongoRepositoryMessage>(MongoRepositoryMessage::class.java)
                .first()
        MessagesSerenityHelpers.MESSAGE_ID.set(insertedMessage._id?.toHexString())
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(insertedMessage)
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

    private fun createSendersIfNotExists(
        sendersToInsert: ArrayList<SqlRepositoryRecordCommsSender<SqlRepositoryCommsSender>>
    ) {
        CosmosSqlConnection.CommsHubSendersContainer.upsertValues(sendersToInsert)
    }

    private fun createSenderMessages(messages: ArrayList<SingleMessageFacade>): SenderFacade {
        val distinctSender = messages.map { message -> message.sender }.distinct()
        Assert.assertEquals("Expected one distinct sender", 1, distinctSender.count())
        val unreadCount = messages.count { message -> !message.read }
        return SenderFacade(distinctSender.single(), unreadCount.toString(), messages)
    }

    companion object {
        val patchToUpdateAsRead = JsonPatch(JsonPatchOperation.ADD, "/read", true)
        val patchToUpdateAsReplied = JsonPatch(JsonPatchOperation.ADD, "/reply/response", MESSAGE_RESPONSE)
        val patchToUpdateAsRepliedChangedResponse = JsonPatch(JsonPatchOperation.ADD, "/reply/response",
                                                              MESSAGE_SECOND_RESPONSE)
        val patchToUpdateAsRepliedInvalidResponse = JsonPatch(JsonPatchOperation.ADD, "/reply/response",
                "TEST_RESPONSE")
    }

    private fun prefixInternalLink(link: String): String {
        if (link.startsWith("/")) {
            return "${Config.instance.url}$link"
        }
        return link
    }
}
