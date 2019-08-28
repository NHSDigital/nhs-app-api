package features.authentication.stepDefinitions

import config.Config
import constants.DateTimeFormats
import cucumber.api.DataTable
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
import mocking.GsonFactory
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.TppSessionCreateJourneyFactory
import mocking.emis.me.LinkApplicationRequestModel
import mocking.emis.me.LinkageDetailsModel
import mocking.emis.models.AssociationType
import mockingFacade.linkage.LinkageInformationFacade
import models.patients.EmisPatients
import models.Patient
import mongodb.MongoDBConnection
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Steps
import org.apache.commons.lang3.StringUtils
import org.apache.http.HttpStatus
import org.joda.time.DateTime
import org.junit.Assert
import pages.MyAccountPage
import pages.ServiceUnavailablePage
import pages.navigation.WebHeader
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse
import worker.models.patient.Im1ConnectionToken
import worker.models.serviceJourneyRules.AppointmentsProvider
import worker.models.serviceJourneyRules.CdssProvider
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse

const val INVALID_VALUE = "xxx-wrong-format-xxx"

@Suppress("LargeClass", "Do not duplicate this suppression in other classes, " +
        "if possible, break down steps into functional areas")
class AuthenticationStepDefinitions : AbstractSteps() {

    @Steps
    lateinit var accountCreation: CIDAccountCreationSteps
    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var home: HomeSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var nav: NavigationSteps
    @Steps
    lateinit var navHeader: NavHeaderSteps
    @Steps
    lateinit var webHeader: WebHeader

    lateinit var myAccount: MyAccountPage

    lateinit var serviceUnavailablePage: ServiceUnavailablePage

    private var authCode: String? = EmisMockDefaults.patientEmis.cidUserSession.authCode
    private var codeVerifier: String? = EmisMockDefaults.patientEmis.cidUserSession.codeVerifier
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
        val accessToken = Patient.getAccessToken(patient)
        SerenityHelpers.setPatient(patient)
        mockingClient.forCitizenId {
            tokenRequest(this@AuthenticationStepDefinitions.codeVerifier!!, this@AuthenticationStepDefinitions.authCode)
                    .respondWithServerError()
            userInfoRequest(accessToken).respondWithServerError()
        }
        SessionCreateJourneyFactory.getForSupplier("EMIS", mockingClient).createFor(patient)
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis { authentication.sessionRequest(Patient.getDefault("EMIS"))
                .respondWithSuccess(Patient.getDefault("EMIS"), associationType) }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis { authentication.endUserSessionRequest()
                .respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId) }
        mockingClient.forEmis { authentication.sessionRequest(Patient.getDefault("EMIS")).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and (.*) is not available$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsAndGpSystemUnavailable()
    }

    @Given("^I have valid OAuth details and (.*) returns with an incomplete response$")
    fun iHaveValidOAuthDetailsAndGpSystemReturnsAnIncompleteResponse(gpSystem: String)
    {
        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncompleteResponse(Patient.getDefault(gpSystem))
    }

    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        mockingClient = SerenityHelpers.getMockingClient()
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in (\\d+) seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInXSeconds(gpSystem: String, delayBySeconds: Int) {
            mockingClient = SerenityHelpers.getMockingClient()
            CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
            AuthenticationFactory.getForSupplier(gpSystem)
                    .validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds.toLong())
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
        AuthenticationFactory.getForSupplier(gpSystem).patientDoesNotExist(this.patient)

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect linkage key$")
    fun iHaveDataForAPatientWithIncorrectLinkageKey(gpSystem: String) {

        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = "incorrectLinkageKey")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectLinkageKey(this.patient)

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect surname$")
    fun iHaveDataForAPatientWithIncorrectSurname(gpSystem: String) {

        this.patient = Patient.getDefault(gpSystem).copy(surname = "incorrectSurname")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectSurname(this.patient)

        setIm1Request()
    }

    @Given("^I have data for a (.+) patient with incorrect date of birth$")
    fun iHaveDataForAPatientWithIncorrectDateOfBirth(gpSystem: String) {

        this.patient = Patient.getDefault(gpSystem).copy(surname = "1900-01-01")
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncorrectDOB(this.patient)

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
        AuthenticationFactory.getForSupplier(gpSystem).patientWithSurnameInWrongFormat(this.patient)

        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with an Account ID not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithAnAccountIdNotInTheExpectedFormat(gpSystem: String) {

        if(gpSystem == "VISION") {
            this.patient = Patient.getDefault(gpSystem).copy(rosuAccountId = "10496")
        }
        else {
            this.patient = Patient.getDefault(gpSystem).copy(accountId = INVALID_VALUE)
        }
        AuthenticationFactory.getForSupplier(gpSystem).patientWithAccountIDInWrongFormat(this.patient)

        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Linkage Key not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithALinkageKeyNotInTheExpectedFormat(gpSystem: String) {

        this.patient = Patient.getDefault(gpSystem).copy(linkageKey = INVALID_VALUE)
        AuthenticationFactory.getForSupplier(gpSystem).patientWithLinkageKeyInWrongFormat(this.patient)

        setIm1Request()
    }

    @Given("^I have a (.+) user's IM1 credentials with a Date Of Birth not in the expected format$")
    fun iHaveAnEMISUsersIMCredentialsWithADateOfBirthNotInTheExpectedFormat(gpSystem: String) {

        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = INVALID_VALUE)
        AuthenticationFactory.getForSupplier(gpSystem).patientWithDOBInWrongFormat(patient)

        setIm1Request()
    }

    @Given("^I have a user's IM1 credentials with missing ODS Code$")
    fun iHaveAnEMISUsersIMCredentialsWithMissingODSCode() {
        this.patient = EmisPatients.johnSmith
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have data for an EMIS patient that has already been associated with the application in the GP system$")
    fun iHaveDataForAnEMISPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGPSystem() {
        this.patient = Patient.getDefault("EMIS")

        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis {
            authentication.meApplicationsRequest(patient, createLinkApplicationRequestModel(patient))
                    .respondWithAlreadyLinked()
        }

        setIm1Request()
        setSessionVariable("HttpExceptionExpected").to(true)
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

    @Given("^I have data for a Vision patient that has already been associated with the application in the GP system$")
    fun iHaveDataForAVisionPatientThatHasAlreadyBeenAssociatedWithTheApplicationInTheGPSystem() {

        this.patient = Patient.getDefault("VISION")
        AuthenticationFactoryVision.patientIsAlreadyRegistered(this.patient)

        setIm1Request()
        setSessionVariable("HttpExceptionExpected").to(true)
    }

    @Given("^I have data for a Vision patient with a locked account " +
            "as the account is opened in the Vision application$")
    fun iHaveDataForAVisionPatientThatHasALockedAccount() {

        this.patient = Patient.getDefault("VISION")
        AuthenticationFactoryVision.patientHasALockedAccount(this.patient)

        setIm1Request()
        setSessionVariable("HttpExceptionExpected").to(true)
    }

    private fun setIm1Request() {
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                LinkageKey = patient.linkageKey,
                OdsCode = patient.odsCode,
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
    fun iHaveAnEMISUsersIMCredentialsWithMissingLinkageKey(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        this.im1ConnectionRequest = Im1ConnectionRequest(
                AccountId = patient.accountId,
                OdsCode = patient.odsCode,
                Surname = patient.surname,
                DateOfBirth = patient.dateOfBirth)
    }

    @Given("^I have just logged out$")
    fun iHaveJustLoggedOut() {
        browser.goToApp()
        login.using(EmisMockDefaults.patientEmis)
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
    }

    @Given("^I am logged in as a (.*) user$")
    fun iAmLoggedInTo(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I am logged in as a (.*) user created before Im1 Cache Keys existed$")
    fun iAmLoggedInWithoutIm1CacheKey(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(im1ConnectionToken = null)
        setupAndLogIn(patient, gpSystem)
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

    @Given("^I attempt to log in as a (.*) user with an age under (\\d+)$")
    fun iAmLoggedInToWithAgeUnderMinAge(gpSystem: String, age : Int) {
        val birthdayToday =DateTime.now().minusYears(age);
        val birthdayTomorrow = birthdayToday.plusDays(1)
        val dateOfBirth = birthdayTomorrow.toString(DateTimeFormats.dateWithoutTimeFormat)
        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = dateOfBirth)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("^I attempt to log in as a (.*) user that is (\\d+)$")
    fun iAmLoggedInToWithUserOfAge(gpSystem: String, age : Int) {
        val dateOfBirth = DateTime.now().minusYears(age)
                .toString(DateTimeFormats.dateWithoutTimeFormat)
        this.patient = Patient.getDefault(gpSystem).copy(dateOfBirth = dateOfBirth)
        setupAndLogIn(patient, gpSystem)
    }

    @Given("I attempt to log in as a (.*) user with invalid ODS Code$")
    fun iAttemptToLogInWithInvalidOdsCode(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem).copy(odsCode = "A33224")
        setupAndLogIn(patient, gpSystem)
    }

    private fun setupAndLogIn(patient: Patient, gpSystem: String) {
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)

        browser.goToApp()
        login.using(patient)
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

    @Given("^I want to register for a (.+) account$")
    fun iWantToRegisterForAnAccount(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SuccessfulRegistrationJourney(mockingClient).create(patient, gpSystem)

        browser.goToApp()
    }

    @Given("^no IM1 Connection Token is currently cached$")
    fun im1ConnectionTokensClearedFromTheCache() {
        MongoDBConnection.Im1CacheCollection.clearCache()
    }

    @When("I log out")
    fun iLogOut(){
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
    }

    @When("I use the header link to log out of the website")
    fun iLogOutUsingHeaderLink(){
        webHeader.clickLogout()
    }

    @When("^I create a user session$")
    fun iCreateUserSession() {
        try {
            this.userSessionResponse = WorkerClient().authentication.postSessionConnection(
                    UserSessionRequest(authCode = this.authCode,
                            codeVerifier = this.codeVerifier!!,
                            redirectUrl = Config.instance.cidRedirectUri))
        } catch (httpException: NhsoHttpException) {
            setErrorResponse(httpException)
        }
    }

    @When("^I register the user's IM1 credentials$")
    fun iRegisterAUsersIMCredentials() {
        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postIm1Connection(this.im1ConnectionRequest!!)
            setSessionVariable(Im1ConnectionResponse::class).to(result)
            this.im1ConnectionResponse = result
        } catch (httpException: NhsoHttpException) {
            setErrorResponse(httpException)
        }
    }

    private fun setErrorResponse(errorResponse: NhsoHttpException) {
        this.errorResponse = errorResponse
        setSessionVariable("HttpException").to(errorResponse)
    }

    @When("I log in again")
    fun iLogInAgain(){
        val patient = SerenityHelpers.getPatient()
        login.using(patient)
        home.waitForLoginToCompleteSuccessfully()
    }

    @When("^I browse to the page at (.*)$")
    fun iBrowseToPageAt(url: String) {
        this.currentUrl = nav.browseToPage(url)
    }

    @When("^I browse to the pages at the following urls I see the (.*) page$")
    fun iBrowseToPageAtXAndSeeTheXPage(page: String, table: DataTable) {
        val lowerPage = page.toLowerCase()
        for(row in table.raw()) {
            val fullUrl = Config.instance.url + row.get(0)
            browser.browseTo(fullUrl)
            this.currentUrl = fullUrl

            when (lowerPage) {
                "login" -> {
                    login.loginPage.shouldBeDisplayed()
                }
                "home" -> {
                    home.assertHeaderVisible()
                }
                "relevant" -> {
                    browser.shouldHaveUrl(Config.instance.url + row.get(1))
                }
            }
        }
    }

    @When("^I select to create an account$")
    fun iClickCreateAccountButton() {
        login.loginPage.loginOrCreateAccountButton.click()
    }

    @When("^I have completed (.+) account creation$")
    fun iCreateAnAccount(gpSystem: String) {
        this.patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(this.patient)
        iWantToRegisterForAnAccount(gpSystem)
        login.loginPage.createAccount(patient)
    }


    @When("^I sign out")
    @Throws(Exception::class)
    fun iClickTheSignOutButton() {
        navHeader.clickMyAccount()
        myAccount.signOutButton.click()
        browser.waitUntilSignoutCompletes()
    }

    @When("^I POST to IM1 Connection to register the user$")
    fun iPostToIm1Connection() {
        val linkingInformationExample =
                Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        this.im1ConnectionRequest = Im1ConnectionRequest(
                linkingInformationExample.accountId,
                linkingInformationExample.linkageKey,
                linkingInformationExample.odsCode,
                linkingInformationExample.surname,
                linkingInformationExample.dateOfBirth
        )
        val gpSystem = GlobalSerenityHelpers.GP_SYSTEM.getOrFail<String>()
        this.patient = Patient.getDefault(gpSystem).copy(
                accountId = linkingInformationExample.accountId,
                linkageKey = linkingInformationExample.linkageKey,
                odsCode = linkingInformationExample.odsCode,
                surname = linkingInformationExample.surname,
                dateOfBirth = linkingInformationExample.dateOfBirth,
                nhsNumbers = arrayListOf(linkingInformationExample.nhsNumber)
        )
        CitizenIdSessionCreateJourney(mockingClient).createFor(this.patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(this.patient)
        SuccessfulRegistrationJourney(mockingClient).create(this.patient, gpSystem)
        iRegisterAUsersIMCredentials()
        Assert.assertNotNull("IM1 Connection Token post failed: $errorResponse", this.im1ConnectionResponse)
    }

    @When("^I have logged in with the user associated with the IM1 Connection Token$")
    fun loggedInWithTheUserAssociatedWithTheIm1ConnectionToken() {
        Assert.assertNotNull(Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession))
    }

    @Then("^I receive a response$")
    fun iReceiveAResponse() {
        val responses = arrayListOf(this.userSessionResponse, this.im1ConnectionResponse).filter { it != null }
        Assert.assertEquals("No responses found.  Errors: ${this.errorResponse}", responses.size, 1)
        Assert.assertEquals(this.errorResponse, null)
    }

    @Then("^the response has a name$")
    fun theResponseHasAName() {
        Assert.assertEquals(EmisMockDefaults.patientEmis.formattedFullName(),
                this.userSessionResponse?.userSessionResponseBody?.name)
    }

    @Then("^the response has a session timeout$")
    fun theResponseHasASessionLength() {
        checkNotNull(this.userSessionResponse?.userSessionResponseBody?.sessionTimeout)
    }

    @Then("^the response has service journey rules$")
    fun theResponseHasServiceJourneyRules() {
        val serviceJourneyRules = this.userSessionResponse!!.userSessionResponseBody.serviceJourneyRules
        Assert.assertEquals("Service Journey Rules Appointments provider",
                AppointmentsProvider.im1,
                serviceJourneyRules.journeys.appointments.provider)
        Assert.assertEquals("Service Journey Rules CdssAdmin provider",
                CdssProvider.eConsult,
                serviceJourneyRules.journeys.cdssAdmin.provider)
        Assert.assertEquals("Service Journey Rules CdssAdvice provider",
                CdssProvider.eConsult,
                serviceJourneyRules.journeys.cdssAdvice.provider)
    }

    @Then("^the response has the expected connection token$")
    fun theResponseHasTheExpectedConnectionToken() {
        val result = this.im1ConnectionResponse

        val expectedIm1ConnectionToken = this.patient.im1ConnectionToken

        val actualIm1ConnectionToken = GsonFactory.asPascal.fromJson<Im1ConnectionToken>(
                result?.connectionToken,
                Im1ConnectionToken::class.java
        )

        Assert.assertEquals(expectedIm1ConnectionToken, actualIm1ConnectionToken)
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
        Assert.assertTrue(cookieParams.toString(),
                !cookieParams["httponly"].isNullOrEmpty() && cookieParams["httponly"]!!.toBoolean())
    }

    private fun retrieveCookie(result: UserSessionResponse): HashMap<String, String> {
        checkNotNull(result.userSessionResponseCookie.cookie)
        Assert.assertFalse("Cookie value is empty or null",
                result.userSessionResponseCookie.cookie.value.isNullOrEmpty())
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

    @Then("I can cycle through the header links")
    fun iLCycleTheHeaderLinks(){
        val linksToFollow = arrayListOf(
                { followSymptomsHeaderLink()},
                { followAppointmentHeaderLink() },
                { followPrescriptionsHeaderLink() },
                { followMedicalRecordHeaderLink()},
                { followAccountHeaderLink() }
        )

        linksToFollow.forEachIndexed { index, link ->
            if (index != linksToFollow.size)
            link.invoke()
        }

    }

    private fun followAppointmentHeaderLink() {
        webHeader.clickAppointmentsPageLink()
        webHeader.isPageTitleCorrect("Appointments")
    }

    private fun followSymptomsHeaderLink() {
        webHeader.clickSymptomsPageLink()
        webHeader.isPageTitleCorrect("Symptoms")
    }

    private fun followPrescriptionsHeaderLink() {
        webHeader.clickPrescriptionsPageLink()
        webHeader.isPageTitleCorrect("Repeat prescriptions")
    }

    private fun followMedicalRecordHeaderLink() {
        webHeader.clickMyRecordPageLink()
        webHeader.isPageTitleCorrect("My medical record")
    }

    private fun followMoreHeaderLink() {
        webHeader.clickMorePageLink()
        webHeader.isPageTitleCorrect("More")
    }

    private fun followAccountHeaderLink() {
        webHeader.clickAccount()
        webHeader.isPageTitleCorrect("Account")
    }

    @Then("^I see an error message informing me I cannot log in as I am under the minimum age$")
    fun iSeeAnErrorMessageInformingMeICannotLogInAsIAmUnderSixteen() {
        serviceUnavailablePage.assertIsPresent("You are too young to use the NHS App",
                "Due to legal restrictions, you cannot use the NHS App until you are at least 13 years old. " +
                        "You can still call or visit your GP surgery to access your NHS services. " +
                        "For urgent medical advice, call 111.")
    }

    @Then("^I see an error message informing me I cannot log in$")
    fun iSeeAnErrorMessageInformingMeICannotLogIn() {
        serviceUnavailablePage.assertIsPresent("You cannot currently use this service",
                "You can still call or visit your GP surgery to access your " +
                "NHS services. For urgent medical advice, call 111.")
    }

    @Then("^I get (?:a|an) \"(.*)\" error")
    fun thenIReceiveAnError(expectedStatusCode: String) {
        val code = httpStatusCodeTransform(expectedStatusCode)
        checkNotNull(this.errorResponse)
        Assert.assertEquals(code, this.errorResponse?.statusCode)
    }

    private fun httpStatusCodeTransform(errorName: String): Int? {
        return _errorMapping[errorName.toLowerCase()]
                ?: throw IllegalArgumentException("Could not identify an HTTP status code named: $errorName")
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

    @Then("^I see the home page$")
    fun iSeeTheHomePage() {
        home.assertHeaderVisible()
    }

    @Then("^I see the login page$")
    fun iSeeTheLoginPage() {
        login.loginPage.shouldBeDisplayed()
    }

    @Then("^I see a welcome message$")
    fun iSeeAWelcomeMessageFor() {
        val patient = SerenityHelpers.getPatient()
        home.assertWelcomeMessageShownFor(patient)
    }

    @Then("I see the patient details of name, date of birth and NHS number$")
    fun iSeePatientDetails() {
        val patient = SerenityHelpers.getPatient()
        val regex = """${'^'}${'['}0-9${']'}${'{'}10${'}'}${'$'}""".toRegex()
        Assert.assertTrue("Test Setup Incorrect: Patient must have unformatted nhs number " +
                "to check front end formatting. Regex: '$regex' Number: '${patient.nhsNumbers.first()}' ",
                regex.containsMatchIn(patient.nhsNumbers.first()))
        home.assertPatientDetailsShownFor(patient)
    }

    @Then("^I see the home page header$")
    fun iSeeHeader() {
        home.assertHeaderVisible()
    }

    @Then("^I see the navigation menu$")
    fun iSeeNavbar() {
        nav.assertVisible()
    }

    @Then("^I do not see the menu bar$")
    @Throws(Exception::class)
    fun iDoNotSeeMenuBar() {
        login.loginPage.assertMenuIsNotVisible()
    }

    @Then("^the user login details are cleared from cookies$")
    @Throws(Exception::class)
    fun theUserLoginDetailsAreClearedFromCookies() {
        browser.checkLoginDetailsAreReset()
    }

    @Then("^I see the CID create an account page$")
    @Throws(Exception::class)
    fun thenISeeTheCIDCreateAnAccountPage() {
        accountCreation.assertPageIsVisible()
    }

    @Then("^I see the signed in home page$")
    @Throws(Exception::class)
    fun thenISeeTheSignedInHomePage() {
        navHeader.assertHomePageHeaderVisible()
    }

    @Then("^the IM1 Connection Token is in the cache$")
    fun theIm1ConnectionTokenIsInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(1)
    }

    @Then("^the IM1 Connection Token is no longer in the cache$")
    fun theIm1ConnectionTokenIsNoLongerInTheCache() {
        MongoDBConnection.Im1CacheCollection.assertNumberOfDocuments(0)
    }
}
