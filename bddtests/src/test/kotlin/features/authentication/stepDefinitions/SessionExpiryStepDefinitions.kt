package features.authentication.stepDefinitions
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.factories.PatientVerificationFactory
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.CheckMySymptomsPage
import pages.SessionExpiryNative
import utils.GlobalSerenityHelpers
import utils.LinkedProfilesSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.session.UserSessionRequest

private const val DELAY_SECONDS_FOR_WAITING = 2000L
private const val DELAY_BEFORE_RESUME = 10_000L

class SessionExpiryStepDefinitions  {

    private val mockingClient = MockingClient.instance

    @Steps
    lateinit var login: LoginSteps

    lateinit var sessionExpiry: SessionExpiryNative

    lateinit var patient: Patient

    lateinit var checkMySymptoms: CheckMySymptomsPage

    @Given("^I am logged in as a (.*) user expecting a \"(.*)\"\\ response when extending their session$")
    fun iClickToExtendSessionExpectingResponse(gpSystem: String, expectedResponse: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier).copy()
        val redirectUri = GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail<String>()
        patient.linkedAccounts = setOf()

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier, mockingClient).createFor(patient)
        DemographicsFactory.getForSupplier(supplier).enabled(patient)

        Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(UserSessionRequest(
                        authCode = patient.authCode,
                        codeVerifier = patient.codeVerifier,
                        redirectUrl = redirectUri))

        val patientConfig = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .getPatientLinkedAccountsConfiguration()

        LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.set(patientConfig.id)

        PatientVerificationFactory.getForSupplier(supplier)
                .setSessionExtendMockResponse(patient, expectedResponse)
    }

    @When("^I click Use NHS 111 online$")
    fun iClickCheckIfYouNeedUrgentHelp() = checkMySymptoms.clickNHS111Header()

    @When("^I click Get advice about coronavirus$")
    fun iClickGetAdviceAboutCoronaVirus() {
        checkMySymptoms.clickCoronaVirusHeader()
    }

    @When("^I try to extend my session$")
    fun iTryToExtendMySession() {
        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .session.postSessionConnection(LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail())
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
            }
            true -> {
                sessionExpiry.waitForSessionExpiry()
            }
        }
    }

    @When("^I am idle long enough for the session to expire after the dialog$")
    fun iAmIdleLongEnoughForTheSessionToExpireAfterTheDialog() = sessionExpiry.waitForSessionExpiryAfterModalDisplay()

    @Given("I allow my session to expire")
    @When("I am idle long enough for the desktop session to expire")
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

        Assert.assertTrue("Session expiry modal visible", presentOnPage)
    }

    @Then("^the dialog box is not visible on the screen$")
    fun theDialogBoxIsNotVisibile() {
        val presentOnPage = sessionExpiry.isSessionExpiryModalVisible()
        Assert.assertFalse("Session expiry modal visible", presentOnPage)
    }

    @Then("^I background the app long enough for the session warning dialog to appear and bring it back to foreground$")
    fun iBackgroundTheAppForDialog() = sessionExpiry.backgroundAppUntilSessionExpiryModalShouldBeDisplayed()

    @Then("^I background the app long enough for the session expiry and bring it back to foreground$")
    fun iBackgroundTheAppForSessionExpiry() = sessionExpiry.backgroundAppUntilSessionExpiry()

    @Then("^I lock the device$")
    fun iLockTheDevice() = sessionExpiry.lockAndroidDevice()

    @Then("^I unlock the device$")
    fun iUnlockTheDevice() {
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
        sessionExpiry.unlockAndroidDevice()
        Thread.sleep(DELAY_SECONDS_FOR_WAITING)
    }

    @Then("^I scroll the device$")
    fun iScrollTheDevice() = sessionExpiry.scrollAndroidNativePage()

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
