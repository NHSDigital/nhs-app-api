package features.userInfo.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserAndInfo
import org.junit.Assert
import utils.SerenityHelpers
import worker.models.userInfo.UserInfoResponse
import worker.models.userInfo.UserAndInfoResponse

class UserInfoPostStepDefinitionsBackend {

    @Given("^I am an api user wishing to submit their details to the user info endpoint$")
    fun iAmAnApiUserWishingToSubmitTheirDetailsToTheUserInfoEndpoint() {
        val factory = UserInfoFactory()
        factory.setUpUser()
    }

    @When("^I post to the user info endpoint$")
    fun iPostToTheUserInfoEndpoint() {
        val authToken = SerenityHelpers.getPatient().accessToken
        UserInfoApi.postUserInfoWithGivenToken(authToken)
    }

    @When("^I post to the user info endpoint without an access token$")
    fun iPostToTheUserInfoEndpointWithoutAuthToken() {
        UserInfoApi.postUserInfoWithGivenToken(null)
    }

    @Then("^posting to the user info endpoint with an invalid access token will return an Unauthorised error$")
    fun iPostToTheUserInfoEndpointWithInvalidAuthToken() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            UserInfoApi.postUserInfoWithGivenToken(authToken = accessToken)
        }
    }

    @Then("^my details are available in the user info repository$")
    fun myDetailsAreAvailableInTheUserInfoRepository() {
        val patient = SerenityHelpers.getPatient()
        val expectedUserInfo = UserAndInfoResponse(
                patient.subject,
                UserInfoResponse(patient.odsCode,
                        patient.nhsNumbers.first(),
                        false),
                ""
        )
        assertSingleRecordInUserInfoRepository(expectedUserInfo)
    }

    @Then("^there are no details available in the user info repository$")
    fun thereAreNoDetailsAvailableInTheUserInfoRepository() {
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(0)
    }

    private fun assertSingleRecordInUserInfoRepository(expectedUserInfo: UserAndInfoResponse) {
        val userInfo = assertNumberOfUserInfoInRepository(1)
        val registeredUserInfo = userInfo.single()
        assertFoundUserInfoRecord(expectedUserInfo, registeredUserInfo)
    }

    private fun assertFoundUserInfoRecord(expectedUserInfo: UserAndInfoResponse,
                                          registeredUserInfo: MongoRepositoryUserAndInfo) {
        Assert.assertEquals("NHS Login ID.", expectedUserInfo.nhsLoginId, registeredUserInfo.NhsLoginId)
        Assert.assertEquals("ODS Code.", expectedUserInfo.info.odsCode, registeredUserInfo.Info.OdsCode)
        Assert.assertEquals("NHS Number.", expectedUserInfo.info.nhsNumber, registeredUserInfo.Info.NhsNumber)
        Assert.assertEquals("Beta Tester.", expectedUserInfo.info.betaTester, registeredUserInfo.Info.BetaTester)
    }

    private fun assertNumberOfUserInfoInRepository(expectedNumber: Int): List<MongoRepositoryUserAndInfo> {
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(expectedNumber)
        val userInfo = MongoDBConnection.UserInfoCollection
                .getValues<MongoRepositoryUserAndInfo>(MongoRepositoryUserAndInfo::class.java)
        Assert.assertNotNull("User info", userInfo)
        Assert.assertEquals("Number of user info documents", expectedNumber, userInfo.count())
        return userInfo
    }
}
