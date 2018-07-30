package features.oneOneOneOnline.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.oneOneOneOnline.Steps.CheckMySymptoms
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert

open class OneOneOneStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navBar: NavigationSteps
    @Steps
    lateinit var checkMySymptoms: CheckMySymptoms

    @When("^I Check My Symptoms$")
    open fun iCheckMySymptoms() {
        login.checkMySymptoms()
    }

    @And("^Symptoms is unselected")
    fun symptomsIsUnselected() {
        Assert.assertFalse(navBar.hasSelectedTab("Symptoms"))
    }

    @And("^Check My symptoms page is displayed Logged Out")
    fun checkMySymptomsPageIsDisplayedLoggedOut() {
        Assert.assertTrue(checkMySymptoms.isPageDisplayedLoggedOut())
    }

    @And("^Check My symptoms page is displayed Logged In")
    fun checkMySymptomsPageIsDisplayedLoggedIn() {
        Assert.assertTrue(checkMySymptoms.isPageDisplayedLoggedIn())
    }
}