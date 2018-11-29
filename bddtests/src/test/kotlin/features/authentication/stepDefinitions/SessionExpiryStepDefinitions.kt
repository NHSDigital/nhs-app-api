package features.authentication.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient

private const val DELAY_FOR_DIALOG = 70_000L

class SessionExpiryStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var login: LoginSteps

    @Given("^a (.*) user expecting a \"(.*)\"\\ response when extending their session$")
    fun iClickToExtendSessionExpectingResponse(gpSystem: String, expectedResponse: String) {

        PatientVerificationFactory.getForSupplier(gpSystem).setSessionExtendMockResponse(expectedResponse)

    }

    @When("^I try to extend my session$")
    fun iTryToExtendMySession() {
        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .session.postSessionConnection()
            SerenityHelpers.setHttpResponse(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I am idle long enough for the session to expire")
    fun iAmIdleLongEnoughForSessionExpiryDialogBackEnd() {
        Thread.sleep(DELAY_FOR_DIALOG)
    }

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }
}
