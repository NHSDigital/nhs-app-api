package features.sharedStepDefinitions

import cucumber.api.java.After
import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import features.sharedSteps.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.MockDataPopulate
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.emis.models.AssociationType
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import webdrivers.browserstack.BrowserstackLocalService
import java.net.URL

open class SharedStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var navBar: NavigationSteps

    val mockingClient = MockingClient.instance

    lateinit var patient: Patient

    @Before
    fun resetWiremock() {
        MockingClient.instance.clearWiremock()
    }

    @After
    fun stopBrowserstackIfRunning() {
        BrowserstackLocalService.stop()
    }

    @Given("(.*) logged in session started$")
    @Throws(Exception::class)
    fun emis_logged_in_session_started(system: String) {
        when (system) {
            "TPP" -> initialiseTpp()
            else -> initialiseEmisLoggedSession()
        }
    }

    private fun initialiseEmisLoggedSession() {
        this.patient = MockDefaults.patient
        MockDataPopulate(mockingClient).populateForJustLoggedIn()
        mockingClient.forEmis { sessionRequest(this@SharedStepDefinitions.patient).respondWithSuccess(this@SharedStepDefinitions.patient, AssociationType.Self) }
    }

    @Given("a patient from (.*) is defined")
    fun systemPatient(gpSystem: String)
    {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        this.patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)

        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        SerenityHelpers.setPatient(patient)
        setSessionVariable(GLOBAL_PROVIDER_TYPE).to(gpSystem)
    }

    @Given("(TPP|EMIS) is initialised")
    fun system(system: String) {

        when (system) {
            "TPP" -> initialiseTpp()
            "EMIS" -> initialiseEmis()
        }

        setSessionVariable(GLOBAL_PROVIDER_TYPE).to(system)
    }

    private fun initialiseEmis() {
        this.patient = MockDefaults.patient
        MockDataPopulate(mockingClient).populate()
        mockingClient.forEmis { sessionRequest(this@SharedStepDefinitions.patient).respondWithSuccess(this@SharedStepDefinitions.patient, AssociationType.Self) }
    }

    private fun initialiseTpp() {
        this.patient = Patient.getDefault("TPP")
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)
        TppSessionCreateJourneyFactory(mockingClient).createFor(this.patient)
    }

    @Given("^I am logged in$")
    open fun iAmLoggedIn() {
        this.patient = SerenityHelpers.getPatientOrNull() ?: this.patient
        browser.goToApp()
        login.using(this.patient)
    }

    @Given("^I am not logged in$")
    open fun iAmNotLoggedIn() {
        browser.goToApp()
        CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patient)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(MockDefaults.patient)
    }

    @When("^I navigate to (.*)$")
    open fun iNavigateTo(tab: String) {
        navBar.select(tab)
    }

    @When("^I wait (\\d*) seconds$")
    open fun iWait(waitInSeconds: Int) {
        Thread.sleep(waitInSeconds * 1000L)
    }

    @Then("^I see the (.*) menu button")
    fun iSeeAMenuButton(type: String) {
        Assert.assertTrue(navBar.hasVisible(type))
    }

    @And("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navBar.hasSelectedTab(type))
    }

    @Then("^none of the menu buttons are highlighted")
    fun iDoNotSeeAHighlightedMenuButton() {
        Assert.assertFalse("Nav bar has highlighted item, expected none", navBar.hasAnyTabSelected())
    }


    @Then("^I am redirected to '(.*)'$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }

    @Then("^a new tab opens (.*)$")
    fun aNewTabOpens(url: String) {
        browser.changeTab(URL(url))
        browser.shouldHaveUrl(url)
    }

    @Then("I wait for (\\d+) seconds")
    fun iWaitForXSeconds(secondsToWaitFor: Long) {
        Thread.sleep(((secondsToWaitFor) * 1000))
    }

    @Given("My session has expired")
    fun givenMySessionHasExpired() {
        Serenity.setSessionVariable("SESSION_EXPIRY_MINUTES").to(1)
        iWaitForXSeconds(70)
    }
}
