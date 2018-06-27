package features.authentication.stepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.AuthReturnSteps
import features.authentication.steps.CIDAccountCreationSteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.myAccount.steps.MyAccountSteps
import features.sharedSteps.NavigationSteps
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.emis.models.AssociationType
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.commons.lang3.StringUtils
import org.apache.http.HttpStatus
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.time.Duration


class AuthenticationStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var authReturn: AuthReturnSteps
    @Steps
    lateinit var accountCreation: CIDAccountCreationSteps
    @Steps
    lateinit var myAccount: MyAccountSteps

    private var authCode: String? = MockDefaults.userSessionRequest.authCode
    private var codeVerifier: String? = MockDefaults.userSessionRequest.codeVerifier
    private val accessToken = MockDefaults.DEFAULT_ACCESS_TOKEN
    private val bearerToken = MockDefaults.DEFAULT_BEARER_TOKEN
    lateinit private var patient: Patient
    private val associationType = AssociationType.Self

    private var userSessionResponse: UserSessionResponse? = null
    private var errorResponse: NhsoHttpException? = null

    lateinit var currentUrl: String

    @And("^sign in verification is slow$")
    fun signInVerificationIsSlow() {
        mockingClient.forEmis {
            endUserSessionRequest().respondWithSuccess(
                    endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID)
                    .delayedBy(Duration.ofSeconds(2))
        }
    }

    @Given("^I have a valid authCode and codeVerifier$")
    fun iHaveValidAuthCodeAndCodeVerifier() {
        createCidStubs()
        createEmisStubs()
    }

    @Given("^I have incomplete OAuth details$")
    fun iHaveIncompleteOAuthDetails() {
        createCidStubs(authCode = null)
    }


    @Given("^I have invalid OAuth details$")
    fun iHaveInvalidOAuthDetails() {
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithBadRequest()
        }
        mockingClient.forCitizenId {
            userInfoRequest(this@AuthenticationStepDefinitions.bearerToken)
                    .respondWithSuccess(Patient.montelFrye)
        }
        
        createEmisStubs()
    }

    @Given("^I have valid OAuth details and the CID tokens endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDTokenEndpointFails() {
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithServerError()
        }
        mockingClient.forCitizenId {
            userInfoRequest(this@AuthenticationStepDefinitions.bearerToken)
                    .respondWithSuccess(Patient.montelFrye)
        }
        
        createEmisStubs()
    }

    @Given("^I have valid OAuth details and the CID user profile endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDUserProfileEndpointFails() {
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(this@AuthenticationStepDefinitions.bearerToken)
                    .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
        }
        
        createEmisStubs()
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        createCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        createCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId) }
        mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and (.*) is unavailable$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        createCidStubs()
        when(gpSystem.toUpperCase()){
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithServiceUnavailable() }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
            }
            "TPP" -> {
                mockingClient.forTpp { authenticateRequest(Authenticate())
                        // respond with error.  Unconfirmed format.
                        .respondWithError(Error(errorCode = "0", userFriendlyMessage = "Service Unavailable")) }
            }
        }
    }

    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        createCidStubs()
        when(gpSystem.toUpperCase()){
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault(gpSystem).endUserSessionId) }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithForbidden() }
            }
            "TPP" -> {
                mockingClient.forTpp { authenticateRequest(Authenticate())
                        // respond with error.  Unconfirmed format.
                        .respondWithError(Error(errorCode = "9", userFriendlyMessage = "There was a problem logging on")) }
            }
        }
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in 30 seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInThirtySeconds(gpSystem: String) {

        when(gpSystem.toUpperCase()){
            "EMIS" -> {
                createCidStubs()
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId).delayedBy(Duration.ofSeconds(31)) }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
            }
            "TPP" -> {
                CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patientTpp)
                mockingClient.forTpp { authenticateRequest(MockDefaults.tppAuthenticateRequest).respondWithSuccess(AuthenticateReply()).delayedBy(Duration.ofSeconds(31)) }
            }
        }
    }

    private fun createCidStubs(
            authCode:String? = this.authCode!!,
            codeVerifier: String = this.codeVerifier!!,
            accessToken: String = this.accessToken,
            bearerToken: String = this.bearerToken,
            patient: Patient = MockDefaults.patient) {
        mockingClient.forCitizenId {
            tokenRequest(codeVerifier, authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess(patient)
        }
    }

    private fun createEmisStubs(patient: Patient = Patient.getDefault("EMIS"), defaultAssociationType: AssociationType = this.associationType) {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId) }
        mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), defaultAssociationType) }
    }

    @When("^I create a user session$")
    fun iCreateUserSession() {
        try {
            this.userSessionResponse = WorkerClient().postSessionConnection(UserSessionRequest(authCode = this.authCode, codeVerifier = this.codeVerifier!!))
        }
        catch (httpException: NhsoHttpException) {
            this.errorResponse = httpException
        }
    }

    @Then("^I receive a response$")
    fun iReceiveAResponse() {
        checkNotNull(this.userSessionResponse)
    }

    @And("^the response has a name$")
    fun theResponseHasAName() {
        Assert.assertEquals("${MockDefaults.patient.firstName} ${MockDefaults.patient.surname}", this.userSessionResponse?.userSessionResponseBody?.name)
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

    @Then("^I get (?:a|an) \"(.*)\" error")
    fun thenIReceiveAnError(expectedStatusCode: String) {
        val code = httpStatusCodeTransform(expectedStatusCode)
        checkNotNull(this.errorResponse)
        Assert.assertEquals(code, this.errorResponse?.StatusCode)
    }

    private val _errorMapping: HashMap<String, Int> = hashMapOf(
            "bad gateway" to HttpStatus.SC_BAD_GATEWAY,
            "bad request" to HttpStatus.SC_BAD_REQUEST,
            "gateway timeout" to HttpStatus.SC_GATEWAY_TIMEOUT,
            "not found" to HttpStatus.SC_NOT_FOUND,
            "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR,
            "conflict" to HttpStatus.SC_CONFLICT,
            "forbidden" to HttpStatus.SC_FORBIDDEN,
            "service unavailable" to HttpStatus.SC_SERVICE_UNAVAILABLE
    )

    fun httpStatusCodeTransform(errorName: String): Int? {
        return _errorMapping[errorName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $errorName")
    }

    @Given("^I have just logged out$")
    fun iHaveJustLoggedOut() {
        browser.goToApp()
        login.asDefault()
        nav.myAccount()
        myAccount.signOut()
    }

    @Given("^I am logged in as a (.*) user$")
    fun iAmLoggedInTo(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        when(gpSystem.toUpperCase()) {
            "EMIS" -> { EmisSessionCreateJourneyFactory(mockingClient).createFor(patient) }
            "TPP" -> { TppSessionCreateJourneyFactory(mockingClient).createFor(patient) }
        }

        browser.goToApp()
        login.asDefault()
    }

    @When("^I log in$")
    fun logIn() {
        login.asDefault(false)
    }

    @When("I am on the home page")
    fun gotoHomePage()
    {
        browser.changeTabToApp()
    }

    @When("^I browse to the page at (.*)$")
    fun iBrowseToPageAt(url: String) {
        val fullUrl = Config.instance.url+url
        browser.browseTo(fullUrl)
        this.currentUrl = fullUrl
    }

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        home.assertPageIsVisible()
    }

    @Then("^I see the login page$")
    fun iSeeTheLoginPage() {
        login.assertPageIsDisplayed()
    }

    @Then("^I see the relevant page$")
    fun iSeeTheRelevantPage() {
        browser.shouldHaveUrl(this.currentUrl)
    }

    @Then("^I see a welcome message$")
    fun iSeeAWelcomeMessageFor() {
        val fullName = "${patient.title} ${patient.firstName} ${patient.surname}".trim()
        home.assertWelcomeMessageShownFor(fullName)
    }

    @And("^I see the header$")
    fun iSeeHeader() {
        home.assertHeaderVisible()
    }

    @And("^I see the navigation menu$")
    fun iSeeNavbar() {
        nav.assertVisible()
    }

    @When("^I sign out")
    @Throws(Exception::class)
    fun iClickTheSignOutButton() {
        nav.myAccount()
        myAccount.signOut();
    }

    @Then("^I do not see the menu bar$")
    @Throws(Exception::class)
    fun iDoNotSeeMenuBar() {
        login.assertMenuIsNotVisible()
    }

    @Then("^the user login details are cleared from cookies$")
    @Throws(Exception::class)
    fun theUserLoginDetailsAreClearedFromCookies() {
        browser.checkLoginDetailsAreReset();
    }

    @Given("^I want to register for an account$")
    fun iWantToRegisterForAnAccount() {
        this.patient = MockDefaults.patient
        browser.goToApp()
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(this.patient)
    }

    @Then("^I see create account button$")
    fun iSeeCreateAccountButton() {
        login.assertCreateAccountButtonIsVisible()
    }

    @When("^I select to create an account$")
    fun iClickCreateAccountButton() {
        login.clickCreateAccountButton()
    }

    @When("^I have completed account creation$")
    fun iCreateAnAccount() {
        this.patient = MockDefaults.patient
        iWantToRegisterForAnAccount()
        iCompleteAccountRegistration()
    }

    @And("^I complete the account registration$")
    fun iCompleteAccountRegistration() {
        login.createAccount()
    }

    @Then("^the spinner appears$")
    @Throws(Exception::class)
    fun theSpinnerAppears() {
        authReturn.assertSpinnerVisible()
    }

    @Then ("^I am redirected to the CID create an account page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheCIDCreateAnAccountPage() {
        accountCreation.assertPageIsVisible()
    }

    @Then ("^I am redirected to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheSignedInHomePage()
    {
        home.assertPageIsVisible();
    }

    @Then ("^I am redirected to the app to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheAppToTheSignedInHomePage()
    {
        home.assertPageIsVisible();
    }

}