package features.messagesHub.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.MessagesHubPage

class MessagesHubStepDefinitions {
    private lateinit var messagesHubPage: MessagesHubPage

    @When("^I click on the NHS App Messages link on the Messages Hub page$")
    fun clickOnNHSAppMessages() {
        messagesHubPage.clickOnMenuItem("btn_appMessaging")
    }

    @When("^I click the PKB Messages and online consultations link on the Messages Hub page$")
    fun clickOnPkbMessagesConsultations() {
        messagesHubPage.clickOnMenuItem("btn_pkb_messages_and_consultations")
    }

    @When("^I click the CIE Messages and online consultations link on the Messages Hub page$")
    fun clickOnCieMessagesConsultations() {
        messagesHubPage.clickOnMenuItem("btn_pkb_cie_messages_and_consultations")
    }

    @When("^I click on the patient practice Messages link on the Messages Hub page$")
    fun clickOnPatientPracticeMessages() {
        messagesHubPage.clickOnMenuItem("btn_im1_messaging")
    }

    @Then("^the link to NHS App Messages link is not displayed$")
    fun theAppMessagesLinkIsNotDisplayed() {
        messagesHubPage.assertMenuItemNotDisplayed("btn_appMessaging")
    }

    @Then("^the link to Messages and consultations is not available on the Messages Hub page")
    fun theMessagesConsultationsLinkIsNotDisplayed() {
        messagesHubPage.assertMenuItemNotDisplayed("btn_pkb_messages_and_consultations")
    }

    @Then("^the Messages Hub page is displayed$")
    fun assertIsDisplayed() {
        messagesHubPage.assertMessagesHubPageDisplayed()
    }

    @Then("^the Messages Hub page is displayed with the unread indicator for GP messaging")
    fun assertIsDisplayedWithUnreadIndicatorGPMessaging() {
        messagesHubPage.assertUnreadGPIndicatorDisplayed()
    }

    @Then("^the Messages Hub page is displayed with the unread indicator for app messaging")
    fun assertIsDisplayedWithUnreadIndicatorAppMessaging() {
        messagesHubPage.assertUnreadAppIndicatorDisplayed()
    }


}
