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
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.NavBarNative
import webdrivers.browserstack.BrowserstackLocalService
import java.net.URL

private const val WAIT_IN_SECONDS_MODIFIER = 1000L
private const val WAIT_IN_SECONDS = 70L

open class SharedStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var navBar: NavigationSteps

    val mockingClient = MockingClient.instance

    companion object {
        lateinit var patient: Patient
    }

    @Before
    fun resetWiremock() {
        MockingClient.instance.clearWiremock()
    }

    @After
    fun stopBrowserstackIfRunning() {
        BrowserstackLocalService.stop()
    }

    @Given("a patient from (.*) is defined")
    fun systemPatient(gpSystem: String)
    {
        mockingClient.clearWiremock()
        mockingClient.favicon()
        SharedStepDefinitions.patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SharedStepDefinitions.patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        SerenityHelpers.setPatient(patient)
        setSessionVariable(GLOBAL_PROVIDER_TYPE).to(gpSystem)
    }

    @Given("(TPP|EMIS|VISION) is initialised")
    fun system(system: String) {
        SharedStepDefinitions.patient = Patient.getDefault(system)
        SerenityHelpers.setPatient(SharedStepDefinitions.patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(SharedStepDefinitions.patient)
        SessionCreateJourneyFactory.getForSupplier(system, mockingClient).createFor(SharedStepDefinitions.patient)
        setSessionVariable(GLOBAL_PROVIDER_TYPE).to(system)
    }

    @Given("^I am logged in$")
    open fun iAmLoggedIn() {
        SharedStepDefinitions.patient = SerenityHelpers.getPatientOrNull() ?: SharedStepDefinitions.patient
        browser.goToApp()
        login.using(SharedStepDefinitions.patient)
        browser.loginPage.waitForNativeStepToComplete()
    }

    @Given("^I am logged in and have not accepted the terms and conditions$")
    open fun iAmLoggedInAndHaveNotAcceptedTermsAndConditions() {
        SharedStepDefinitions.patient =
                Serenity.sessionVariableCalled<Patient>(Patient::class) ?: SharedStepDefinitions.patient
        browser.goToApp()
        login.using(SharedStepDefinitions.patient)
    }

    @Given("^I am not logged in$")
    open fun iAmNotLoggedIn() {
        browser.goToApp()
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(EmisMockDefaults.patientEmis)
    }

    @When("^I navigate to (.*)$")
    open fun iNavigateTo(tab: String) {
        navBar.select(NavBarNative.NavBarType.valueOf(tab.toUpperCase()))
    }

    @When("^I wait (\\d*) seconds$")
    open fun iWait(waitInSeconds: Int) {
        Thread.sleep(waitInSeconds * WAIT_IN_SECONDS_MODIFIER)
    }

    @Then("^I see the (.*) menu button")
    fun iSeeAMenuButton(type: String) {
        Assert.assertTrue(navBar.hasVisible(type))
    }

    @And("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navBar.hasSelectedTab(NavBarNative.NavBarType.valueOf(type.toUpperCase())))
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
        Thread.sleep(((secondsToWaitFor) * WAIT_IN_SECONDS_MODIFIER))
    }

    @Given("My session has expired")
    fun givenMySessionHasExpired() {
        Serenity.setSessionVariable("SESSION_EXPIRY_MINUTES").to(1)
        iWaitForXSeconds(WAIT_IN_SECONDS)
    }
}
