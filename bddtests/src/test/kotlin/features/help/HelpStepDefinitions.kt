package features.help

import features.sharedSteps.BrowserSteps
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import mocking.pages.help.NhsAppointmentsHelpPage
import mocking.pages.help.NhsMessagingHelpPage
import mocking.pages.help.NhsPrescriptionsHelpPage
import mocking.pages.help.NhsYourHealthHelpPage
import net.thucydides.core.annotations.Steps
import pages.navigation.WebHeader

class HelpStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    private lateinit var webHeader: WebHeader
    private lateinit var nhsAppointmentsHelpPage: NhsAppointmentsHelpPage
    private lateinit var nhsPrescriptionsHelpPage: NhsPrescriptionsHelpPage
    private lateinit var nhsMessagingHelpPage: NhsMessagingHelpPage
    private lateinit var nhsYourHealthHelpPage: NhsYourHealthHelpPage

    @When("^I click help and support$")
    fun iClickHelpAndSupport() {
        webHeader.clickHelpAndSupportLink()
    }

    @Then("^I see the appointments help page$")
    fun iSeeTheCorrectAppointmentsHelpPage() {
        browser.changeTab(nhsAppointmentsHelpPage.url)
        nhsAppointmentsHelpPage.assertTitle()
    }

    @Then("^I see the prescriptions help page$")
    fun iSeeTheCorrectPrescriptionsHelpPage() {
        browser.changeTab(nhsPrescriptionsHelpPage.url)
        nhsPrescriptionsHelpPage.assertTitle()
    }

    @Then("^I see the messaging help page$")
    fun iSeeTheCorrectMessagingHelpPage() {
        browser.changeTab(nhsMessagingHelpPage.url)
        nhsMessagingHelpPage.assertTitle()
    }

    @Then("^I see the your health help page$")
    fun iSeeTheCorrectYourHealthHelpPage() {
        browser.changeTab(nhsYourHealthHelpPage.url)
        nhsYourHealthHelpPage.assertTitle()
    }
}
