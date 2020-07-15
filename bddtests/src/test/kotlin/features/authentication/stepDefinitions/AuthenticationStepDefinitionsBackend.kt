package features.authentication.stepDefinitions

import config.Config
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.MockingClient
import mocking.citizenId.models.signingKeys.SucceededResponse
import mocking.defaults.EmisMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.emis.models.AssociationType
import models.Patient
import org.apache.commons.lang3.StringUtils
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse
import worker.models.serviceJourneyRules.AppointmentsProvider
import worker.models.serviceJourneyRules.CdssProvider
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse

class AuthenticationStepDefinitionsBackend {

    private val mockingClient = MockingClient.instance

    private var authCode: String? = EmisMockDefaults.patientEmis.authCode
    private var codeVerifier: String? = EmisMockDefaults.patientEmis.codeVerifier
    private val associationType = AssociationType.Self

    @Given("^I have a valid authCode and codeVerifier$")
    fun iHaveValidAuthCodeAndCodeVerifier() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
    }

    @Given("^I have incomplete OAuth details$")
    fun iHaveIncompleteOAuthDetails() {
        this.authCode = null
    }

    @Given("^I have invalid OAuth details$")
    fun iHaveInvalidOAuthDetails() {
        mockingClient.forCitizenId.mock {
            tokenRequest(codeVerifier!!, authCode, GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail())
                    .respondWithBadRequest()
        }
    }

    @Given("^I have valid OAuth details and the CID tokens endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDTokenEndpointFails() {
        val supplier = Supplier.EMIS
        val patient = Patient.getDefault(supplier)
        SerenityHelpers.setPatient(patient)
        mockingClient.forCitizenId.mock {
            tokenRequest(codeVerifier!!,
                    authCode,
                    GlobalSerenityHelpers.LOGIN_REDIRECT_URI.getOrFail())
                    .respondWithServerError()
        }
        mockingClient.forCitizenId.mock {
            userInfoRequest(patient.accessToken).respondWithServerError()
        }
        mockingClient.forCitizenId.mock {
            signingKeyRequest().respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        val supplier = Supplier.EMIS
        CitizenIdSessionCreateJourney().createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis.mock { authentication.endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis.mock {
            authentication.sessionRequest(Patient.getDefault(supplier))
                    .respondWithSuccess(Patient.getDefault(supplier), associationType)
        }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        CitizenIdSessionCreateJourney().createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis.mock {
            authentication.endUserSessionRequest()
                    .respondWithSuccess(EmisMockDefaults.patientEmis.endUserSessionId)
        }
        mockingClient.forEmis.mock {
            authentication.sessionRequest(EmisMockDefaults.patientEmis).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and (.*) is not available$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        CitizenIdSessionCreateJourney().createFor(Patient.getDefault(supplier))
        AuthenticationFactory.getForSupplier(supplier).validOAuthDetailsAndGpSystemUnavailable()
    }

    @Given("^I have valid OAuth details and (.*) returns with an incomplete response$")
    fun iHaveValidOAuthDetailsAndGpSystemReturnsAnIncompleteResponse(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        CitizenIdSessionCreateJourney().createFor(Patient.getDefault(supplier))
        AuthenticationFactory.getForSupplier(supplier).patientWithIncompleteResponse(Patient.getDefault(supplier))
    }

    @Given("^I have valid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveValidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        CitizenIdSessionCreateJourney().createFor(Patient.getDefault(supplier))
        AuthenticationFactory.getForSupplier(supplier).validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in (\\d+) seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInXSeconds(gpSystem: String, delayBySeconds: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        CitizenIdSessionCreateJourney().createFor(Patient.getDefault(supplier))

        AuthenticationFactory.getForSupplier(supplier)
                .validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds.toLong())
    }

    @When("^I create a user session$")
    fun iCreateUserSession() {
        val response = WorkerClient().authentication.postSessionConnection(
                UserSessionRequest(authCode = this.authCode,
                        codeVerifier = this.codeVerifier!!,
                        redirectUrl = Config.instance.cidRedirectUri))
        AuthenticationSerenityHelpers.USER_SESSION_RESPONSE.set(response)
    }

    @Then("^I receive a response$")
    fun iReceiveAResponse() {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        val im1ConnectionResponse = AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE
                .getOrNull<Im1ConnectionResponse?>()
        val responses = arrayListOf(userSessionResponse, im1ConnectionResponse).filter { it != null }
        val errorResponse = SerenityHelpers.getHttpException()
        Assert.assertEquals("No responses found.  Errors: ${errorResponse}", responses.size, 1)
        Assert.assertEquals(errorResponse, null)
    }

    @Then("^the response has a name$")
    fun theResponseHasAName() {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        Assert.assertEquals(EmisMockDefaults.patientEmis.formattedFullName(),
                userSessionResponse?.userSessionResponseBody?.name)
    }

    @Then("^the response has a name for the (.*) patient with no title$")
    fun theResponseHasANameWithNoTitle(gpSystem: String) {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        val supplier = Supplier.valueOf(gpSystem)
        val patient = Patient.getDefault(supplier)
        Assert.assertEquals(patient.formattedFullName(false),
                userSessionResponse?.userSessionResponseBody?.name)
    }

    @Then("^the response has a session timeout$")
    fun theResponseHasASessionLength() {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        checkNotNull(userSessionResponse?.userSessionResponseBody?.sessionTimeout)
    }

    @Then("^the response has service journey rules$")
    fun theResponseHasServiceJourneyRules() {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        val serviceJourneyRules = userSessionResponse!!.userSessionResponseBody.serviceJourneyRules
        Assert.assertNotNull(serviceJourneyRules)

        if(SerenityHelpers.getGpSupplier() === Supplier.EMIS ||
                SerenityHelpers.getGpSupplier() === Supplier.TPP) {
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

    }

    @Then("^the cookie contains a session guid with http-only$")
    fun iReceiveCookieWithSessionIdHttpOnly() {
        val userSessionResponse = AuthenticationSerenityHelpers.USER_SESSION_RESPONSE
                .getOrNull<UserSessionResponse>()
        val cookieParams = retrieveCookie(userSessionResponse!!)
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
}
