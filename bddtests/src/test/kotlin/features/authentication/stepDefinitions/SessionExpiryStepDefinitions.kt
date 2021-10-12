package features.authentication.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import pages.ErrorPage
import pages.SessionExpiry
import utils.GlobalSerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.session.UserSessionRequest

private const val DELAY_BEFORE_RESUME = 10_000L

class SessionExpiryStepDefinitions  {

    @Steps
    lateinit var login: LoginSteps

    lateinit var sessionExpiry: SessionExpiry

    private lateinit var errorPage: ErrorPage

    @Given("^I am logged in as a (.*) user expecting a \"(.*)\"\\ response when extending their session$")
    fun iClickToExtendSessionExpectingResponse(gpSystem: String, expectedResponse: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier).copy(linkedAccounts = setOf())
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        val ssoRedirectUri = GlobalSerenityHelpers.GP_SESSION_REDIRECT_URI.getOrFail<String>()

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = redirectUri))

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = ssoRedirectUri))

        val patientConfig = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getPatientLinkedAccountsConfiguration()

        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(patientConfig?.id)

        PatientVerificationFactory.getForSupplier(supplier)
                .setSessionExtendMockResponse(patient, expectedResponse)
    }

    @When("^I try to extend my session$")
    fun iTryToExtendMySession() {
        val response = Serenity
                .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .session.postSessionConnection(LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail())
        SerenityHelpers.setHttpResponse(response)
    }

    @When("^I am idle long enough for the session to expire$")
    fun iAmIdleLongEnoughForTheSessionToExpire() {
        sessionExpiry.waitForSessionExpiryAfterModalDisplay()
    }

    @Given("^I allow my session to expire$")
    fun givenIAllowMySessionToExpire() {
        sessionExpiry.waitForSessionExpiry()
    }

    @When("^I click to extend the session$")
    fun iClickToExtendSession() {
        val patient = Patient.getDefault(Supplier.EMIS)
        PatientVerificationFactory.getForSupplier(Supplier.EMIS)
                .setSessionExtendMockResponse(patient, "Success")
        sessionExpiry.clickExtendSession()
    }

    @When("^I click to log out$")
    fun iClickToLogOut() = sessionExpiry.clickLogOut()


    @When("^I am idle long enough for the session expiry dialog box to appear$")
    fun iAmIdleLongEnoughForSessionExpiryDialog() = sessionExpiry.waitForSessionExpiryModal()

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }

    @Then("^I see a dialog box prompting to extend the session$")
    fun iSeeTheDialogBoxPromptingToExtendTheSession() {
        sessionExpiry.assertIsDisplayed()
    }

    @When("^I click to extend the session that returns bad gateway$")
    fun iClickToExtendSessionThatReturnsBadGateway() {
        val patient = Patient.getDefault(Supplier.EMIS)
        PatientVerificationFactory.getForSupplier(Supplier.EMIS)
            .setSessionExtendMockResponse(patient, "bad gateway")
        sessionExpiry.clickExtendSession()
    }

    @Then("^the dialog box is not visible on the screen$")
    fun theDialogBoxIsNotVisible() {
        sessionExpiry.assertIsNotDisplayed()
    }

    @Then("^I am idle for a short time$")
    fun iamIdleBeforeResume() {
        Thread.sleep(DELAY_BEFORE_RESUME)
    }

    @Then("^I do not see the error page$")
    fun iDoNotSeeTheErrorPage() {
        errorPage.assertIsNotDisplayed()
    }
}
