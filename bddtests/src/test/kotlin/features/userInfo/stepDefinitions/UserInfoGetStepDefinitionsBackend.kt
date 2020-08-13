package features.userInfo.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import features.sharedSteps.InvalidAccessTokenTester
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.models.userInfo.UserAndInfoResponse

class UserInfoGetStepDefinitionsBackend {

    @Given("^I am an api user with stored details wishing to get my details$")
    fun iAmAnApiUserWithStoredDetailsWishingToGetMyDetails() {
        val factory = UserInfoFactory()
        factory.setUpUser()
        factory.setUpExistingUserInfo()
    }

    @Given("^I am an api user without stored details wishing to get my details")
    fun iAmAnApiUserWithoutStoredDetailsWishingToGetMyDetails() {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, SJRJourneyType.USER_INFO_DISABLED)

        val factory = UserInfoFactory()
        factory.setUpUser(patient)
    }

    @When("^I get user info details from the user info endpoint$")
    fun iGetUserInfoDetailsFromTheUserInfoEndpoint() {
        val authToken = SerenityHelpers.getPatient().accessToken
        UserInfoApi.getUserInfoWithGivenToken(authToken)
    }

    @When("^I get user info details from the user info endpoint without an access token$")
    fun iGetTheUserInfoEndpointWithoutAuthToken() {
        UserInfoApi.getUserInfoWithGivenToken(null)
    }

    @Then("^getting details from the user info endpoint with an invalid access token will return an " +
            "Unauthorised error$")
    fun iGetTheUserInfoEndpointWithInvalidAuthToken() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            UserInfoApi.getUserInfoWithGivenToken(authToken = accessToken)
        }
    }

    @Then("^I receive my details from the user info endpoint$")
    fun iReceiveMyDetailsFromTheUserInfoEndpoint() {
        val responseUserInfo = UserInfoSerenityHelpers.GET_USER_INFO_RESPONSE.getOrFail<UserAndInfoResponse>()
        val patient = SerenityHelpers.getPatient()
        Assert.assertEquals("Expected NHS Login ID.", patient.subject, responseUserInfo.nhsLoginId)
        Assert.assertEquals("Expected ODS Code.", patient.odsCode, responseUserInfo.info.odsCode)
        Assert.assertEquals("Expected NHS Number.",  patient.nhsNumbers.first(), responseUserInfo.info.nhsNumber)
        Assert.assertEquals("Expected Beta Tester.", false, responseUserInfo.info.betaTester)
    }
}
