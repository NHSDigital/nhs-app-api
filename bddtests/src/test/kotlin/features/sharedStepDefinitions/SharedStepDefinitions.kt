package features.sharedStepDefinitions

import cucumber.api.java.After
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.navigation.NavBarNative
import utils.SerenityHelpers
import webdrivers.browserstack.BrowserstackLocalService
import webdrivers.options.OptionManager
import webdrivers.options.nojs.NoJsOption

private const val WAIT_IN_SECONDS_MODIFIER = 1000L
private const val WAIT_IN_SECONDS = 190L

open class SharedStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navBar: NavigationSteps

    val mockingClient = MockingClient.instance

    companion object {
        lateinit var patient: Patient
    }

    @After
    fun stopBrowserstackIfRunning() {
        BrowserstackLocalService.stop()
    }

    @Given("^I am a (.*) patient$")
    fun initialisePatientAndGpSystem(gpSystem: String)
    {
        mockingClient.clearWiremock()
        mockingClient.favicon()

        patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
    }

    @Given("^I am logged in$")
    fun iAmLoggedIn() {
        SharedStepDefinitions.patient = SerenityHelpers.getPatientOrNull() ?: SharedStepDefinitions.patient
        browser.goToApp()
        login.using(patient)
        home.waitForLoginToCompleteSuccessfully()
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
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(EmisMockDefaults.patientEmis)
    }

    @Given("My session has expired")
    fun givenMySessionHasExpired() {
        Serenity.setSessionVariable("SESSION_EXPIRY_MINUTES").to(1)
        iWaitForXSeconds(WAIT_IN_SECONDS)
    }

    @When("^I navigate to (\\w+)$")
    open fun iNavigateTo(tab: String) {
        navBar.select(NavBarNative.NavBarType.valueOf(tab.toUpperCase()))
    }

    @Then("^I see the (.*) menu button on mobile")
    fun iSeeAMenuButtonOnMobile(type: String) {
        if(home.headerNative.onMobile()) {
            Assert.assertTrue(navBar.hasVisible(type))
        }
    }

    @Then("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navBar.hasSelectedTab(NavBarNative.NavBarType.valueOf(type.toUpperCase())))
    }

    @Then("^none of the menu buttons are highlighted")
    fun iDoNotSeeAHighlightedMenuButton() {
        if(home.headerNative.onMobile()) {
            Assert.assertFalse("Nav bar has highlighted item, expected none", navBar.hasAnyTabSelected())
        }
    }

    @Then("^I am redirected to '(.*)'$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }

    @Then("I wait for (\\d+) seconds")
    fun iWaitForXSeconds(secondsToWaitFor: Long) {
        Thread.sleep(((secondsToWaitFor) * WAIT_IN_SECONDS_MODIFIER))
    }


}
