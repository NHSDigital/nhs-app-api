package features.authentication.stepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.myAccount.steps.MyAccountSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockDefaults
import mocking.MockingClient
import mocking.emis.models.AssociationType
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.commons.lang3.StringUtils
import org.junit.Assert
import worker.WorkerClient
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse


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

    val mockingClient = MockingClient.instance

    private var authCode: String? = null
    private var codeVerifier: String? = null
    private val accessToken = MockDefaults.DEFAULT_ACCESS_TOKEN
    private val bearerToken = MockDefaults.DEFAULT_BEARER_TOKEN

    private var userSessionResponse: UserSessionResponse? = null

    lateinit var currentUrl: String

    @Given("^I have a valid authCode and codeVerifier$")
    fun iHaveValidAuthCodeAndCodeVerifier() {
        this.authCode = MockDefaults.userSessionRequest.authCode
        this.codeVerifier = MockDefaults.userSessionRequest.codeVerifier

        createSuccessCidStubs()
        createSuccessEmisStubs()
    }

    private fun createSuccessCidStubs(
            authCode:String = this.authCode!!,
            codeVerifier: String = this.codeVerifier!!,
            accessToken: String = this.accessToken,
            bearerToken: String = this.bearerToken) {
        mockingClient.forCitizenId {
            tokenRequest(codeVerifier, authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess()
        }
    }

    private fun createSuccessEmisStubs(patient: Patient = MockDefaults.patient, defaultAssociationType: AssociationType = AssociationType.Self) {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, defaultAssociationType) }
    }

    @When("^I create a user session$")
    fun iCreateUserSessionWithValidDetails() {
        val result = WorkerClient().postSessionConnection(UserSessionRequest(authCode = this.authCode, codeVerifier = this.codeVerifier!!))
        this.userSessionResponse = result
    }

    @Then("^I receive a response$")
    fun iReceiveAResponse() {
        checkNotNull(this.userSessionResponse)
    }

    @And("^the response has a given name$")
    fun theResponseHasAGivenName() {
        checkNotNull(this.userSessionResponse?.userSessionResponseBody?.givenName)
    }

    @And("^the response has a family name$")
    fun theResponseHasAFamilyName() {
        checkNotNull(this.userSessionResponse?.userSessionResponseBody?.familyName)
    }

    @And("^the response has a session timeout$")
    fun theResponseHasASessionLength() {
        checkNotNull(this.userSessionResponse?.userSessionResponseBody?.sessionTimeout)
    }

    @Then("^the cookie contains a session guid with http-only$")
    fun iReceiveCookieWithSessionIdHttpOnly() {
        val cookieParams = retrieveCookie(this.userSessionResponse!!)
        Assert.assertFalse("NHSO-Session-Id is empty or null", cookieParams["NHSO-Session-Id"].isNullOrEmpty())
        Assert.assertTrue(cookieParams.toString(), !cookieParams["httponly"].isNullOrEmpty() && cookieParams["httponly"]!!.toBoolean())
    }

    private fun retrieveCookie(result: UserSessionResponse): HashMap<String, String> {
        checkNotNull(result.userSessionResponseCookie.cookie)
        Assert.assertFalse("Cookie value is empty or null", result.userSessionResponseCookie.cookie.value.isNullOrEmpty())
        val cookieContents = StringUtils.split(result.userSessionResponseCookie.cookie.value, "; ")
        val cookieParams = HashMap<String, String>()
        for (c in cookieContents) {
            if (c.contains('=')) {
                val pair = StringUtils.split(c, "=")
                cookieParams[pair[0]] = pair[1]
            } else {
                cookieParams[c] = "true"
            }
        }
        return cookieParams
    }

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