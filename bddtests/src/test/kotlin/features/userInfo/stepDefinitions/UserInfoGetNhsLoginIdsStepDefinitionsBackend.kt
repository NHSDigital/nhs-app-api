package features.userInfo.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.getOrFail
import utils.set
import utils.setSingleton
import worker.models.userInfo.GetUserInfoV2Response
import worker.models.userInfo.UserInfoV2Response
import java.util.UUID
import kotlin.collections.ArrayList

class UserInfoGetNhsLoginIdsStepDefinitionsBackend {

    @Given("^I am an api user wishing to get a list of NhsLoginIds$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIds(){
        val factory = UserInfoFactory()
        val odsCode = "OdsCode"
        val nhsNumber = "9998889999"

        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = UserInfoSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set("$nhsNumber-$runUuid")
        UserInfoSerenityHelpers.TARGET_ODSCODE.set("$odsCode-$runUuid")

        factory.setUpMultipleExistingUserInfoRecords(odsCode, nhsNumber, runUuid)
    }

    @Given("^I am an api user wishing to get a list of NhsLoginIds that are linked to a given ods code$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsLinkedToOdsCode() {
        val factory = UserInfoFactory()
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
    }

    @Given("^I am an api user wishing to get NhsLoginIds, but the ods code I am using is not linked to any records$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsButTheOdsCodeIAmUsingIsNotLinkedToRecords() {
        val factory = UserInfoFactory()
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
        UserInfoSerenityHelpers.TARGET_ODSCODE.set("RandomOdsCode")
    }

    @Given("^I am an api user wishing to get NhsLoginIds, but the nhs number I am using is not linked to any records$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsButThenhsNumberIAmUsingIsNotLinkedToRecords() {
        val factory = UserInfoFactory()
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set("9999999998")
    }

    @Given("^I am an api user wishing to get a list of NhsLoginIds that are linked to a given nhs number")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsLinkedToNhsNumber() {
        val factory = UserInfoFactory()
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        factory.setUpMultipleExistingUserInfoRecordsLinkedToNhsNumber()
    }

    @When("^I get user info details based on an ods code$")
    fun iGetUserInfoDetailsBasedOnAnOdsCode() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = odsCode, nhsNumber = null)
    }

    @When("^I get user info details based on an ods code from V2 endpoint$")
    fun iGetUserInfoDetailsV2BasedOnAnOdsCode() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        UserInfoApi.getUserInfoV2(odsCode = odsCode, nhsNumber = null)
    }

    @When("^I get user info details without ods code or nhs number$")
    fun iGetUserInfoDetailsWithoutOdsCodeOrNhsNumber() {
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = null)
    }

    @When("^I get user info details V2 without ods code or nhs number$")
    fun iGetUserInfoDetailsV2WithoutOdsCodeOrNhsNumber() {
        UserInfoApi.getUserInfoV2(odsCode = null, nhsNumber = null)
    }

    @When("^I get user info details with ods code and nhs number$")
    fun iGetUserInfoDetailsWithOdsCodeAndNhsNumber() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()

        UserInfoApi.getUserInfo(odsCode = odsCode, nhsNumber = nhsNumber)
    }

    @When("^I get user info details V2 with ods code and nhs number$")
    fun iGetUserInfoDetailsV2WithOdsCodeAndNhsNumber() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()

        UserInfoApi.getUserInfoV2(odsCode = odsCode, nhsNumber = nhsNumber)
    }

    @When("^I get user info details based on an nhs number$")
    fun iGetUserInfoDetailsBasedOnAnNhsNumber() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = nhsNumber)
    }

    @When("^I get user info details based on an nhs number from V2 endpoint$")
    fun iGetUserInfoDetailsV2BasedOnAnNhsNumber() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfoV2(odsCode = null, nhsNumber = nhsNumber)
    }

    @When("^I get user info details based on an nhs number without the api key$")
    fun iGetUserInfoDetailsBasedOnAnNhsNumberWithoutTheApiKey() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = nhsNumber, includeApiKey = false)
    }

    @When("^I get user info details V2 based on an nhs number without the api key$")
    fun iGetUserInfoDetailsV2BasedOnAnNhsNumberWithoutTheApiKey() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfoV2(odsCode = null, nhsNumber = nhsNumber, includeApiKey = false)
    }

    @Then("^I receive a list of NhsLoginIds from user info endpoint$")
    fun iReceiveMyDetailsFromTheUserInfoEndpoint() {
        val responseNhsLoginIds = UserInfoSerenityHelpers.GET_USER_INFO_NHSLOGINIDS_RESPONSE
                .getOrFail<Array<String>>()
        val expectedNhsLoginIds = UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS
                .getOrFail<ArrayList<String>>().toTypedArray()
        Assert.assertArrayEquals("NhsLoginIds.", expectedNhsLoginIds, responseNhsLoginIds)
    }

    @Then("^I receive an empty list of NhsLoginIds from user info endpoint$")
    fun iReceiveAnEmptyListOfNhsLoginIdsFromTheUserInfoEndpoint() {
        val responseNhsLoginIds = UserInfoSerenityHelpers.GET_USER_INFO_NHSLOGINIDS_RESPONSE
                .getOrFail<Array<String>>()
        Assert.assertTrue("Expected NhsLoginIds to be empty", responseNhsLoginIds.isEmpty())
    }

    @Then("^I receive a list of users from user info V2 endpoint$")
    fun iReceiveListOfUsersFromTheUserInfoV2Endpoint() {
        val responseUsers = UserInfoSerenityHelpers.GET_USER_INFO_V2_RESPONSE
            .getOrFail<GetUserInfoV2Response>().users
        val expectedUsers = UserInfoSerenityHelpers.EXPECTED_USERINFOS
            .getOrFail<List<UserInfoV2Response>>()
        assertUsers(expectedUsers, responseUsers)
    }

    @Then("^I receive an empty list of users from user info V2 endpoint$")
    fun iReceiveAnEmptyListOfUsersFromTheUserInfoV2Endpoint() {
        val responseUsers = UserInfoSerenityHelpers.GET_USER_INFO_V2_RESPONSE
            .getOrFail<GetUserInfoV2Response>()
        Assert.assertTrue("Expected NhsLoginIds to be empty", responseUsers.users.isEmpty())
    }

    private fun assertUsers(expectedUsers: List<UserInfoV2Response>, responseUsers: List<UserInfoV2Response>) {
        Assert.assertEquals("Number Of Users", expectedUsers.count(), responseUsers.count())
        for (x in 0 until expectedUsers.count()){
            assertEquals(expectedUsers[x],responseUsers[x] )
        }
    }

    fun assertEquals(expected: UserInfoV2Response, actual: UserInfoV2Response) {
        Assert.assertEquals("User NhsLoginId", expected.nhsLoginId, actual.nhsLoginId)
        Assert.assertEquals("User OdsCode", expected.odsCode, actual.odsCode)
        Assert.assertEquals("User NhsNumber", expected.nhsNumber, actual.nhsNumber)
        Assert.assertEquals("User LastLogin", expected.lastLogin, actual.lastLogin)
    }
}
