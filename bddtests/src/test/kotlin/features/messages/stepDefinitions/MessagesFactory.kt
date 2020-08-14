package features.messages.stepDefinitions

import config.Config
import io.cucumber.datatable.DataTable
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryMessage
import org.junit.Assert
import utils.SerenityHelpers
import utils.set
import utils.toSingleElementList
import worker.JsonPatch
import worker.JsonPatchOperation
import worker.models.messages.MessageRequest
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val SEVEN_DAYS: Long = 7
private const val TWO_MONTHS: Long = 2
private const val ONE_MONTH: Long = 1
class MessagesFactory {

    private val frontendSummaryDateFormatter = DateTimeFormatter.ofPattern("dd/MM/yyyy")
    private val twoMonthsAgo = ZonedDateTime.now(ZoneId.of("Europe/London")).minusMonths(TWO_MONTHS)
    private val oneMonthAgo = ZonedDateTime.now(ZoneId.of("Europe/London")).minusMonths(ONE_MONTH)
    private val oneWeekAgo = ZonedDateTime.now(ZoneId.of("Europe/London")).minusDays(SEVEN_DAYS)

    private val senderOne = "Sender One"
    private val senderTwo = "Sender Two"

    fun setUpUser(patient: Patient? = null) {
        val patientToUse = patient
                ?: ServiceJourneyRulesMapper.findPatientForConfiguration(
                        null,
                        SJRJourneyType.MESSAGES_ENABLED)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney().createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                .createFor(patientToUse)
        MessagesSerenityHelpers.TARGET_SENDER.set(senderOne)
    }

    fun setUpMultipleMessagesInCache() {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val senderOneMessageOne = createReadMessage(senderOne, "Message One", twoMonthsAgo)
        val senderOneMessageTwo = createUnreadMessage(senderOne, "Message Two", oneMonthAgo)
        val senderOneMessageThree = createReadMessage(senderOne, "Message Three", oneWeekAgo)
        val senderOneMessages = createSenderMessages(
                arrayListOf(senderOneMessageOne, senderOneMessageTwo, senderOneMessageThree))
        val senderOneSummaryMessage = MessagesSummaryFacade(senderOne, 1, arrayListOf(senderOneMessageThree),
                lastMessageTime = frontendSummaryDateFormatter.format(oneWeekAgo))

        val senderTwoMessageOne = createUnreadMessage(senderTwo, "Message Three", twoMonthsAgo)
        val senderTwoMessages = createSenderMessages(arrayListOf(senderTwoMessageOne))
        val senderTwoSummaryMessage = MessagesSummaryFacade(senderTwo, 1, arrayListOf(senderTwoMessageOne),
                lastMessageTime = frontendSummaryDateFormatter.format(twoMonthsAgo))

        createMessagesInRepository(arrayListOf(senderOneMessages, senderTwoMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.set(
                arrayListOf(senderOneSummaryMessage, senderTwoSummaryMessage))
        MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES_AFTER_READING_SENDER.set(
                arrayListOf(  afterReading(senderOneSummaryMessage), senderTwoSummaryMessage))

        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderOneMessages)
        MessagesSerenityHelpers.EXPECTED_UNREAD_MESSAGES.set(arrayListOf(senderOneMessageTwo, senderOneMessageThree))
        MessagesSerenityHelpers.EXPECTED_READ_MESSAGES.set(arrayListOf(senderOneMessageOne))
    }

    fun setUpMultipleMessagesWithContentInCache(table: DataTable) {
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val links = table.toSingleElementList()
        val messages = links.mapIndexed{ index, link -> createReadMessage(senderOne,
                "message ${prefixInternalLink(link)}", twoMonthsAgo.minusDays(index.toLong()))}

        val senderMessages = createSenderMessages(messages as ArrayList<SingleMessageFacade>)
        val senderSummaryMessage = MessagesSummaryFacade(senderOne, 0, arrayListOf(messages.first()),
                lastMessageTime = frontendSummaryDateFormatter.format(twoMonthsAgo))

        createMessagesInRepository(arrayListOf(senderMessages), nhsLoginId)

        MessagesSerenityHelpers.EXPECTED_SUMMARY_MESSAGES.set(arrayListOf(senderSummaryMessage))
        MessagesSerenityHelpers.EXPECTED_MESSAGES_FROM_SENDER.set(senderMessages)
    }

    private fun afterReading(senderOneSummaryMessage: MessagesSummaryFacade):MessagesSummaryFacade{
        return  senderOneSummaryMessage.copy(
                unreadCount = 0,
                messages = senderOneSummaryMessage.messages.map { message -> message.copy(read = true) })
    }

    fun setUpSingleUnreadMessage() {
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)

        val messageToPost = MessageRequest(senderOne, "Message One", 1)
        val response = MessagesApi.postSetup(messageToPost, nhsLoginId)
        MessagesSerenityHelpers.MESSAGE_ID.set(response?.messageId)
        MessagesSerenityHelpers.EXPECTED_MESSAGE.set(messageToPost)
    }

    private fun createReadMessage(sender: String, body: String, sentTime: ZonedDateTime): SingleMessageFacade {
        return SingleMessageFacade(sender,
                body,
                true,
                MongoDBConnection.mongoDateFormatter.format(sentTime)
        )
    }

    fun setUpInvalidMessageInCache(){
        val nhsLoginId = SerenityHelpers.getPatient().subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        MongoDBConnection.MessagesCollection.clearAndInsertJson(listOf(asInvalidJson(nhsLoginId, senderOne)))
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
                .flatMap { senderMessage -> senderMessage.messages.map {
                    message -> MongoRepositoryMessage.createJson(message, nhsLoginId) } }
        MongoDBConnection.MessagesCollection.clearAndInsertJson(messagesToInsert)
    }

    private fun createSenderMessages(messages: ArrayList<SingleMessageFacade>): MessagesSummaryFacade {
        val distinctSender = messages.map { message -> message.sender }.distinct()
        Assert.assertEquals("Expected one distinct sender", 1, distinctSender.count())
        val unreadCount = messages.count { message -> !message.read }
        return MessagesSummaryFacade(distinctSender.single(), unreadCount, messages)
    }

    companion object{
        val patchToUpdateAsRead = JsonPatch(JsonPatchOperation.ADD, "/read",true)
    }

    private fun asInvalidJson(nhsLoginId: String, sender: String): String {
        return "{" +
                "\"NhsLoginId\" : \"$nhsLoginId\"," +
                "\"Sender\" : \"$sender\"," +
                "\"Blah\" : \"Blah\"," +
                "},"
    }

    private fun prefixInternalLink(link: String): String{
        if(link.startsWith("/")) {
          return "${Config.instance.url}$link"
        }
        return link
    }
}
