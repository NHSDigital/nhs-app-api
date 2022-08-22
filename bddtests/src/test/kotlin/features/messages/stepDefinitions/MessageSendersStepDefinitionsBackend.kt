package features.messages.stepDefinitions
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.messages.Sender
import worker.models.messages.SenderFacade
import worker.models.messages.SendersResponse

class MessageSendersStepDefinitionsBackend {

    private fun setUpHandler(isV1SendersEndpoint: Boolean = false)
    {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpMultipleMessagesInCache(isV1SendersEndpoint)
    }

    @Given("^I am an api user wishing to get my messages from v1 endpoint$")
    fun iAmAnApiUserWishingToGetTheirMessagesfromV1Endpoint() {
        setUpHandler(true)
    }

    @Then("^I can see a list of message senders along with a count of unread messages per sender$")
    fun iCanSeeAListOfMessageSendersAlongWithACountOfUnreadMessagesPerSender() {
        val response = MessagesSerenityHelpers.GET_SENDERS.getOrFail<SendersResponse>()
        val expectedMessages = MessagesSerenityHelpers.EXPECTED_SENDERS
            .getOrFail<ArrayList<SenderFacade>>()
        Assert.assertNotNull(response)
        Assert.assertNotNull(response.senders)
        val expectedSenders = expectedMessages.map { message ->
            Sender(null, message.name, message.unreadCount)
        }

        Assert.assertEquals("Number Of Senders", expectedSenders.count(), response.senders.count())

        for (x in 0 until expectedSenders.count()){
            Assert.assertEquals(expectedSenders[x].name, response.senders[x].name)
            Assert.assertEquals(expectedSenders[x].unreadCount, response.senders[x].unreadCount)
        }
    }

    @Given("^I am an api user with an unread message$")
    fun iAmAnApiUserWithAnUnreadMessage() {
        val factory = MessagesFactory()
        factory.setUpUser()
        factory.setUpSingleUnreadMessage()
    }

    @When("^I try to get the message senders without passing an access token$")
    fun iTryToGetTheMessageSendersWithoutPassingAnAccessToken() {
        val authToken = ""
        MessagesApi.getSenders(authToken)
    }

    @When("^I get a summary of my messages from the api without an access token$")
    fun iGetMyMessagesFromTheApiWithoutAuthToken() {
        MessagesApi.getSummary(authToken = null)
    }

    @Given("^I am an api user wishing to get my messages, but I have no messages$")
    fun iAmAnApiUserWishingToGetTheirMessagesButIHaveNoMessages() {
        val factory = MessagesFactory()
        factory.setUpUser()
    }

    @When("^I try to get a list of message senders$")
    fun iTryToGetAListOfMessageSenders() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getSenders(authToken)
    }

    @Given("^I am an api user wishing to get my messages$")
    fun iAmAnApiUserWishingToGetTheirMessages() {
        setUpHandler()
    }

    @When("^I get my messages from a sender by name from the api without an access token$")
    fun iGetMyMessagesFromASenderByNameFromTheApiWithoutAuthToken() {
        val targetSender = MessagesSerenityHelpers.TARGET_SENDER_NAME.getOrFail<String>()
        MessagesApi.getFromSenderByName(null, targetSender)
    }

    @When("^I try to get a list of message senders from v2 endpoint$")
    fun iTryToGetAListOfMessageSendersV2() {
        val authToken = SerenityHelpers.getPatient().accessToken
        MessagesApi.getSendersV2(authToken)
    }

    @Then("^I can see a list of message senders from v2 endpoint along with a count of unread messages per sender$")
    fun iCanSeeAListOfMessageSendersAlongWithACountOfUnreadMessagesPerSenderV2() {
        val response = MessagesSerenityHelpers.GET_SENDERS.getOrFail<SendersResponse>()
        val expectedMessages = MessagesSerenityHelpers.EXPECTED_SENDERS
            .getOrFail<ArrayList<SenderFacade>>()
        Assert.assertNotNull(response)
        Assert.assertNotNull(response.senders)
        val expectedSenders = expectedMessages.map { message ->
            Sender(
                message.messages[0].senderContext!!.senderId,
                message.name,
                message.unreadCount
            )
        }

        Assert.assertEquals("Number Of Senders", expectedSenders.count(), response.senders.count())

        for (x in 0 until expectedSenders.count()){
            Assert.assertEquals(expectedSenders[x].id, response.senders[x].id)
            Assert.assertEquals(expectedSenders[x].name, response.senders[x].name)
            Assert.assertEquals(expectedSenders[x].unreadCount, response.senders[x].unreadCount)
        }
    }

    @When("^I try to get the message senders from v2 endpoint without passing an access token$")
    fun iTryToGetTheMessageSendersWithoutPassingAnAccessTokenV2() {
        val authToken = ""
        MessagesApi.getSendersV2(authToken)
    }

}
