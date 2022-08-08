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

    @Given("^I am an api user wishing to get senders existing in database whose details have not been recently updated")
    fun iAmWishingToGetStaleCommsSender() {
        val factory = CommsSendersFactory()
        factory.setUpMultipleCommsSenders(recordsAreStale = true)
    }

    @Given("^I am an api user without stale senders stored details wishing to get stale senders")
    fun iAmWishingToGetStaleCommsSenderWithoutStoredDetails() {
        val factory = CommsSendersFactory()
        factory.setUpMultipleCommsSenders(recordsAreStale = false)
    }

    @When("^I get sender details based on an sender id$")
    fun iGetCommsSenderDetailsBasedOnSenderId() {
        val sender = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.getOrFail<SqlRepositoryCommsSender>()
        CommsSendersApi.get(senderId = sender.id)
    }

    @When("^I get sender details based on when they were last updated")
    fun iGetCommsSenderDetailsBasedOnLastUpdated() {
        val lastUpdatedBefore = MessagesSerenityHelpers.TARGET_LAST_UPDATED_BEFORE.getOrFail<String>();
        CommsSendersApi.getLastUpdatedBefore(lastUpdatedBefore, limit = 1000)
    }

    @When("^I get sender details based on when they were last updated without the api key")
    fun iGetCommsSenderDetailsBasedOnLastUpdatedWithoutApiKey() {
        val lastUpdatedBefore = MessagesSerenityHelpers.TARGET_LAST_UPDATED_BEFORE.getOrFail<String>();
        CommsSendersApi.getLastUpdatedBefore(lastUpdatedBefore, limit = 1000, includeApiKey = false)
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

    @Then("^I receive ids of senders not recently updated from senders endpoint")
    fun iReceiveCommsSenderIdsFromTheSendersEndpoint() {
        val responseSenderIds = MessagesSerenityHelpers.GET_COMMS_SENDERS_RESPONSE
            .getOrFail<List<String>>()
        val expectedSenderIds = MessagesSerenityHelpers.EXPECTED_COMMS_SENDER
            .getOrFail<List<String>>()

        Assert.assertTrue(responseSenderIds.containsAll(expectedSenderIds))
    }

    @Then("^I receive an empty list of sender ids not recently updated from senders endpoint")
    fun iReceiveEmptyCommsSenderIdsFromTheSendersEndpoint() {
        val responseSenderIds = MessagesSerenityHelpers.GET_COMMS_SENDERS_RESPONSE
                .getOrFail<List<String>>()
        Assert.assertTrue("Expected sender ids to be empty", responseSenderIds.isEmpty())
    }
}
