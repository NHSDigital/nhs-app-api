package features.redirector.stepDefinitions

import config.Config
import features.sharedSteps.BrowserSteps
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.RedirectorPage

class RedirectorStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    private lateinit var redirector: RedirectorPage

    @When("I navigate to the redirector page with a url of '(.*)'$")
    fun iNavigateToRedirectPageWith(path: String) {
        browser.browseToInternal(path)
    }

    @When("^I click the 'Continue' button on the redirector page with a url starting with '(.*)'$")
    fun iClickTheContinueButtonOnTheRedirectorPageWithAUrlOf(continueUrl: String) {
        redirector.interruptionCard.assertContinueAndClick(continueUrl)
    }

    @Then("I am redirected to the redirector page with the header '(.*)'$")
    fun assertRedirectorPageIsVisible(header: String) {
        redirector.title(header).waitForElement()
    }

    @Then("^I am navigated to a third party site$")
    fun assertNoLongerOnNhsAppSite() {
        browser.shouldNotHaveUrl(Config.instance.url)
    }
}
