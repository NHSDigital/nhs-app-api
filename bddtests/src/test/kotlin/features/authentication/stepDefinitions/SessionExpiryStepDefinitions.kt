package features.authentication.stepDefinitions
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.authentication.steps.LoginSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.SessionExpiryNative
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient

private const val SESSION_EXPIRY_IN_MILLIS_SECONDS = 120L
private const val DELAY_SECONDS_FOR_WAITING = 2000L
private const val DELAY_BEFORE_RESUME = 10_000L
private const val BACKGROUND_DURATION = 100L
private const val DELAY_FOR_DIALOG = 120_000L
private const val DESKTOP_EXPIRY_INTERVAL = 180_000L

class SessionExpiryStepDefinitions  {

    private val mockingClient = MockingClient.instance

    @Steps
    lateinit var login: LoginSteps

    lateinit var sessionExpiry: SessionExpiryNative

    lateinit var patient: Patient

    @Given("^I am logged in as a (.*) user expecting a \"(.*)\"\\ response when extending their session$")
    fun iClickToExtendSessionExpectingResponse(gpSystem: String, expectedResponse: String) {
        val patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession)

        PatientVerificationFactory.getForSupplier(gpSystem)
                .setSessionExtendMockResponse(patient, expectedResponse)
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

    @When("^I am idle long enough for the session to expire$")
    fun iAmIdleLongEnoughForTheSessionToExpire() {
        when(sessionExpiry.onMobile()){
            false -> {
                sessionExpiry.waitForSessionExpiryAfterModalDisplay()
                Thread.sleep(DELAY_BEFORE_RESUME)
            }
            true -> {
                sessionExpiry.scrollAndroidNativePage()
                sessionExpiry.waitForSessionExpiryModal()
                sessionExpiry.scrollAndroidNativePage()
                sessionExpiry.waitForSessionExpiryAfterModalDisplay()
                sessionExpiry.scrollAndroidNativePage()
            }
        }
    }

    @When("^I am idle long enough for the session to expire after the dialog$")
    fun iAmIdleLongEnoughForTheSessionToExpireAfterTheDialog() {
        sessionExpiry.waitForSessionExpiryAfterModalDisplay()
    }

    @When("^I am idle long enough for the backend session to expire")
    fun iAmIdleLongEnoughForSessionExpiryDialogBackEnd() {
        Thread.sleep(DELAY_FOR_DIALOG)
    }

    @When("I am idle long enough for the desktop session to expire")
    fun iAmIdleLongEnoughForTheDesktopSessionToExpire() {
        Thread.sleep(DESKTOP_EXPIRY_INTERVAL)
    }

    @When("^I click to extend the session$")
    fun iClickToExtendSession() {
        val patient = Patient.getDefault("EMIS")
        PatientVerificationFactory.getForSupplier("EMIS")
                .setSessionExtendMockResponse(patient, "Success")
        sessionExpiry.clickExtendSession()
    }

    @When("^I click to log out$")
    fun iClickToLogOut() {
        sessionExpiry.clickLogOut()
    }


    @When("^I am idle long enough for the session expiry dialog box to appear$")
    fun iAmIdleLongEnoughForSessionExpiryDialog() {
        sessionExpiry.waitForSessionExpiryModal()
    }

    @When("^I am idle long enough on a secure page for the session expiry dialog box to appear$")
    fun iAmIdleLongEnoughOnASecurePageForSessionExpiryDialog() {
        sessionExpiry.waitForSessionExpiryModal()
        Thread.sleep(DELAY_BEFORE_RESUME)
        val presentOnPage =  sessionExpiry.isSessionExpiryModalVisible()
        Assert.assertEquals(true, presentOnPage)
        if(sessionExpiry.onMobile()) {
            sessionExpiry.scrollAndroidNativePage()
        }
    }

    @Then("^I see the login page with the session expiry notification$")
    fun iSeeTheLoginPageWithTheSessionExpiryNotification() {
        login.loginPage.shouldBeDisplayed()
        login.loginPage.assertTimeoutBannerIsVisible()
    }

    @Then("^I see a dialog box prompting to extend the session$")
    fun iSeeTheDialogBoxPromptingToExtendTheSession() {
        val presentOnPage =  sessionExpiry.isSessionExpiryModalVisible()

        Assert.assertEquals(true, presentOnPage)
    }

    @Then("^the dialog box is not visible on the screen$")
    fun theDialogBoxIsNotVisibile() {
        val presentOnPage = sessionExpiry.isSessionExpiryModalVisible()
        Assert.assertEquals(false, presentOnPage)
    }

    @Then("^I background the app long enough for the session warning dialog to appear and bring it back to foreground$")
    fun iBackgroundTheAppForDialog() {
        sessionExpiry.backgroundAndroidAppforDurationBeforeReturning(BACKGROUND_DURATION)
    }

    @Then("^I background the app long enough for the session expiry and bring it back to foreground$")
    fun iBackgroundTheAppForSessionExpiry() {
        sessionExpiry.backgroundAndroidAppforDurationBeforeReturning(SESSION_EXPIRY_IN_MILLIS_SECONDS)
    }

    @Then("^I lock the device$")
    fun iLockTheDevice() {
        sessionExpiry.lockAndroidDevice()
    }

    @Then("^I unlock the device$")
    fun iUnlockTheDevice() {
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
        sessionExpiry.unlockAndroidDevice()
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
    }

    @Then("^I scroll the device$")
    fun iScrollTheDevice() {
        sessionExpiry.scrollAndroidNativePage()
    }

    @Then("^I am idle for a short time$")
    fun iamIdleBeforeResume() {
        when(sessionExpiry.onMobile()) {
            true -> {
                sessionExpiry.scrollAndroidNativePage()
                Thread.sleep(DELAY_BEFORE_RESUME)
                sessionExpiry.scrollAndroidNativePage()
            }
            false -> Thread.sleep(DELAY_BEFORE_RESUME)
        }
    }
}
