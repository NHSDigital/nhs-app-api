package features.navigation.stepDefintions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.navigation.steps.NavHeaderSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps


class NavHeaderStepDefinitions {

    @Steps
    lateinit var navHeader: NavHeaderSteps
    @Steps
    lateinit var nav: NavigationSteps

    @When("^I click the my account icon$")
    fun iClickTheMyAccountIcon() {
        navHeader.clickMyAccount()
    }
    @When("^I click the help icon$")
    fun iClickTheHelpIcon() {
        navHeader.clickHelp()
    }

    @When("^I click the nhs logo$")
    fun iClickTheNHSLogo() {
        navHeader.clickHome()
    }

    @And("^I see the header$")
    fun iSeeHeader() {
        navHeader.assertHeaderVisible()
    }

    @Given("^I navigate away from the home page$")
    fun iAmOnTheRecordWarningPage() {
        nav.select("MY_RECORD")
    }
}
