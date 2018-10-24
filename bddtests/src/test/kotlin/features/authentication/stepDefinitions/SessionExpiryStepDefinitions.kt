package features.authentication.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import net.thucydides.core.annotations.Steps

class SessionExpiryStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var login: LoginSteps

    private val sessionExpiryInMilliSecs: Long = 70_000


    @When("^I am idle long enough for the session to expire$")
    fun iAmIdleLongEnoughForTheSessionToExpire() {
        Thread.sleep(sessionExpiryInMilliSecs)
    }

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }
}