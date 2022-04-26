package features.sharedSteps

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import pages.ErrorDialogPage

class ErrorDialogPageSteps {
    @Steps
    private lateinit var browser: BrowserSteps
    private lateinit var errorDialogPage: ErrorDialogPage

    @When("^I click the error '(.*)' link$")
    fun iClickTheErrorLink(linkText: String) {
        errorDialogPage.assertLink(linkText).click()
    }

    @When("^I click the warning '(.*)' link$")
    fun iClickTheWarningLink(linkText: String) {
        errorDialogPage.assertWarningLink(linkText).click()
    }

    @When("^I click the warning '(.*)' link with a url of '(.*)'$")
    fun iClickTheWarningLinkWithAUrlOf(linkText: String, url: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.assertWarningLink(linkText, url).click()
    }

    @When("^I click the shutter '(.*)' link$")
    fun iClickTheShutterLink(linkText: String) {
        errorDialogPage.assertLinkOnShutter(linkText).click()
    }

    @When("^I click the error '(.*)' link with a url of '(.*)'$")
    fun iClickTheErrorLinkWithAUrlOf(linkText: String, url: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.assertLink(linkText, url).click()
    }

    @When("^I click the shutter '(.*)' link with a url of '(.*)'$")
    fun iClickTheShutterLinkWithAUrlOf(linkText: String, url: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.assertLinkOnShutter(linkText, url).click()
    }

    @Then("^I see the shutter '(.*)' link with a url of '(.*)'$")
    fun iSeeTheShutterLinkWithAUrlOf(linkText: String, url: String) {
        errorDialogPage.assertLinkOnShutter(linkText, url)
    }

    @Then("^I see a shutter '(.*)' link with a url of '(.*)' and error prefix of '(.*)'$")
    fun iSeeTheShutterLinkWithAUrlAndPrefix(linkText: String, url: String, prefix: String) {
        errorDialogPage.assertLinkWithPrefixOnShutter(linkText, url, prefix)
    }

    @Then("^I click a shutter '(.*)' link with a url of '(.*)' and error prefix of '(.*)'$")
    fun iClickTheShutterLinkWithAUrlAndPrefix(linkText: String, url: String, prefix: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.assertLinkWithPrefixOnShutter(linkText, url, prefix).click()
    }
}
