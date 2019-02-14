package features.sharedStepDefinitions

import cucumber.api.java.en.When
import pages.HybridPageObject

private const val TIMEOUT_PLUS_ONE_SECOND = 11L

class GenericPageStepDefinitions {

    lateinit var genericPage: HybridPageObject

     @When("^I click the '(.*)' button$")
     fun iClickTheButton(buttonText: String) {
         genericPage.waitForSpinnerToDisappear(TIMEOUT_PLUS_ONE_SECOND)
         genericPage.clickOnButtonContainingText(buttonText)
     }

    @When("^I click the '(.*)' link$")
    fun iClickTheLink(linkText: String) {
        genericPage.waitForSpinnerToDisappear(TIMEOUT_PLUS_ONE_SECOND)
        genericPage.clickOnLinkContainingText(linkText)
    }
}
