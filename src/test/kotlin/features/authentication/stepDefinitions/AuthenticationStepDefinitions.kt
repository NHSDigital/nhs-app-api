package features.authentication.stepDefinitions

import config.Config
import cucumber.api.DataTable
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myAccount.steps.MyAccountSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import net.serenitybdd.core.PendingStepException
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert


class AuthenticationStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var myAccount: MyAccountSteps

    lateinit var currentUrl: String

    @Given("^I have just logged out$")
    fun iHaveJustLoggedOut() {
        browser.goToApp()
        login.asDefault()
        nav.myAccount()
        myAccount.signOut()
    }

    @And("^I have a slow connection$")
    fun hasASlowConnection() {
        // TODOs
    }

    @When("^I log in")
    fun logIn() {
        login.asDefault()
    }

    @When("^I browse to the page at (.*)$")
    fun iBrowseToPageAt(url: String) {
        val fullUrl = Config.instance.url+url
        browser.browseTo(fullUrl)
        this.currentUrl = fullUrl
    }

    @Then("^I see the home page")
    fun iSeeTheHomePage() {
        home.assertPageIsVisible()
    }

    @Then("^I see the login page")
    fun iSeeTheLoginPage() {
        login.assertPageIsDisplayed()
    }

    @Then("^I see the relevant page")
    fun iSeeTheRelevantPage() {
        browser.shouldHaveUrl(this.currentUrl)
    }

    @Then("^I see a welcome message for (.*)$")
    fun iSeeAWelcomeMessageFor(name: String) {
        home.assertWelcomeMessageShownFor(name)
    }

    @And("^I see the header$")
    fun iSeeHeader() {
        home.assertHeaderVisible()
    }

    @And("^I see the navigation menu$")
    fun iSeeNavbar() {
        nav.assertVisible()
    }
}