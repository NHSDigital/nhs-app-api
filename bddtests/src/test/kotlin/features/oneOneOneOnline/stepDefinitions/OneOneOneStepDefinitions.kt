package features.oneOneOneOnline.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.oneOneOneOnline.steps.CheckMySymptoms
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.NavBarNative

open class OneOneOneStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navBar: NavigationSteps
    @Steps
    lateinit var checkMySymptoms: CheckMySymptoms

    @When("^I Check My Symptoms$")
    open fun iCheckMySymptoms() {
        login.loginPage.symptomsButton.click()
    }

    @And("^Symptoms is unselected")
    fun symptomsIsUnselected() {
        Assert.assertFalse(navBar.hasSelectedTab(NavBarNative.NavBarType.SYMPTOMS))
    }

    @And("^the Check My Symptoms page is displayed")
    fun checkMySymptomsPageIsDisplayed() {
        checkMySymptoms.assertConditionsHeaderVisible()
        checkMySymptoms.assertNhs111HeaderVisible()
    }

    @Then("^the Check My Symptoms page header and navigation menu are correct$")
    fun checkMySymptomsPageHeaderAndNavigationMenuAreCorrect() {
        navBar.headerNative.assertIsVisible("Check my symptoms")
        navBar.assertSelectedTab(NavBarNative.NavBarType.SYMPTOMS)
    }

    @When("I press the A-Z symptoms header")
    fun iPressTheAtoZHelpHeader() {
        checkMySymptoms.clickConditionsHeader()
    }

    @When("I press the urgent help header")
    fun iPressUrgentHelpHeader() {
        checkMySymptoms.clickNHS111Header()
    }

}
