package features.sharedSteps

import io.cucumber.java.en.Then
import pages.HybridPageObject
import pages.assertIsVisible
import pages.withNormalisedText

class ValidationSteps {

    lateinit var genericPage: HybridPageObject

    @Then("^I see '(.*)' form error summary heading$")
    fun iSeeTheFormErrorSummaryHeading(heading: String) {
        genericPage.validationBanner.assertFormErrorSummaryHeading(heading)
    }

    @Then("^I see '(.*)' form error summary reason item$")
    fun iSeeTheFormErrorSummaryReasonItem(reasonItem: String) {
        genericPage.validationBanner.assertFormErrorSummaryReasonItem(reasonItem)
    }

    @Then("^I see '(.*)' inline error$")
    fun iSeeAnInlineError(error: String) {
        genericPage.getElement("//span[@class='nhsuk-error-message']")
                .withNormalisedText(error)
                .assertIsVisible()
    }
}
