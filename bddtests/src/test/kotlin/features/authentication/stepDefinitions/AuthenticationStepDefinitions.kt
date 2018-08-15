package features.authentication.stepDefinitions

import com.google.gson.Gson
import config.Config
import cucumber.api.java.en.*
import features.authentication.steps.AuthReturnSteps
import features.authentication.steps.CIDAccountCreationSteps
import features.authentication.steps.HomeSteps
import features.authentication.steps.LoginSteps
import features.sharedStepDefinitions.backend.AbstractSteps
import features.sharedSteps.BrowserSteps
import features.myAccount.steps.MyAccountSteps
import features.sharedSteps.NavigationSteps
import io.restassured.builder.RequestSpecBuilder
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.*
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mocking.emis.models.IdentifierType
import mocking.tpp.models.Authenticate
import mocking.tpp.models.AuthenticateReply
import mocking.tpp.models.Error
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import net.serenitybdd.rest.SerenityRest
import net.thucydides.core.annotations.Steps
import org.apache.commons.lang3.StringUtils
import org.apache.http.HttpStatus
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.WorkerPaths
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.net.URI
import java.time.Duration

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
    lateinit var authReturn: AuthReturnSteps
    @Steps
    lateinit var accountCreation: CIDAccountCreationSteps
    @Steps
    lateinit var myAccount: MyAccountSteps

    private var authCode: String? = MockDefaults.patient.cidUserSession.authCode
    private var codeVerifier: String? = MockDefaults.patient.cidUserSession.codeVerifier
    private val accessToken = MockDefaults.DEFAULT_ACCESS_TOKEN
    private val bearerToken = MockDefaults.DEFAULT_BEARER_TOKEN
    lateinit private var patient: Patient
    private val associationType = AssociationType.Self

    private var im1ConnectionRequest: Im1ConnectionRequest? = null
    private var im1ConnectionResponse: Im1ConnectionResponse? = null
    private var userSessionResponse: UserSessionResponse? = null
    private var errorResponse: NhsoHttpException? = null

    lateinit var currentUrl: String

    @And("^sign in verification is slow$")
    fun signInVerificationIsSlow() {
        mockingClient.forEmis {
            endUserSessionRequest().respondWithSuccess(
                    endUserSessionId = MockDefaults.DEFAULT_END_USER_SESSION_ID)
                    .delayedBy(Duration.ofSeconds(10))
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

    @Given("^I have valid OAuth details and (.*) is not available$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        createCidStubs()
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithServiceUnavailable() }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    authenticateRequest(Authenticate())
                            // respond with error.  Unconfirmed format.
                            .respondWithError(Error(errorCode = "0", userFriendlyMessage = "Service Unavailable"))
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    getConfigurationRequest(
                            MockDefaults.visionUserSession,
                            MockDefaults.visionGetConfiguration)
                            .respondWithServiceUnavailable()
                }
            }
        }
    }


    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        createCidStubs()
        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault(gpSystem).endUserSessionId) }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithForbidden() }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    authenticateRequest(Authenticate())
                            // respond with error.  Unconfirmed format.
                            .respondWithError(Error(errorCode = "9", userFriendlyMessage = "There was a problem logging on"))
                }
            }
            "VISION" -> {
                createCidStubs(patient = MockDefaults.patientVision)
                mockingClient
                        .forVision {
                            getConfigurationRequest(MockDefaults.visionUserSession, MockDefaults.visionGetConfiguration)
                                    .respondWitInvalidUserCredentials()
                        }
            }
        }
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in 30 seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInThirtySeconds(gpSystem: String) {

        when (gpSystem.toUpperCase()) {
            "EMIS" -> {
                createCidStubs()
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId).delayedBy(Duration.ofSeconds(31)) }
                mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
            }
            "TPP" -> {
                CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patientTpp)
                mockingClient.forTpp { authenticateRequest(MockDefaults.tppAuthenticateRequest).respondWithSuccess(AuthenticateReply()).delayedBy(Duration.ofSeconds(31)) }
            }
            "VISION" -> {
                CitizenIdSessionCreateJourney(mockingClient).createFor(MockDefaults.patientVision)
                mockingClient
                        .forVision {
                            getConfigurationRequest(MockDefaults.visionUserSession, MockDefaults.visionGetConfiguration)
                                    .respondWithSuccess(MockDefaults.visionConfigurationResponse).delayedBy(Duration.ofSeconds(31))
                        }
            }
        }
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

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithNoOnlineUserFound() }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
                }
            }
        }


        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect linkage key$")
    fun iHaveDataForAPatientWithIncorrectLinkageKey(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = "incorrectLinkageKey")

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithLinkageKeyDoesNotMatch() }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
                }
            }
        }

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect surname$")
    fun iHaveDataForAPatientWithIncorrectSurname(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(surname = "incorrectSurname")
        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithIncorrectSurnameOrDateOfBirth() }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
                }
            }
        }

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect date of birth$")
    fun iHaveDataForAPatientWithIncorrectDateOfBirth(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = "1918-12-24T14:03:15.892Z")

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithIncorrectSurnameOrDateOfBirth() }
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    linkAccountRequest(patient).respondWithInvalidLinkageCredentials()
                }
            }
        }

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

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithBadRequest("The request is invalid.", "Surname") }
            }
            "TPP" -> {
                mockingClient.forTpp { linkAccountRequest(patient).respondWithInvalidLinkageCredentials() }
            }
        }

        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with an Account ID not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnAccountIdNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(accountId = INVALID_VALUE)

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithBadRequest("The request is invalid.", "LinkageDetails.AccountId") }
            }
            "TPP" -> {
                mockingClient.forTpp { linkAccountRequest(patient).respondWithInvalidLinkageCredentials() }
            }
        }

        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Linkage Key not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithALinkageKeyNotInTheExpectedFormat(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = INVALID_VALUE)

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
                mockingClient.forEmis { meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithBadRequest("The request is invalid.", "LinkageDetails.LinkageKey") }
            }
            "TPP" -> {
                mockingClient.forTpp { linkAccountRequest(patient).respondWithInvalidLinkageCredentials() }
            }
        }

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

        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
            meApplicationsRequest(patient, createLinkApplicationRequestModel(patient)).respondWithAlreadyLinked()
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
        val uri = URI(Config.instance.backendUrl + WorkerPaths.patientIm1Connection)
        val response = SerenityRest
                .given()
                .contentType("application/json")
                .body(Gson().toJson(this.im1ConnectionRequest!!))
                .post(uri)

        if (arrayOf(200, 201).contains(response.statusCode)) {
            this.im1ConnectionResponse = Gson().fromJson(response.body.asString(), Im1ConnectionResponse::class.java)
        } else {
            setErrorResponse(NhsoHttpException(uri=uri.toString(), statusCode = response.statusCode, body = response.body?.toString(), method = "POST"))
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
        Assert.assertEquals("${MockDefaults.patient.firstName} ${MockDefaults.patient.surname}", this.userSessionResponse?.userSessionResponseBody?.name)
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
        nav.myAccount()
        myAccount.signOut()
    }

    @Given("^I am logged in as a (.*) user$")
    fun iAmLoggedInTo(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(this.patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        browser.goToApp()
        login.using(this.patient)
        home.waitForLoginToComplete()
    }

    @Given("^I am logged in as a (.*) user where the session will fail to clear on signout$")
    fun iAmLoggedInToWhereSessionFailsToClear(gpSystem: String) {
        if (gpSystem.toUpperCase() != "TPP") {
            Assert.fail("'$gpSystem' not set up for this step")
        }
        this.patient = Patient.getDefault(gpSystem)
        Serenity.setSessionVariable(Patient::class).to(this.patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        //Whereas the usual TppSessionCreateJourneyFactory.createFor includes the logOff request,
        //createAuthenticateRequest does not.
        TppSessionCreateJourneyFactory(mockingClient).createAuthenticateRequest(patient)
        mockingClient.forTpp { logOffRequest().respondWithError() }

        browser.goToApp()
        login.using(this.patient)
    }

    @When("^I log in$")
    fun logIn() {
        login.asDefault()
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
        myAccount.signOut()
        browser.waitUntilSignoutCompletes()
    }

    @Then("^I do not see the menu bar$")
    @Throws(Exception::class)
    fun iDoNotSeeMenuBar() {
        login.assertMenuIsNotVisible()
    }

    @Then("^I see the sign out button$")
    fun iSeeTheSignOutButton() {
        myAccount.assertSignoutButtonVisible()
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
        this.patient = MockDefaults.patient
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
        authReturn.assertSpinnerVisible()
    }

    @Then("^I am redirected to the CID create an account page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheCIDCreateAnAccountPage() {
        accountCreation.assertPageIsVisible()
    }

    @Then("^I am redirected to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheSignedInHomePage() {
        home.assertPageIsVisible()
    }

    @Then("^I am redirected to the app to the signed in home page$")
    @Throws(Exception::class)
    fun IAmRedirectedToTheAppToTheSignedInHomePage() {
        home.assertPageIsVisible()
    }

    private fun createLinkApplicationRequestModel(patient: Patient): LinkApplicationRequestModel {
        return LinkApplicationRequestModel(
                surname = patient.surname,
                dateOfBirth = patient.dateOfBirth,
                linkageDetails = LinkageDetailsModel(
                        accountId = patient.accountId,
                        nationalPracticeCode = patient.odsCode,
                        linkageKey = patient.linkageKey
                )
        )
    }


    private fun createEmisStubs(patient: Patient = Patient.getDefault("EMIS"), defaultAssociationType: AssociationType = this.associationType) {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(Patient.getDefault("EMIS")).respondWithSuccess(Patient.getDefault("EMIS"), defaultAssociationType) }
        mockingClient.forEmis {
            demographicsRequest(patient).respondWithSuccess(patient,
                    patientIdentifiers = arrayOf(
                            PatientIdentifier(
                                    identifierType = IdentifierType.NhsNumber,
                                    identifierValue = patient.nhsNumbers[0]
                            )
                    )
            )
        }
    }

    private fun createCidStubs(
            authCode: String? = this.authCode!!,
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

