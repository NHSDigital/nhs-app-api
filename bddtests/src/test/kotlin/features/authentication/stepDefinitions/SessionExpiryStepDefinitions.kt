package features.authentication.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import pages.SessionExpiry
import utils.GlobalSerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.WorkerClient
import worker.models.session.UserSessionRequest

private const val DELAY_SECONDS_FOR_WAITING = 2000L
private const val DELAY_BEFORE_RESUME = 10_000L

class SessionExpiryStepDefinitions  {

    @Steps
    lateinit var login: LoginSteps

    lateinit var sessionExpiry: SessionExpiry

    @Given("^I am logged in as a (.*) user expecting a \"(.*)\"\\ response when extending their session$")
    fun iClickToExtendSessionExpectingResponse(gpSystem: String, expectedResponse: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier).copy(linkedAccounts = setOf())
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = redirectUri))

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
        when(sessionExpiry.onMobile()){
            false -> {
                sessionExpiry.waitForSessionExpiryAfterModalDisplay()
            }
            true -> {
                sessionExpiry.waitForSessionExpiry()
            }
        }
    }

    @When("^I am idle long enough for the session to expire after the dialog$")
    fun iAmIdleLongEnoughForTheSessionToExpireAfterTheDialog() = sessionExpiry.waitForSessionExpiryAfterModalDisplay()

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

    @When("^I am idle long enough on a secure page for the session expiry dialog box to appear$")
    fun iAmIdleLongEnoughOnASecurePageForSessionExpiryDialog() {
        sessionExpiry.waitForSessionExpiryModal()
        sessionExpiry.assertIsDisplayed()
    }

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }

    @Then("^I see a dialog box prompting to extend the session$")
    fun iSeeTheDialogBoxPromptingToExtendTheSession() {
        sessionExpiry.assertIsDisplayed()
    }

    @Then("^the dialog box is not visible on the screen$")
    fun theDialogBoxIsNotVisible() {
        sessionExpiry.assertIsNotDisplayed()
    }

    @Then("^I background the app long enough for the session warning dialog to appear and bring it back to foreground$")
    fun iBackgroundTheAppForDialog() = sessionExpiry.backgroundAppUntilSessionExpiryModalShouldBeDisplayed()

    @Then("^I background the app long enough for the session expiry and bring it back to foreground$")
    fun iBackgroundTheAppForSessionExpiry() = sessionExpiry.backgroundAppUntilSessionExpiry()

    @Then("^I lock the device$")
    fun iLockTheDevice() = sessionExpiry.lockDevice()

    @Then("^I unlock the device$")
    fun iUnlockTheDevice() {
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
        sessionExpiry.unlockDevice()
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
    }

    @Then("^I am idle for a short time$")
    fun iamIdleBeforeResume() {
        Thread.sleep(DELAY_BEFORE_RESUME)
        sessionExpiry.tryUnlockDevice()
    }
}
