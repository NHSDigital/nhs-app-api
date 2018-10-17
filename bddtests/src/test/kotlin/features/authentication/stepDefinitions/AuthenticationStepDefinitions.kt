package features.authentication.stepDefinitions

import com.google.gson.Gson
import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.CIDAccountCreationSteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.navigation.steps.NavHeaderSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import features.sharedSteps.SerenityHelpers
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.emis.me.EmisMeApplicationsBuilder
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.models.Mapping
import models.Patient
import net.serenitybdd.core.Serenity.setSessionVariable
import net.serenitybdd.rest.SerenityRest
import net.thucydides.core.annotations.Steps
import org.apache.commons.lang3.StringUtils
import org.apache.http.HttpStatus
import org.junit.Assert
import pages.AuthReturnPage
import pages.MyAccountPage
import pages.ServiceUnavailablePage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.WorkerPaths
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.net.URI

const val INVALID_VALUE = "xxx-wrong-format-xxx"

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
    lateinit var navHeader: NavHeaderSteps
    @Steps
    lateinit var accountCreation: CIDAccountCreationSteps

    lateinit var myAccount: MyAccountPage

    lateinit var authReturnPage: AuthReturnPage
    lateinit var serviceUnavailablePage: ServiceUnavailablePage

    private var authCode: String? = MockDefaults.patient.cidUserSession.authCode
    private var codeVerifier: String? = MockDefaults.patient.cidUserSession.codeVerifier
    private lateinit var patient: Patient
    private val associationType = AssociationType.Self

    private var im1ConnectionRequest: Im1ConnectionRequest? = null
    private var im1ConnectionResponse: Im1ConnectionResponse? = null
    private var userSessionResponse: UserSessionResponse? = null
    private var errorResponse: NhsoHttpException? = null

    lateinit var currentUrl: String

    @Given("^I have a valid authCode and codeVerifier$")
    fun iHaveValidAuthCodeAndCodeVerifier() {
        val gpSystem = "EMIS"
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
    }

    @Given("^I have incomplete OAuth details$")
    fun iHaveIncompleteOAuthDetails() {
        this.authCode = null
    }

    @Given("^I have invalid OAuth details$")
    fun iHaveInvalidOAuthDetails() {
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithBadRequest()
        }
    }

    @Given("^I have valid OAuth details and the CID tokens endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDTokenEndpointFails() {
        val gpSystem = "EMIS"
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithServerError()
        }
        SessionCreateJourneyFactory.getForSupplier("EMIS", mockingClient).createFor(patient)
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patient)
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis { authentication.sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patient)
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId) }
        mockingClient.forEmis { authentication.sessionRequest(Patient.getDefault("EMIS")).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and (.*) is not available$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsAndGpSystemUnavailable()
    }

    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in 30 seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInThirtySeconds(gpSystem: String) {

        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsAndGpSystemSlowToRespond()
    }

    @Given("^I have a new (.+) patient with Nhs Numbers of (.*)$")
    fun iHaveValidPatientDataToRegisterNewAccount(gpSystem: String, nhsNumbers: String) {
        val nhsNumbersList = nhsNumbers.split(",").filter { it.isNotEmpty() }
        this.patient = Patient.getDefault(gpSystem).copy(nhsNumbers = nhsNumbersList)

        SuccessfulRegistrationJourney(mockingClient).create(this.patient, gpSystem)

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient that does not exist$")
    fun iHaveDataForAPatientThatDoesNotExist(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(nhsNumbers = arrayListOf("nonExistingNhsNumber"))
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithNoOnlineUserFound() }
        setIm1Request()
    }


    private fun createInvalidLinkageTest(gpSystem: String, patient: Patient, emisResponse: (EmisMeApplicationsBuilder.() -> Mapping)) {

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { emisResponse(authentication.meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))) }
                mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    authentication.linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
                }
            }
        }
    }

    @Given("^I have data for a (.+) patient with incorrect linkage key$")
    fun iHaveDataForAPatientWithIncorrectLinkageKey(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = "incorrectLinkageKey")
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithLinkageKeyDoesNotMatch() }
        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect surname$")
    fun iHaveDataForAPatientWithIncorrectSurname(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(surname = "incorrectSurname")
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithIncorrectSurnameOrDateOfBirth() }
        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect date of birth$")
    fun iHaveDataForAPatientWithIncorrectDateOfBirth(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithIncorrectSurnameOrDateOfBirth() }
        setIm1Request()
    }

    @Given("^I have a user's IM1 credentials with an ODS Code not in the expected format$")
    fun iHaveAUsersIMCredentialsWithAnODSCodeNotInTheExpectedFormat() {
        this.patient = Patient.getDefault("EMIS").copy(odsCode = INVALID_VALUE)
        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Surname not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithASurnameNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(surname = INVALID_VALUE)
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithBadRequest("The request is invalid.", "Surname") }
        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with an Account ID not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnAccountIdNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(accountId = INVALID_VALUE)
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithBadRequest("The request is invalid.", "LinkageDetails.AccountId") }
        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Linkage Key not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithALinkageKeyNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = INVALID_VALUE)
        createInvalidLinkageTest(gpSystem, this.patient) { respondWithBadRequest("The request is invalid.", "LinkageDetails.LinkageKey") }
        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Date Of Birth not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithADateOfBirthNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = INVALID_VALUE)
        setIm1Request()
    }

    private fun setIm1Request() {
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have a user's IM1 credentials with missing ODS Code$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingODSCode() {
        this.patient = Patient.johnSmith
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Surname$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingSurname(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Account ID$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingAccountID(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        this.im1ConnectionRequest = Im1ConnectionRequest(
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have a (.+) user's IM1 credentials with missing Linkage Key$")
    fun iHaveAn_EMISUsersIMCredentialsWithMissingLinkageKey(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have data for an EMIS patient that has already been associated with the application in the GP system$")
    fun iHaveDataForAnEMISPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGPSystem() {
        this.patient = Patient.getDefault("EMIS")

        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
            authentication.meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithAlreadyLinked()
        }

        setIm1Request()
        setSessionVariable("HttpExceptionExpected").to(true)
    }

    @When("^I create a user session$")
    fun iCreateUserSession() {
        try {
            this.userSessionResponse = WorkerClient().postSessionConnection(
                    UserSessionRequest(authCode = this.authCode, codeVerifier = this.codeVerifier!!, redirectUrl = Config.instance.cidRedirectUri))
        } catch (httpException: NhsoHttpException) {
            setErrorResponse(httpException)
        }
    }

    @When("^I register the user's IM1 credentials$")
    fun iRegisterAnEMISUsersIMCredentials() {
        val uri = URI(Config.instance.cidBackendUrl + WorkerPaths.patientIm1Connection)
        val response = SerenityRest
                .given()
                .contentType("application/json")
                .body(Gson().toJson(this.im1ConnectionRequest!!))
                .post(uri)

        if (arrayOf(200, 201).contains(response.statusCode)) {
            this.im1ConnectionResponse = Gson().fromJson(response.body.asString(), Im1ConnectionResponse::class.java)
        } else {
            setErrorResponse(NhsoHttpException(uri = uri.toString(), statusCode = response.statusCode, body = response.body?.toString(), method = "POST"))
        }
    }

    @Then("^I receive a response$")
    fun iReceiveAResponse() {
        val responses = arrayListOf(this.userSessionResponse, this.im1ConnectionResponse).filter { it != null }
        Assert.assertEquals("No responses found.  Errors: ${this.errorResponse}", responses.size, 1)
        Assert.assertEquals(this.errorResponse, null)
    }

    @And("^the response has a name$")
    fun theResponseHasAName() {
        Assert.assertEquals("${MockDefaults.patient.title} ${MockDefaults.patient.firstName} ${MockDefaults.patient.surname}", this.userSessionResponse?.userSessionResponseBody?.name)
    }

    @And("^the response has a session timeout$")
    fun theResponseHasASessionLength() {
        checkNotNull(this.userSessionResponse?.userSessionResponseBody?.sessionTimeout)
    }

    @Then("^the response has the expected connection token$")
    fun theResponseHasTheExpectedConnectionToken() {
        val result = this.im1ConnectionResponse

        Assert.assertEquals(this.patient.connectionToken, result!!.connectionToken)
    }

    @Then("^the response has the expected NHS numbers$")
    fun theResponseHasTheExpectedNhsNumbers() {
        val response = this.im1ConnectionResponse
        val responseNhsNumbers = response!!.nhsNumbers!!.map { it.nhsNumber.replace(" ", "") }

        Assert.assertEquals(this.patient.nhsNumbers, responseNhsNumbers)
    }

    @Then("^the cookie contains a session guid with http-only$")
    fun iReceiveCookieWithSessionIdHttpOnly() {
        val cookieParams = retrieveCookie(this.userSessionResponse!!)
        Assert.assertFalse("NHSO-Session-Id is empty or null", cookieParams["NHSO-Session-Id"].isNullOrEmpty())
        Assert.assertTrue(cookieParams.toString(), !cookieParams["httponly"].isNullOrEmpty() && cookieParams["httponly"]!!.toBoolean())
    }

    @Then("^I get (?:a|an) \"(.*)\" error")
    fun thenIReceiveAnError(expectedStatusCode: String) {
        val code = httpStatusCodeTransform(expectedStatusCode)
        checkNotNull(this.errorResponse)
        Assert.assertEquals(code, this.errorResponse?.statusCode)
    }

    @Given("^I have just logged out$")
    fun iHaveJustLoggedOut() {
        browser.goToApp()
        login.asDefault()
        navHeader.clickMyAccount()
        myAccount.signOutButton.element.click()
    }

    @When("I log out")
    fun iLogOut(){
        navHeader.clickMyAccount()
        myAccount.signOutButton.element.click()
    }

    @Given("^I am logged in as a (.*) user$")
    fun iAmLoggedInTo(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(this.patient)
        setupAndLogIn(patient, gpSystem)
    }

    @When("I log in again")
    fun iLogInAgain(){
        val patient = SerenityHelpers.getPatient()
        login.using(patient)
        home.waitForLoginToComplete()
    }

    @Given("^I attempt to log in as a (.*) user without an NHS Number$")
    fun iAmLoggedInToWithoutNHSNumber(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(nhsNumbers = emptyList())
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user without a date of birth$")
    fun iAmLoggedInToWithoutDOB(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = "")
        setupAndLogIn(patient, gpSystem)
    }

    @Given("I attempt to log in as a (.*) user with invalid ODS Code$")
    fun iAttemptToLogInWithInvalidOdsCode(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(odsCode = "A33224")
        setupAndLogIn(patient, gpSystem)
    }

    @Then("^I see an error message informing me I cannot log in as I am under 16$")
    fun iSeeAnErrorMessageInformingMeICannotLogInAsIAmUnderSixteen() {
        serviceUnavailablePage.assertIsPresent("As you’re under 16, you cannot currently access the NHS App.")
    }

    @Then("^I see an error message informing me I cannot log in$")
    fun iSeeAnErrorMessageInformingMeICannotLogIn() {
        serviceUnavailablePage.assertIsPresent("You can still call or visit your GP surgery to access your NHS services. For urgent medical advice, call 111.")
    }

    fun setupAndLogIn(patient: Patient, gpSystem: String) {
        this.patient = patient
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        browser.goToApp()
        login.using(patient)
        home.waitForLoginToComplete()
    }

    @Given("^I am logged in as a (.*) user where the session will fail to clear on signout$")
    fun iAmLoggedInToWhereSessionFailsToClear(gpSystem: String) {
        if (gpSystem.toUpperCase() != "TPP") {
            Assert.fail("'$gpSystem' not set up for this step")
        }
        this.patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        //Whereas the usual TppSessionCreateJourneyFactory.createFor includes the logOff request,
        //createAuthenticateRequest does not.
        TppSessionCreateJourneyFactory(mockingClient).createAuthenticateRequest(patient)
        mockingClient.forTpp { authentication.logOffRequest().respondWithError() }

        browser.goToApp()
        login.using(this.patient)
    }

    @When("I am on the home page")
    fun gotoHomePage() {
        browser.changeTabToApp()
    }

    @When("^I browse to the page at (.*)$")
    fun iBrowseToPageAt(url: String) {
        val fullUrl = Config.instance.url + url
        browser.browseTo(fullUrl)
        this.currentUrl = fullUrl
    }

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        home.assertHeaderVisible()
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
        var patient = SerenityHelpers.getPatient()
        home.assertWelcomeMessageShownFor(patient)
    }

    @Then("I see the patient details of name, date of birth and NHS number$")
    fun iSeePatientDetails() {
        var patient = SerenityHelpers.getPatient()
        val regex = """${'^'}${'['}0-9${']'}${'{'}10${'}'}${'$'}""".toRegex()
        Assert.assertTrue("Test Setup Incorrect: Patient must have unformatted nhs number to check front end formatting. Regex: '$regex' Number: '${patient.nhsNumbers.first()}' ",
                regex.containsMatchIn(patient.nhsNumbers.first()))
        home.assertPatientDetailsShownFor(patient)
    }

    @Then("^I see the home page header$")
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
        navHeader.clickMyAccount()
        myAccount.signOutButton.element.click()
        browser.waitUntilSignoutCompletes()
    }

    @Then("^I do not see the menu bar$")
    @Throws(Exception::class)
    fun iDoNotSeeMenuBar() {
        login.assertMenuIsNotVisible()
    }

    @Then("^I see the sign out button$")
    fun iSeeTheSignOutButton() {
        myAccount.signOutButton.assertSingleElementPresent().assertIsVisible()
    }

    @Then("^the user login details are cleared from cookies$")
    @Throws(Exception::class)
    fun theUserLoginDetailsAreClearedFromCookies() {
        browser.checkLoginDetailsAreReset()
    }

    @Given("^I want to register for a (.+) account$")
    fun iWantToRegisterForAnAccount(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)

        browser.goToApp()
    }

    @Then("^I see create account button$")
    fun iSeeCreateAccountButton() {
        login.assertCreateAccountButtonIsVisible()
    }

    @When("^I select to create an account$")
    fun iClickCreateAccountButton() {
        login.clickCreateAccountButton()
    }

    @When("^I have completed (.+) account creation$")
    fun iCreateAnAccount(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(this.patient)
        iWantToRegisterForAnAccount(gpSystem)
        iCompleteAccountRegistration()
    }

    @And("^I complete the account registration$")
    fun iCompleteAccountRegistration() {
        login.createAccount(this.patient)
    }

    @Then("^the spinner appears$")
    @Throws(Exception::class)
    fun theSpinnerAppears() {
        authReturnPage.assertSpinnerVisible()
    }

    @Then("^I am redirected to the CID create an account page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheCIDCreateAnAccountPage() {
        accountCreation.assertPageIsVisible()
    }

    @Then("^I am redirected to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheSignedInHomePage() {
        navHeader.assertHomePageHeaderVisible()
    }

    @Then("^I am redirected to the app to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheAppToTheSignedInHomePage() {
        navHeader.assertHomePageHeaderVisible()
    }

    private fun createLinkApplicationRequestModel(patient: Patient): LinkApplicationRequestModel {
        return LinkApplicationRequestModel(
                surname = patient.surname,
                dateOfBirth = patient.dateOfBirth.plus("T00:00:00"),
                linkageDetails = LinkageDetailsModel(
                        accountId = patient.accountId,
                        nationalPracticeCode = patient.odsCode,
                        linkageKey = patient.linkageKey
                )
        )
    }

    private val _errorMapping: HashMap<String, Int> = hashMapOf(
            "bad gateway" to HttpStatus.SC_BAD_GATEWAY,
            "bad request" to HttpStatus.SC_BAD_REQUEST,
            "gateway timeout" to HttpStatus.SC_GATEWAY_TIMEOUT,
            "not found" to HttpStatus.SC_NOT_FOUND,
            "internal server error" to HttpStatus.SC_INTERNAL_SERVER_ERROR,
            "conflict" to HttpStatus.SC_CONFLICT,
            "forbidden" to HttpStatus.SC_FORBIDDEN,
            "service unavailable" to HttpStatus.SC_SERVICE_UNAVAILABLE,
            "not implemented" to HttpStatus.SC_NOT_IMPLEMENTED
    )

    private fun httpStatusCodeTransform(errorName: String): Int? {
        return _errorMapping[errorName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $errorName")
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

    private fun setErrorResponse(errorResponse: NhsoHttpException) {
        this.errorResponse = errorResponse
        setSessionVariable("HttpException").to(errorResponse)
    }
}
