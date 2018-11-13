package features.authentication.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import net.thucydides.core.annotations.Steps

private const val SESSION_EXPIRY_IN_MILLIS_SECONDS = 80_000L

class SessionExpiryStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var login: LoginSteps

    @When("^I am idle long enough for the session to expire$")
    fun iAmIdleLongEnoughForTheSessionToExpire() {
        Thread.sleep(SESSION_EXPIRY_IN_MILLIS_SECONDS)
    }

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }
}
