package features.sharedStepDefinitions

import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.MockDataPopulate
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.emis.models.AssociationType
import mocking.tpp.models.AuthenticateReply
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import java.net.URL
import java.util.*

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

    @Given("(.*) is initialised")
    fun system(system: String) {

        if(system == "wiremock"){
            initialiseEmis()
        }
        if(system == "EMIS"){
            initialiseEmis()
        }
        if(system == "TPP"){
            initialiseTpp()
        }
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
    fun givenMySessionHasExpired(){
        Serenity.setSessionVariable("SESSION_EXPIRY_MINUTES").to(1)
        iWaitForXSeconds(61)
    }
}