package features.sharedStepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockDefaults
import mocking.MockingClient
import mocking.emis.models.AssociationType
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import java.net.URL

open class SharedStepDefinitions {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var navBar: NavigationSteps

    val mockingClient = MockingClient.instance

    @Given("^wiremock is initialised")
    fun initialiseWiremock() {
        MockDefaults(Config.instance, mockingClient).mock()

        mockingClient.forEmis { sessionRequest(MockDefaults.patient).respondWithSuccess(MockDefaults.patient, AssociationType.Self) }
    }

    @Given("^I am logged in$")
    open fun iAmLoggedIn() {
        browser.goToApp()
        login.asDefault()
    }

    @Given("^I am not logged in$")
    open fun iAmNotLoggedIn() {
        browser.goToApp()
    }

    @When("^I navigate to (.*)$")
    open fun iNavigateTo(tab: String) {
        navBar.select(tab)
    }


    @Then("^I see the (.*) menu button")
    fun iSeeAMenuButton(type: String) {
        Assert.assertTrue(navBar.hasVisible(type))
    }

    @And("^the (.*) menu button is highlighted")
    fun iSeeAHighlightedMenuButton(type: String) {
        Assert.assertTrue(navBar.hasSelectedTab(type))
    }

    @Then("^I am redirected to (.*)$")
    fun iAmRedirectedTo(url: String) {
        browser.shouldHaveUrl(url)
    }

    @Then("^a new tab opens (.*)$")
    fun aNewTabOpens(url: String) {
        browser.changeTab(URL(url))
        browser.shouldHaveUrl(url)
        browser.changeTabToApp()
    }

    @Then("^the spinner appears$")
    fun theSpinnerAppears() {
        // TODO
    }

}