package features.sharedSteps

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
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.serviceJourneyRules.mappers.SJRJourneyTypesMapper
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journeys.termsAndConditions.TermsAndConditionsJourneyFactory
import models.IdentityProofingLevel
import models.Patient
import models.patients.PatientHandler
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import pages.navigation.WebHeader
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

private const val WAIT_IN_SECONDS_MODIFIER = 1000L
private const val WAIT_IN_SECONDS = 190L
private const val FOUR_SECOND_SLEEP: Long = 4000
private const val P5_PROOF_LEVEL = 5

open class SharedStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps

    @Steps
    lateinit var cookieSteps: CookieSteps

    @Steps
    lateinit var home: HomeSteps

    @Steps
    lateinit var login: LoginSteps

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
        setupPatientWithUnavailableGpSession(patient, supplier)
    }

    @Given("^GP session is unavailable$")
    fun initialisePatientWithUnavailableGpSystem() {
        val patient = SerenityHelpers.getPatient()
        val supplier = SerenityHelpers.getGpSupplier()
        setupPatientWithUnavailableGpSession(patient, supplier)
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

        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        setupPatient(patient, supplier)

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        TermsAndConditionsJourneyFactory.consent(patient)
    }

    @Given("^I am a user with (.*) (enabled|disabled)$")
    fun iAmAUserWithJourney(journey: String, status: String) =
        initialisePatientWithJourney(IdentityProofingLevel.P9, journey, status)

    @Given("^I am a proof level ([5,9]{1}) user with (.*) (enabled|disabled)$")
    fun iAmAProofLevelUserWithJourney(proofLevel: Int, journey: String, status: String) =
        initialisePatientWithJourney(proofLevel, journey, status)

    @Given("^I am a patient using the native app$")
    fun patientOnNativeApp() {
        browser.setUserAgentSource("ios")
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(true)

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
        login.usingLoginWithNotificationOptions(patient)
    }

    @Given("^I log in without notification cookie$")
    fun iLogInWithoutNotificationCookie() {
        val patient = handleLogin()
        login.using(patient, false)
    }

    @Given("^I am logged in$")
    fun iAmLoggedIn() {
        doLogin(true)
    }

    @Given("^I am logged in with notifications denied$")
    fun iAmLoggedInWithNotificationsDenied() {
        val patient = handleLogin()
        login.using(patient)

        home.waitForLoginToCompleteSuccessfully(true)

        initialSetup(SettingStatus.Denied, false)

        browser.executeScripts()
    }

    @Given("^I am logged in skipping the notifications prompt$")
    fun iAmLoggedInSkippingNotificationsPrompt() {
        val patient = handleLogin()
        login.using(patient)

        home.waitForLoginToCompleteSuccessfully(true)

        browser.executeScripts()
    }

    @Given("^I am logged in with notifications enabled, but with an existing incorrect user device$")
    fun iAmLoggedInWithNotificationsEnabledButIssuesOccurWhenLoggedIn() {
        val patient = handleLogin()
        login.using(patient)

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
        when (status) {
            "disabled" -> OptionManager.instance().registerOption(NoJsOption())
            "enable" -> {
            }
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

    @Then("^Wait for the request to complete$")
    fun waitForRequestToComplete() {
        Thread.sleep(FOUR_SECOND_SLEEP)
    }

    @Then("^I wait for (\\d+) seconds$")
    fun iWaitForXSeconds(secondsToWaitFor: Long) {
        Thread.sleep(((secondsToWaitFor) * WAIT_IN_SECONDS_MODIFIER))
    }

    private fun initialisePatientWithJourney(
        proofLevel: Int,
        journey: String,
        status: String
    ) = initialisePatientWithJourney(
        if (proofLevel == P5_PROOF_LEVEL) IdentityProofingLevel.P5 else IdentityProofingLevel.P9,
        journey,
        status
    )

    private fun initialisePatientWithJourney(
        proofLevel: IdentityProofingLevel,
        journey: String,
        status: String
    ) {
        val disabled = when (status) {
            "disabled" -> true
            "enable" -> false
            else -> false
        }

        initialisePatientWithJourney(proofLevel, journey, disabled)
    }

    private fun initialisePatientWithJourney(
        proofLevel: IdentityProofingLevel,
        journey: String,
        disabled: Boolean
    ) {
        val journeyTypes = SJRJourneyTypesMapper.map(journey, disabled)
        var patient = SerenityHelpers.getPatientOrNull()
        val supplier: Supplier?

        if (patient != null) {
            supplier = SerenityHelpers.getGpSupplier()
            val odsCode = ServiceJourneyRulesMapper.findOdsCode(supplier, journeyTypes)

            patient.updateOdsCodes(odsCode)
            patient = patient.copy(identityProofingLevel = proofLevel)
            SerenityHelpers.setPatient(patient)
        } else {
            patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, journeyTypes, proofLevel)
            supplier = SerenityHelpers.getGpSupplier()
        }

        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient, alternativeUser = true)
        CitizenIdSessionCreateJourney().createFor(patient, alternativeUser = true)
    }

    private fun initialSetup(status: SettingStatus, authorised: Boolean): NotificationsFactory {
        val factory = NotificationsFactory()
        val patient = GlobalSerenityHelpers.PATIENT.getOrFail<Patient>()
        factory.setUpDeviceValues(patient.accessToken)
        factory.mockNativeNotificationFunctions(status, authorised)

        return factory
    }

    private fun doLogin(waitForLoginPage: Boolean) {
        val patient = handleLogin()

        login.using(patient)

        mockingClient.forHelp.mock {
            respondWithPage()
        }

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

    private fun setupPatientWithUnavailableGpSession(patient: Patient, supplier: Supplier) {
        setupPatient(patient, supplier)

        AuthenticationFactory.getForSupplier(supplier)
            .validOAuthDetailsAndGpSystemUnavailable(patient)

        TermsAndConditionsJourneyFactory.consent(patient)
    }

    private fun setupPatient(patient: Patient, supplier: Supplier) {
        mockingClient.favicon()

        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(supplier)

        CitizenIdSessionCreateJourney().createFor(patient)
    }
}
