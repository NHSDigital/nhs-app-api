package features.messages.stepDefinitions

import cosmosSql.SqlRepositoryCommsSender
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.getOrFail
import worker.models.messages.CommsSenderResponse

class CommsSendersGetStepDefinitionsBackend {
    @Given("^I am an api user wishing to get the sender details existing in database")
    fun iAmWishingToGetACommsSender() {
        val factory = CommsSendersFactory()
        factory.setUpCommsSender()
    }

    @Given("^I am an api user without stored details wishing to get sender details")
    fun iAmWishingToGetACommsSenderWithoutStoredDetails() {
        val factory = CommsSendersFactory()
        factory.setUpCommsSender(isStored = false)
    }

    @When("^I get sender details based on an sender id$")
    fun iGetCommsSenderDetailsBasedOnSenderId() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<SqlRepositoryCommsSender>()
        CommsSendersApi.get(senderId = sender.id)
    }

    @When("^I get sender details for non-existing sender id$")
    fun iGetCommsSenderDetailsForNonExistingSenderId() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<SqlRepositoryCommsSender>()
        CommsSendersApi.get(senderId = sender.id)
    }

    @When("^I get sender details based on sender id without the api key$")
    fun iGetCommsSenderDetailsWithoutApiKey() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<SqlRepositoryCommsSender>()
        CommsSendersApi.get(senderId = sender.id, includeApiKey = false)
    }

    @Then("^I receive a sender data from senders endpoint$")
    fun iReceiveCommsSenderDetailsFromTheSendersEndpoint() {
        val responseSender = MessagesSerenityHelpers.GET_COMMS_SENDERS_RESPONSE
            .getOrFail<CommsSenderResponse>()
        val expectedSender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER
            .getOrFail<SqlRepositoryCommsSender>()

        Assert.assertEquals("Sender Id.", expectedSender.id, responseSender.id)
        Assert.assertEquals("Sender Name.", expectedSender.Name, responseSender.name)
    }
}
