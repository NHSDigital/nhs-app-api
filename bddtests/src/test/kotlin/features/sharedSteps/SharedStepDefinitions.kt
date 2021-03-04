package features.sharedSteps

import config.Config
import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.stepDefinitions.AuthenticationFactory
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myrecord.factories.DemographicsFactory
import features.pushNotifications.stepDefinitions.NotificationsFactory
import features.pushNotifications.stepDefinitions.SettingStatus
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.termsAndConditions.TermsAndConditionsJourneyFactory
import models.Patient
import models.patients.PatientHandler
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertIsVisible
import pages.navigation.NavBarNative
import pages.navigation.WebHeader
import pages.withNormalisedText
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers

import utils.set
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

private const val WAIT_IN_SECONDS_MODIFIER = 1000L
private const val WAIT_IN_SECONDS = 190L
private const val FOUR_SECOND_SLEEP: Long = 4000
open class SharedStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var cookieSteps: CookieSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navSteps: NavigationSteps

    lateinit var webHeader: WebHeader
    private val mockingClient = MockingClient.instance

    @Given("^I am an? (.*) patient$")
    fun initialisePatientAndGpSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        setupPatient(patient, supplier)

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am an? (.*) patient whose GP system is unavailable$")
    fun initialisePatientAndUnavailableGpSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        setupPatient(patient, supplier)

        AuthenticationFactory.getForSupplier(supplier)
                .validOAuthDetailsAndGpSystemUnavailable(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am an? (.*) patient with linked profiles whose GP system is unavailable$")
    fun initialisePatientWithLinkedProfilesAndUnavailableGpSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = PatientHandler.getForSupplier(supplier).getPatientWithLinkedProfiles()
        setupPatient(patient, supplier)

        AuthenticationFactory.getForSupplier(supplier).validOAuthDetailsAndGpSystemUnavailable(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @When("^The (.*) GP system becomes available$")
    fun makeGpSystemAvailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val patient = SerenityHelpers.getPatient()

        SessionCreateJourneyFactory.getForSupplier(supplier)
            .createFor(patient)
    }

    @When("^The (.*) GP system is still unavailable$")
    fun makeGpSystemStillUnavailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)

        // this needs to generate a different service desk reference from initialisePatientAndUnavailableGpSystem
        // to allow asserting the login error code is not shown in P9 journey error screens
        AuthenticationFactory.getForSupplier(supplier)
            .validOAuthDetailsAndGpSystemReturnsError()
    }

    @Given("^I am an? (.*) patient using the native app$")
    fun initialisePatientAndGpSystemOnNativeApp(gpSystem: String) {
        browser.setUserAgentSource("ios")
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidNativeRedirectUri)

        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        setupPatient(patient, supplier)

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am a patient using the native app$")
    fun patientOnNativeApp() {
        browser.setUserAgentSource("ios")
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidNativeRedirectUri)

        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        setupPatient(patient, supplier)

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am logged in expecting to see T&Cs$")
    fun iAmLoggedInFirstTime() {
        doLogin(false)
    }

    @Given("^I am logging in$")
    fun iAmLoggingIn() {
        doLogin(false)
    }

    @Given("^I log in to the app expecting to see the notifications prompt$")
    fun iLogInToTheAppExpectingTheNotificationsPrompt() {
        val patient = handleLogin()
        login.usingLoginWithNotificationOptions(
                patient)
    }

    @Given("^I am logged in$")
    fun iAmLoggedIn() {
        doLogin(true)
    }

    @Given("^I am logged in with notifications denied$")
    fun iAmLoggedInWithNotificationsDenied() {
        val patient = handleLogin()
        login.using(patient)
        login.skipNotificationPromptCookie()

        home.waitForLoginToCompleteSuccessfully(true)

        initialSetup(SettingStatus.Denied, false)

        browser.executeScripts()
    }

    @Given("^I am logged in skipping the notifications prompt$")
    fun iAmLoggedInSkippingNotificationsPrompt() {
        val patient = handleLogin()
        login.using(patient)
        login.skipNotificationPromptCookie()

        home.waitForLoginToCompleteSuccessfully(true)

        browser.executeScripts()
    }

    @Given("^I am logged in with notifications enabled, but with an existing incorrect user device$")
    fun iAmLoggedInWithNotificationsEnabledButIssuesOccurWhenLoggedIn() {
        val patient = handleLogin()
        login.using(patient)
        login.skipNotificationPromptCookie()

        home.waitForLoginToCompleteSuccessfully(true)

        val factory = initialSetup(SettingStatus.Authorised, true)
        factory.setUpInvalidMongoDeviceRegistration()

        browser.executeScripts()
    }

    @When("^I login$")
    fun iLogin() {
        val patient = SerenityHelpers.getPatient()
        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enableForPatientProxyAccounts(patient)

        login.using(patient)
    }

    @Given("^Azure organisation search is working$")
    fun azureOrganisationSearchIsWorking() {
        mockingClient.forAzure.forSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(null)
                    .respondWithSuccess(NhsAzureSearchData.getOrganisationWithinRange())
        }
    }

    @Given("^I have (enabled|disabled) javascript$")
    fun iHaveEnabledDisabledJavascript(status: String) {
        when(status) {
            "disabled" -> OptionManager.instance().registerOption(NoJsOption())
            "enable" -> {}
        }
    }

    @Given("^I am not logged in$")
    open fun iAmNotLoggedIn() {
        browser.goToApp()
        CitizenIdSessionCreateJourney().createFor(EmisMockDefaults.patientEmis)
        EmisSessionCreateJourneyFactory().createFor(EmisMockDefaults.patientEmis)
    }

    @Given("^My session has expired$")
    fun givenMySessionHasExpired() {
        Serenity.setSessionVariable("SESSION_EXPIRY_MINUTES").to(1)
        iWaitForXSeconds(WAIT_IN_SECONDS)
    }

    @When("^I navigate to (\\w+)$")
    open fun iNavigateTo(tab: String) {
        navSteps.select(NavBarNative.NavBarType.valueOf(tab.toUpperCase()))
    }

    @Then("^Wait for the request to complete$")
    fun waitForRequestToComplete() {
        Thread.sleep(FOUR_SECOND_SLEEP)
    }

    @Then("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navSteps.hasSelectedTab(NavBarNative.NavBarType.valueOf(type.toUpperCase())))
    }

    @Then("^none of the menu buttons are highlighted")
    fun iDoNotSeeAHighlightedMenuButton() {
        if(home.headerNative.onMobile()) {
            Assert.assertFalse("Nav bar has highlighted item, expected none", navSteps.hasAnyTabSelected())
        }
    }

    @Then("^I am redirected to '(.*)'$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }

    @Then("^I wait for (\\d+) seconds$")
    fun iWaitForXSeconds(secondsToWaitFor: Long) {
        Thread.sleep(((secondsToWaitFor) * WAIT_IN_SECONDS_MODIFIER))
    }

    @Then("^the page title is '(.*)'$")
    fun thePageTitleIsYourAppointments(title: String) {
        webHeader.getPageTitle().withNormalisedText(title).assertIsVisible()
    }

    @Then("^the page contains the header '(.*)'$")
    fun thePageContainsTheHeaderText(title: String) {
        webHeader.getHtmlElement("h2").withNormalisedText(title).assertIsVisible()
    }

    private fun initialSetup(status: SettingStatus, authorised: Boolean): NotificationsFactory {
        val factory = NotificationsFactory()
        val patient = factory.setUpUser()
        factory.setUpDeviceValues(patient.accessToken)
        factory.mockNativeNotificationFunctions(status, authorised)

        return factory
    }

    private fun doLogin(waitForLoginPage: Boolean) {
        val patient = handleLogin()

        login.using(patient)

        home.waitForLoginToCompleteSuccessfully(waitForLoginPage)
    }

    private fun handleLogin(): Patient {
        val patient = SerenityHelpers.getPatient()
        browser.goToApp()

        DemographicsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enableForPatientProxyAccounts(patient)

        TermsAndConditionsJourneyFactory.consent(patient)

        cookieSteps.setInstructionsCookie("true")

        return patient
    }

    private fun setupPatient(patient: Patient, supplier: Supplier) {
        mockingClient.favicon()

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
