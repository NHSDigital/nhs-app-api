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

    @When("^I click the error '(.*)' link with a url of '(.*)'$")
    fun iClickTheErrorLinkWithAUrlOf(linkText: String, url: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.assertLink(linkText, url).click()
    }

    @Then("^I see the error '(.*)' link with a url of '(.*)'$")
    fun iSeeTheErrorLinkWithAUrlOf(linkText: String, url: String) {
        errorDialogPage.assertLink(linkText, url)
    }
}
