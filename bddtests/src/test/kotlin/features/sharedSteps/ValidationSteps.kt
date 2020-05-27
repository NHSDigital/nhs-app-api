package features.sharedSteps

import cucumber.api.java.en.Then
import pages.HybridPageObject
import pages.assertIsVisible
import pages.withNormalisedText

class ValidationSteps {

    lateinit var genericPage: HybridPageObject

    @Then("^I see '(.*)' error summary message$")
    fun iSeeTheErrorSummaryMessage(message: String) {
        genericPage.validationBanner.assertMessage(message)
    }

    @Then("^I see '(.*)' error summary message item$")
    fun iSeeTheErrorSummaryMessageItem(messageItem: String) {
        genericPage.validationBanner.assertMessageItem(messageItem)
    }

    @Then("^I see '(.*)' inline error$")
    fun iSeeAnInlineError(error: String) {
        genericPage.getElement("//span[@data-purpose='error']")
                .withNormalisedText(error)
                .assertIsVisible()
    }
}