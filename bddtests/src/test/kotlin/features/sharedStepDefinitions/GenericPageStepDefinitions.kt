package features.sharedStepDefinitions

import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.PageUrl
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject

private const val TIMEOUT_PLUS_ONE_SECOND = 11L

class GenericPageStepDefinitions {

    lateinit var genericPage: HybridPageObject
    @Steps
    lateinit var pageUrl: PageUrl
    @Steps
    lateinit var browser: BrowserSteps

     @When("^I click the '(.*)' action$")
     fun iClickTheAction(actionText: String) {
         genericPage.clickOnActionContainingText(actionText)
     }

     @When("^I click the '(.*)' button$")
     fun iClickTheButton(buttonText: String) {
         genericPage.clickOnButtonContainingText(buttonText)
     }

    @When("^I click the '(.*)' link$")
    fun iClickTheLink(linkText: String) {
        genericPage.clickOnActionContainingText(linkText)
    }

    @When("^I click the Back link$")
    fun iClickTheBackLink() {
        genericPage.clickOnBackLink()
    }

    @When("^I retrieve the '(.*)' page directly$")
    fun iretrieveThePageDirectly(pageName:String) {
        val urlForPage = pageUrl.getPage(pageName)
        browser.browseTo(urlForPage)
    }
}
