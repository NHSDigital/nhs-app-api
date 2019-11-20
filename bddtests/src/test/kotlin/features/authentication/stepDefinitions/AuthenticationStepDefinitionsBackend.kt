package features.authentication.stepDefinitions

import config.Config
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
import utils.SerenityHelpers
import utils.getOrNull
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionResponse
import worker.models.serviceJourneyRules.AppointmentsProvider
import worker.models.serviceJourneyRules.CdssProvider
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse

class AuthenticationStepDefinitionsBackend {

    val mockingClient = MockingClient.instance

    private var authCode: String? = EmisMockDefaults.patientEmis.cidUserSession.authCode
    private var codeVerifier: String? = EmisMockDefaults.patientEmis.cidUserSession.codeVerifier
    private val associationType = AssociationType.Self

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
            tokenRequest(codeVerifier!!, authCode)
                    .respondWithBadRequest()
        }
    }

    @Given("^I have valid OAuth details and the CID tokens endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDTokenEndpointFails() {
        val gpSystem = "EMIS"
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        mockingClient.forCitizenId {
            tokenRequest(codeVerifier!!, authCode).respondWithServerError()
        }
        mockingClient.forCitizenId {
            userInfoRequest(patient.accessToken).respondWithServerError()
        }
        mockingClient.forCitizenId {
            signingKeyRequest().respondWithSuccess(SucceededResponse(listOf(Config.keyStore.publicJwk.toJSONObject())))
        }
        SessionCreateJourneyFactory.getForSupplier("EMIS", mockingClient).createFor(patient)
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis { authentication.endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis {
            authentication.sessionRequest(Patient.getDefault("EMIS"))
                    .respondWithSuccess(Patient.getDefault("EMIS"), associationType)
        }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        CitizenIdSessionCreateJourney(mockingClient).createFor(EmisMockDefaults.patientEmis)
        mockingClient.forEmis {
            authentication.endUserSessionRequest()
                    .respondWithSuccess(Patient.getDefault("EMIS").endUserSessionId)
        }
        mockingClient.forEmis { authentication.sessionRequest(Patient.getDefault("EMIS")).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and (.*) is not available$")
    fun iHaveValidOAuthDetailsAndGpSystemUnavailable(gpSystem: String) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsAndGpSystemUnavailable()
    }

    @Given("^I have valid OAuth details and (.*) returns with an incomplete response$")
    fun iHaveValidOAuthDetailsAndGpSystemReturnsAnIncompleteResponse(gpSystem: String) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).patientWithIncompleteResponse(Patient.getDefault(gpSystem))
    }

    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with (.*)$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithGpSystem(gpSystem: String) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem).validOAuthDetailsCidConnectionTokenFailsToAuthenticate()
    }

    @Given("^I have valid OAuth details and (.*) fails to respond in (\\d+) seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInXSeconds(gpSystem: String, delayBySeconds: Int) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(Patient.getDefault(gpSystem))
        AuthenticationFactory.getForSupplier(gpSystem)
                .validOAuthDetailsAndGpSystemSlowToRespond(delayBySeconds.toLong())
    }

    @When("^I create a user session$")
    fun iCreateUserSession() {
        try {
            val response = WorkerClient().authentication.postSessionConnection(
                    UserSessionRequest(authCode = this.authCode,
                            codeVerifier = this.codeVerifier!!,
                            redirectUrl = Config.instance.cidRedirectUri))
            AuthenticationSerenityHelpers.USER_SESSION_RESPONSE.set(response)
        } catch (httpException: NhsoHttpException) {
            setErrorResponse(httpException)
        }
    }

    private fun setErrorResponse(errorResponse: NhsoHttpException) {
        SerenityHelpers.setHttpException(errorResponse)
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
