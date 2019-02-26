package features.sharedStepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.BrowserSteps
import features.sharedSteps.PageUrl
import net.thucydides.core.annotations.Steps
import pages.HybridPageObject
import pages.clickOnActionContainingText

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
        val urlForPage = pageUrl.getPage(pageName,genericPage.onMobile())
        browser.browseTo(urlForPage)
    }

    @When("I click the link called '(.*)' with a url of '(.*)'")
    fun iClickANamedLinkWithAUrl(linkTitle: String, url: String){
        browser.storeCurrentTabCount()
        genericPage.assertLinkExistsAndClickIt(linkTitle,url)
    }

    @Then("^a new tab has been opened by the link")
    fun aNewTabOpens() {
        browser.assertNewTab()
    }
}
