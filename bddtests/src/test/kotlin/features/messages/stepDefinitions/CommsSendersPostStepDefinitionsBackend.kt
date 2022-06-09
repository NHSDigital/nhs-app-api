package features.messages.stepDefinitions

import cosmosSql.SqlRepositoryCommsSender
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.addToList
import utils.getOrFail
import worker.models.messages.CommsSenderRequest
import worker.models.messages.CommsSenderResponse

class CommsSendersPostStepDefinitionsBackend {

    @Given("^I am an api user wishing to submit sender details to the senders endpoint")
    fun iAmAWishingToPostACommsSender() {
        val factory = CommsSendersFactory()
        factory.createCommsSender()
    }

    @When("^I post to the senders endpoint without an api key$")
    fun iPostCommsSenderDetailsWithoutApiKey() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<CommsSenderRequest>()
        CommsSendersApi.post(sender = sender, includeApiKey = false)
    }

    @When("^I post to the senders endpoint without name$")
    fun iPostCommsSenderDetailsWithoutName() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<CommsSenderRequest>()
        sender.name = ""
        CommsSendersApi.post(sender = sender)
    }

    @When("^I post to the senders endpoint with valid body$")
    fun iPostCommsSenderDetailsWithValidBody() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<CommsSenderRequest>()
        CommsSendersApi.post(sender = sender)

        val sqlRecord = SqlRepositoryCommsSender(
            id = sender.id,
            Name = sender.name
        )

        val deletion = { CommsSendersFactory().deleteItemInSqlContainer(sqlRecord) }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
    }

    @Then("^I receive a created sender details response from senders endpoint$")
    fun iReceiveCreatedCommsSenderDetailsFromTheSendersEndpoint() {
        val responseSender = MessagesSerenityHelpers.CREATE_COMMS_SENDERS_RESPONSE
            .getOrFail<CommsSenderResponse>()
        val expectedSender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER
            .getOrFail<CommsSenderRequest>()

        Assert.assertEquals("Sender Id.", expectedSender.id, responseSender.id)
        Assert.assertEquals("Sender Name.", expectedSender.name, responseSender.name)
    }
}
