package features.authentication.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import net.serenitybdd.core.PendingStepException
import net.thucydides.core.annotations.Steps
import org.junit.Assert


class AuthenticationStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps

    @And("^I have a slow connection$")
    fun hasASlowConnection() {
        // TODOs
    }

    @When("^I log in")
    fun logIn() {
        login.asDefault()
    }

    @Then("^I see the home page")
    fun iSeeTheHomePage() {
        home.assertPageIsVisible()
    }

    @Then("^I see a welcome message for (.*)$")
    fun iSeeAWelcomeMessageFor(name: String) {
        home.assertWelcomeMessageShownFor(name)
    }

    @And("^I see the header$")
    fun iSeeHeader() {
        home.assertHeaderVisible()
    }

    @And("^I see the navigation menu$")
    fun iSeeNavbar() {
        nav.assertVisible()
    }
}