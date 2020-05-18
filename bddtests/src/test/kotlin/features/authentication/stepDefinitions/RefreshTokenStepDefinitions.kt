package features.authentication.stepDefinitions

import constants.SessionConstants
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.AccessTokenBuilder
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set
import worker.WorkerClient
import worker.models.authorization.RefreshAccessTokenResponse
import javax.servlet.http.Cookie

class RefreshTokenStepDefinitions {
    private val mockingClient = MockingClient.instance

    @Given("^I am an API user who wishes to refresh their access token$")
    fun givenIAmAnApiUserWhoWishesToRefreshTheirAccessToken() {
        val patient = setupPatient()
        val refreshedAccessToken = AccessTokenBuilder().getSignedToken(patient).serialize()
        AuthorizationSerenityHelpers.REFRESHED_ACCESS_TOKEN.set(refreshedAccessToken)

        mockingClient.forCitizenId.mock {
            refreshTokenRequest(SessionConstants.RefreshToken)
                    .respondWithSuccess(accessToken = refreshedAccessToken)
        }
    }

    @Given("^I am an API user who wishes to refresh their access token but NhsLogin will fail$")
    fun givenIAmAnApiUserWhoWishesToRefreshTheirAccessTokenAndNhsLoginFails() {
        setupPatient()
        mockingClient.forCitizenId.mock {
            refreshTokenRequest(SessionConstants.RefreshToken)
                    .respondWithServerError()
        }
    }

    @When("^I call the refresh access token endpoint$")
    fun whenICallTheRefreshAccessTokenEndpoint() {
            val workerClient = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            val sessionCookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
            val result = workerClient.authentication.postRefreshAccessToken(sessionCookie)
            AuthorizationSerenityHelpers.REFRESH_TOKEN_RESPONSE.set(result)
    }

    @Then("^I receive a refreshed access token$")
    fun thenIReceiveARefreshedAccessToken() {
        val expectedAccessToken = AuthorizationSerenityHelpers.REFRESHED_ACCESS_TOKEN.getOrFail<String>()
        val refreshedResponse =
                AuthorizationSerenityHelpers.REFRESH_TOKEN_RESPONSE.getOrNull<RefreshAccessTokenResponse>()
        Assert.assertNotNull("Refresh access token response", refreshedResponse)
        Assert.assertEquals("Expected access token", expectedAccessToken, refreshedResponse?.token)
    }

    private fun setupPatient(): Patient {
        val gpSystem = Supplier.EMIS
        SerenityHelpers.setGpSupplier(gpSystem)

        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem).createFor(patient)
        return patient
    }
}
