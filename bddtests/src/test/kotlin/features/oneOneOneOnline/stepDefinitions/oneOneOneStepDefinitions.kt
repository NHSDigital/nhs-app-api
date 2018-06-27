package features.oneOneOneOnline.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert

open class OneOneOneStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navBar: NavigationSteps

    @When("^I Check My Symptoms$")
    open fun iCheckMySymptoms() {
        login.checkMySymptoms()
    }

    @And("^Symptoms is unselected")
    fun symptomsIsUnselected() {
        Assert.assertFalse(navBar.hasSelectedTab("Symptoms"))
    }
}