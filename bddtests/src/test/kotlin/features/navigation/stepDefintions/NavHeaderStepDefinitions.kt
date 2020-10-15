package features.navigation.stepDefintions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import pages.navigation.NavBarNative

class NavHeaderStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps

    @Given("^I navigate away from the home page$")
    fun iAmOnTheRecordWarningPage() {
        nav.select(NavBarNative.NavBarType.YOUR_HEALTH)
    }
    @When("^I click the settings icon$")
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

    @Then("^I see the header$")
    fun iSeeHeader() {
        navHeader.assertHomePageHeaderVisible()
    }
}
