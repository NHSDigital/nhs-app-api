package features.sharedStepDefinitions.backend

import cucumber.api.PendingException
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import junit.framework.TestCase.*
import mocking.defaults.MockDefaults
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.apache.commons.lang3.StringUtils
import worker.models.session.UserSessionResponse


class CreateSessionSteps : AbstractSteps() {

    @Given("^I have valid OAuth details and the EMIS session fails to be saved in cache$")
    @Throws(Exception::class)
    fun iHaveValidOAuthDetailsAndEmisSessionFailsToBeSavedInCache() {
        // Write code here that turns the phrase above into concrete actions
        throw PendingException()
    }

    @Then("^a cookie containing a session guid with tls-only$")
    fun iReceiveCookieWithSessionIdTlsOnly() {
        val result = sessionVariableCalled<UserSessionResponse>(UserSessionResponse::class)

        val cookieParams = retrieveCookie(result)
        assertFalse("NHSO-Session-Id is empty or null", cookieParams["NHSO-Session-Id"].isNullOrEmpty())
        assertTrue(cookieParams.toString(), !cookieParams["secure"].isNullOrEmpty() && cookieParams["secure"]!!.toBoolean())
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
