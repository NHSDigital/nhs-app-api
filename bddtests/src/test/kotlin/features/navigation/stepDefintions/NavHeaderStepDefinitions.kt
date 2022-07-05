package features.navigation.stepDefintions

import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps

class NavHeaderStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps

    @When("^I click the more icon$")
    fun iClickTheMoreIcon() {
        navHeader.clickMore()
    }

    @When("^I click the help and support link$")
    fun iClickTheHelpIcon() {
        browser.storeCurrentTabCount()
        navHeader.clickHelpAndSupport()
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
