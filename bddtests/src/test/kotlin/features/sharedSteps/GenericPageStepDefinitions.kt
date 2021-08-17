package features.sharedSteps

import features.authentication.steps.HomeSteps
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.clickOnActionContainingText
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import pages.withNormalisedText

class GenericPageStepDefinitions {
    private lateinit var notFoundErrorPage: NotFoundErrorPage
    private lateinit var genericPage: HybridPageObject
    private lateinit var webHeader: WebHeader

    @Steps
    lateinit var browser: BrowserSteps

    @Steps
    lateinit var home: HomeSteps

    @Steps
    lateinit var navSteps: NavigationSteps

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

    @When("^I click the link called '(.*)' with a url of '(.*)'$")
    fun iClickANamedLinkWithAUrl(linkTitle: String, url: String) {
        browser.storeCurrentTabCount()
        genericPage.assertLinkExists(linkTitle, url, internal = false).click()
    }

    @When("^I click the internal link called '(.*)' with a url of '(.*)'$")
    fun iClickANamedInternalLinkWithAUrl(linkTitle: String, url: String) {
        browser.storeCurrentTabCount()
        genericPage.assertLinkExists(linkTitle, url, internal = true).click()
    }

    @Then("^I see the error reference code with prefix '(.*)'$")
    fun iSeeTheErrorReferenceCode(prefix: String){
        genericPage.containsText("Reference: $prefix")
    }

    @Then("^a new tab has been opened by the link$")
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

    @Then("^I can't see the (.*) menu link$")
    fun iCantSeeTheMenuLink(linkText: String) {
        menuLink(linkText).assertElementNotPresent()
    }

    @When("^I click the (.*) menu link$")
    fun iClickTheMenuLink(linkText: String) {
        browser.storeCurrentTabCount()
        menuLink(linkText).click()
    }

    @When("^I click the '(.*)' radio button$")
    fun iClickTheRadioButton(labelText: String) {
        val label = genericPage.getElement("label")
            .withText(labelText, exact = false)

        genericPage.getElement("//input[@type='radio'][following-sibling::${label.webDesktopLocator}]")
            .click()
    }

    @Then("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navSteps.hasSelectedTab(NavBarNative.NavBarType.valueOf(type.toUpperCase())))
    }

    @Then("^none of the menu buttons are highlighted")
    fun iDoNotSeeAHighlightedMenuButton() {
        if (home.headerNative.onMobile()) {
            Assert.assertFalse("Nav bar has highlighted item, expected none", navSteps.hasAnyTabSelected())
        }
    }

    @Then("^the page title is '(.*)'$")
    fun thePageTitleIsYourAppointments(title: String) {
        webHeader.getPageTitle().withNormalisedText(title).assertIsVisible()
    }

    @Then("^the page contains the header '(.*)'$")
    fun thePageContainsTheHeaderText(title: String) {
        webHeader.getHtmlElement("h2").withNormalisedText(title).assertIsVisible()
    }

    private fun menuLink(linkText: String): HybridPageElement {
        return genericPage.getElement(
            "//ul//a/div/h2",
            helpfulName = "$linkText Link"
        ).withText(linkText, false)
    }
}
