package features.messages.stepDefinitions

import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryMessage
import org.junit.Assert
import utils.SerenityHelpers
import utils.set
import worker.JsonPatch
import worker.JsonPatchOperation
import worker.models.messages.MessageRequest
import worker.models.messages.MessagesSummaryFacade
import worker.models.messages.SingleMessageFacade
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val SEVEN_DAYS: Long = 7
private const val TWO_MONTHS: Long = 2
private const val ONE_MONTH: Long = 1
private const val THREE_DAYS: Long = 3
class MessagesFactory {

    val mockingClient = MockingClient.instance

    private val frontendSummaryDateFormatter = DateTimeFormatter.ofPattern("dd/MM/yyyy")
    private val twoMonthsAgo = ZonedDateTime.now().minusMonths(TWO_MONTHS)
    private val oneMonthAgo = ZonedDateTime.now().minusMonths(ONE_MONTH)
    private val oneWeekAgo = ZonedDateTime.now().minusDays(SEVEN_DAYS)
    private val threeDaysAgo = ZonedDateTime.now().minusDays(THREE_DAYS)

    private val senderOne = "Sender One"
    private val senderTwo = "Sender Two"

    fun setUpUser(patient: Patient? = null) {
        val patientToUse = patient ?:
        ServiceJourneyRulesMapper.findPatientForConfiguration(null,
                ServiceJourneyRulesMapper.Companion.JourneyType.MESSAGES_ENABLED)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(SerenityHelpers.getGpSupplier(), mockingClient)
                .createFor(patientToUse)
        MongoDBConnection.MessagesCollection.clearCache()
        MessagesSerenityHelpers.TARGET_SENDER.set(senderOne)
    }

    fun setUpMultipleMessagesInCache() {
        MongoDBConnection.MessagesCollection.clearCache()
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

    fun afterReading(senderOneSummaryMessage: MessagesSummaryFacade):MessagesSummaryFacade{
        return  senderOneSummaryMessage.copy(
                unreadCount = 0,
                messages = senderOneSummaryMessage.messages.map { message -> message.copy(read = true) })
    }

    fun setUpSingleUnreadMessage() {
        MongoDBConnection.MessagesCollection.clearCache()
        val patient = SerenityHelpers.getPatient()
        val nhsLoginId = patient.subject
        MessagesSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(nhsLoginId)
        val authToken = patient.accessToken

        val messageToPost = MessageRequest(senderOne, "Message One", 1)
        MessagesApi.post(messageToPost, nhsLoginId)
        val response = MessagesApi.getFromSender(authToken, senderOne)!!.single()
        MessagesSerenityHelpers.MESSAGE_ID.set(response.messages.single().id)
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
        MongoDBConnection.MessagesCollection.clearCache()
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
}