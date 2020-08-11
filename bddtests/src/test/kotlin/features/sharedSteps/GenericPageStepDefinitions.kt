package features.sharedSteps

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageObject
import pages.clickOnActionContainingText

class GenericPageStepDefinitions {
    private lateinit var notFoundErrorPage: NotFoundErrorPage
    private lateinit var genericPage: HybridPageObject

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
        val urlForPage = PageUrl.getRelativePagePath(pageName)
        browser.browseTo(urlForPage)
    }

    @When("I click the link called '(.*)' with a url of '(.*)'")
    fun iClickANamedLinkWithAUrl(linkTitle: String, url: String) {
        browser.storeCurrentTabCount()
        genericPage.assertLinkExists(linkTitle, url, internal = false).click()
    }

    @When("I click the link called '(.*)' with the COVID-19 service url")
    fun iClickANamedLinkWithTheCovidServiceUrl(linkTitle: String) {
        val url = "http://stubs.local.bitraft.io:8080/external/covid"
        browser.storeCurrentTabCount()
        genericPage.assertLinkExists(linkTitle, url, internal = false).click()
    }

    @Then("I see the error reference code with prefix '(.*)'")
    fun iSeeTheErrorReferenceCode(prefix: String){
        genericPage.containsText("Reference: $prefix")
    }

    @Then("^a new tab has been opened by the link")
    fun aNewTabOpens() {
        browser.assertNewTab()
    }

    @Then("^I am redirected to the '(.*)' page$")
    fun thenIAmRedirectedToThePage(pageName: String) {
        val redirectUrl = PageUrl.getRelativePagePath(pageName)
        browser.shouldEndWithUrl(redirectUrl)
    }

    @Then("^I press the tab key$")
    fun iPressTheTabKey() {
        genericPage.driver
                .switchTo()
                .activeElement()
                .sendKeys(Keys.TAB)
    }

    @Then("^I check that '(.*)' is in focus$")
    fun iCheckThatElementIsInFocus(name: String) {
        Assert.assertEquals(name, genericPage.driver
                .switchTo()
                .activeElement()
                .text)
    }

    @Then("^the Page not found error is displayed$")
    fun thePageNotFoundErrorIsDisplayed() {
        notFoundErrorPage.assertNotFoundErrorPage()
    }

    @When("^I click the '(.*)' radio button$")
    fun iClickTheRadioButton(labelText: String) {
        val label = genericPage.getElement("label")
                .withText(labelText, exact = false)

        genericPage.getElement("//input[@type='radio'][following-sibling::${label.webDesktopLocator}]")
                .click()
    }
}
