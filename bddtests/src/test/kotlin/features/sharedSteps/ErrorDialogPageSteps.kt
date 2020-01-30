package features.sharedSteps

import cucumber.api.java.en.When
import net.thucydides.core.annotations.Steps
import pages.ErrorDialogPage

class ErrorDialogPageSteps {
    @Steps
    private lateinit var browser: BrowserSteps
    private lateinit var errorDialogPage: ErrorDialogPage

    @When("^I click the error '(.*)' link$")
    fun iClickTheErrorLink(linkText: String) {
        errorDialogPage.clickOnLink(linkText)
    }

    @When("^I click the error '(.*)' link with a url of '(.*)'$")
    fun iClickTheErrorLinkWithAUrlOf(linkText: String, url: String) {
        browser.storeCurrentTabCount()
        errorDialogPage.clickOnLink(linkText, url)
    }
}