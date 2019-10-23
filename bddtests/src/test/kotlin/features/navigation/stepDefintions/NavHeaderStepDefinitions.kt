package features.navigation.stepDefintions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.NavBarNative
import pages.navigation.WebHeader


class NavHeaderStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps

    lateinit var webHeader: WebHeader

    @Given("^I navigate away from the home page$")
    fun iAmOnTheRecordWarningPage() {
        nav.select(NavBarNative.NavBarType.MY_RECORD)
    }
    @When("^I click the my account icon$")
    fun iClickTheMyAccountIcon() {
        navHeader.clickMyAccount()
    }

    @When("^I click the help icon$")
    fun iClickTheHelpIcon() {
        browser.storeCurrentTabCount()
        navHeader.clickHelp()
    }

    @When("^I click the home icon$")
    fun iClickTheNHSLogo() {
        navHeader.clickHome()
    }

    @When("^I click the appointments tab$")
    fun iClickTheAppointmentsTab(){
        webHeader.clickAppointmentsPageLink()
    }

    @When("^I click the back link$")
    fun iClickTheBackLink() {
        webHeader.clickBackLink()
    }

    @Then("^the back bar is visible$")
    fun theBackBarIsVisible() {
        webHeader.assertBackLinkBarVisible()
    }

    @Then("^the back bar is not visible$")
    fun theBackBarIsNotVisible() {
        webHeader.assertBackLinkBarNotVisible()
    }

    @Then("^I see the header$")
    fun iSeeHeader() {
        navHeader.assertHomePageHeaderVisible()
    }
}
