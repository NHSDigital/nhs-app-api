package features.sharedStepDefinitions.backend

import cucumber.api.PendingException
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import junit.framework.TestCase.*

import mocking.emis.models.AssociationType
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.apache.commons.lang3.StringUtils
import org.apache.http.HttpStatus
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.session.UserSessionRequest
import worker.models.session.UserSessionResponse
import java.time.Duration


class CreateSessionSteps : AbstractSteps() {

    private val defaultAssociationType = AssociationType.Self
    private val accessToken = "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICI3WGlNdHVGSHN0MVdtYUppSXdJYVZJZlJOazQ5SzlTNXlWa0thMkZZZHA0In0.eyJqdGkiOiIwNjBlZjM4Yy00YmRlLTQ1ZjgtYjExMy1iOGYzZjVjYWJjOGQiLCJleHAiOjE1MjM5NTg5MTYsIm5iZiI6MCwiaWF0IjoxNTIzOTU4NjE2LCJpc3MiOiJodHRwczovL2tleWNsb2FrLmRldjEuc2lnbmluLm5ocy51ay9jaWNhdXRoL3JlYWxtcy9OSFMiLCJhdWQiOiJuaHMtb25saW5lLXBvYyIsInN1YiI6IjlmZmFhMmNiLTM3MTQtNDMwOS1hZDIyLTlkNGY2YmYwZjUzMSIsInR5cCI6IkJlYXJlciIsImF6cCI6Im5ocy1vbmxpbmUtcG9jIiwiYXV0aF90aW1lIjoxNTIzOTU4NTkyLCJzZXNzaW9uX3N0YXRlIjoiYTRmYmYwN2EtNGM3MS00MTdjLWE2OTYtYmUxNjQ3MDIwOGM0IiwiYWNyIjoiMSIsImFsbG93ZWQtb3JpZ2lucyI6W10sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJ1bWFfYXV0aG9yaXphdGlvbiJdfSwicmVzb3VyY2VfYWNjZXNzIjp7InJlYWxtLW1hbmFnZW1lbnQiOnsicm9sZXMiOlsidmlldy1yZWFsbSIsInZpZXctaWRlbnRpdHktcHJvdmlkZXJzIiwibWFuYWdlLWlkZW50aXR5LXByb3ZpZGVycyIsImltcGVyc29uYXRpb24iLCJyZWFsbS1hZG1pbiIsImNyZWF0ZS1jbGllbnQiLCJtYW5hZ2UtdXNlcnMiLCJxdWVyeS1yZWFsbXMiLCJ2aWV3LWF1dGhvcml6YXRpb24iLCJxdWVyeS1jbGllbnRzIiwicXVlcnktdXNlcnMiLCJtYW5hZ2UtZXZlbnRzIiwibWFuYWdlLXJlYWxtIiwidmlldy1ldmVudHMiLCJ2aWV3LXVzZXJzIiwidmlldy1jbGllbnRzIiwibWFuYWdlLWF1dGhvcml6YXRpb24iLCJtYW5hZ2UtY2xpZW50cyIsInF1ZXJ5LWdyb3VwcyJdfSwiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwibmFtZSI6IlJlYWxtMSBBZG1pbiIsInByZWZlcnJlZF91c2VybmFtZSI6InJlYWxtYWRtaW5AZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6IlJlYWxtMSIsImZhbWlseV9uYW1lIjoiQWRtaW4iLCJlbWFpbCI6InJlYWxtYWRtaW5AZ21haWwuY29tIn0.D2nSVJbZ7M2JZosiC6z-HXx7-Rg1n7w7CCKvWtBzErJVDIedvS5y6syxQnJbtl0yITYM4qP-gN0Ji13qnwu0wjy-NorXvG7BOB5wl2SXekaaphXjv9e6NshQ5SEhyV1hMzfPRqLkZbpETjEOdPiMziG6k8sZCpast3c3diKb96dxjVIOhPayf2P9Z75b-qnegFuV1LkD9mIkGDyA7t5givfouskPSr09EKyxHf_m7kjPipy39cKODgcbsyYpwqAmHYaHJGsqIZYDPTCjvzmkrZOQlGJ_sXAVmxrZY8psUZ7MKeFd4l9xwvfi4N-3FFT5D4_tJq0Yp3RW5Bs3JVc1ig"
    private val bearerToken = "Bearer ".plus(accessToken)
    private val firstName = "Eduardo"
    private val surname = "Crouch"

    val patient = Patient(
            title = "Mr",
            firstName = firstName,
            surname = surname,
            dateOfBirth = "1919-12-24T14:03:15.892Z",
            accountId = "1195029928",
            odsCode = OdsCode,
            connectionToken = ConnectionToken,
            sessionId = "MT4vWCxTKXRYr7fFJWM3wB",
            endUserSessionId = "Ab42ZoP21dT4JE12avEWQ5",
            linkageKey = "KjwzyFSEUAGj4",
            userPatientLinkToken = "3v4DARxCmznF6eiGMQRR2u",
            nhsNumbers = listOf("7174450393")
    )

    val userSessionRequest = UserSessionRequest(
            "xmoKFiYSK6APIDwc7cULOskbmkWD3vD2Map5lIQDdVU",
            "uss.UHLq4ghr4wsANlw5lMdUPFRGji4xlmPSETNewHxUpW0.4dff5848-0cc8-47a1-8eb1-7657b5e9e403.8d4c0a21-6483-4a52-9d47-6bcd737c634e"
    )

    @Given("^I have a valid authCode and codeVerifier for a patient$")
    fun iHaveAValidAuthCodeAndCodeVerifierForAPatient() {
        createSuccessCidStubs()
        createSuccessEmisStubs()
    }

    @Given("^I have incomplete OAuth details$")
    fun iHaveIncompleteOAuthDetails() {
        val incompleteUserSessionRequest = userSessionRequest.copy(authCode = null)
        setSessionVariable(UserSessionRequest::class).to(incompleteUserSessionRequest)

        mockingClient.forCitizenId {
            tokenRequest(incompleteUserSessionRequest.codeVerifier, incompleteUserSessionRequest.authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess()
        }
    }

    @Given("^I have invalid OAuth details$")
    fun iHaveInvalidOAuthDetails() {
        mockingClient.forCitizenId {
            tokenRequest(userSessionRequest.codeVerifier, userSessionRequest.authCode)
                    .respondWithBadRequest()
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess()
        }
        createSuccessEmisStubs()

    }

    @Given("^I have valid OAuth details and the CID tokens endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDTokenEndpointFails() {
        mockingClient.forCitizenId {
            tokenRequest(userSessionRequest.codeVerifier, userSessionRequest.authCode)
                    .respondWithServerError()
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess()
        }
        createSuccessEmisStubs()
    }

    @Given("^I have valid OAuth details and the CID user profile endpoint fails to process the request$")
    fun iHaveValidOAuthDetailsAndCIDUserProfileEndpointFails() {
        mockingClient.forCitizenId {
            tokenRequest(userSessionRequest.codeVerifier, userSessionRequest.authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) { build() }
        }
        createSuccessEmisStubs()
    }

    @Given("^I have valid OAuth details and the EMIS end user session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisUserSessionEndpointFails() {
        createSuccessCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithServerError() }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, defaultAssociationType) }
    }

    @Given("^I have valid OAuth details and the EMIS session endpoint fails to create$")
    fun iHaveValidOAuthDetailsAndEmisSessionEndpointFails() {
        createSuccessCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithServerError() }
    }

    @Given("^I have valid OAuth details and EMIS is unavailable$")
    fun iHaveValidOAuthDetailsAndEmisUnavailable() {
        createSuccessCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithServiceUnavailable() }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, defaultAssociationType) }
    }

    @Given("^I have invalid OAuth details and CID connection token fails to authenticate with emis$")
    fun iHaveInvalidOAuthDetailsAndCIDConnectionTokenFailsToAuthenticateWithEmis() {
        createSuccessCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithForbidden() }
    }

    @Given("^I have valid OAuth details and emis fails to respond in 30 seconds$")
    fun iHaveValidOAuthDetailsAndEmisFailsToRespondInThirtySeconds() {
        createSuccessCidStubs()
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId).delayedBy(Duration.ofSeconds(31)) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, defaultAssociationType) }
    }

    @Given("^I have valid OAuth details and the EMIS session fails to be saved in cache$")
    @Throws(Exception::class)
    fun iHaveValidOAuthDetailsAndEmisSessionFailsToBeSavedInCache() {
        // Write code here that turns the phrase above into concrete actions
        throw PendingException()
    }

    @When("^I create a user session with valid details$|" +
            "^I create a user session with invalid details$")
    fun iCreateUserSessionWithValidDetails() {
        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postSessionConnection(userSessionRequest)
            setSessionVariable(UserSessionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException).also { println("Storing Exception: $httpException") }
        }
    }

    @When("^I create a user session with incomplete details$")
    fun iCreateUserSessionWithIncompleteDetails() {
        try {
            val result = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postSessionConnection(sessionVariableCalled<UserSessionRequest>(UserSessionRequest::class))
            setSessionVariable(UserSessionResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            setSessionVariable("HttpException").to(httpException).also { println("Storing Exception: $httpException") }
        }
    }

    @Then("^I receive a response with given name and family name$")
    fun iReceiveGivenNameAndFamilyName() {
        val result = sessionVariableCalled<UserSessionResponse>(UserSessionResponse::class)

        assertNotNull(result)
        assertEquals(patient.firstName, result.userSessionResponseBody.givenName)
        assertEquals(patient.surname, result.userSessionResponseBody.familyName)
    }

    @Then("^a cookie containing a session guid with http-only$")
    fun iReceiveCookieWithSessionIdHttpOnly() {
        val result = sessionVariableCalled<UserSessionResponse>(UserSessionResponse::class)

        val cookieParams = retrieveCookie(result)
        assertFalse("NHSO-Session-Id is empty or null", cookieParams["NHSO-Session-Id"].isNullOrEmpty())
        assertTrue(cookieParams.toString(), !cookieParams["httponly"].isNullOrEmpty() && cookieParams["httponly"]!!.toBoolean())
    }

    @Then("^a cookie containing a session guid with tls-only$")
    fun iReceiveCookieWithSessionIdTlsOnly() {
        val result = sessionVariableCalled<UserSessionResponse>(UserSessionResponse::class)

        val cookieParams = retrieveCookie(result)
        assertFalse("NHSO-Session-Id is empty or null", cookieParams["NHSO-Session-Id"].isNullOrEmpty())
        assertTrue(cookieParams.toString(), !cookieParams["secure"].isNullOrEmpty() && cookieParams["secure"]!!.toBoolean())
    }

    private fun createSuccessEmisStubs() {
        mockingClient.forEmis { endUserSessionRequest().respondWithSuccess(patient.endUserSessionId) }
        mockingClient.forEmis { sessionRequest(patient).respondWithSuccess(patient, defaultAssociationType) }
    }

    private fun createSuccessCidStubs() {
        mockingClient.forCitizenId {
            tokenRequest(userSessionRequest.codeVerifier, userSessionRequest.authCode)
                    .respondWithSuccess(accessToken)
        }
        mockingClient.forCitizenId {
            userInfoRequest(bearerToken)
                    .respondWithSuccess()
        }
    }

    private fun retrieveCookie(result: UserSessionResponse): HashMap<String, String> {
        assertNotNull("Cookie not found. ", result.userSessionResponseCookie.cookie)
        assertFalse("Cookie value is empty or null", result.userSessionResponseCookie.cookie.value.isNullOrEmpty())
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
