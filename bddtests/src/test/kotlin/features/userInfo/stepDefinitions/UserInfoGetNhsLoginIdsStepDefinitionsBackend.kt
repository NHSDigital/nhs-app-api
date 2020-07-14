package features.userInfo.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import org.junit.Assert
import utils.getOrFail
import utils.set

class UserInfoGetNhsLoginIdsStepDefinitionsBackend {

    @Given("^I am an api user wishing to get a list of NhsLoginIds$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIds(){
        val factory = UserInfoFactory()
        val odsCode = "OdsCode"
        val nhsNumber = "9998889999"
        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set(nhsNumber)
        UserInfoSerenityHelpers.TARGET_ODSCODE.set(odsCode)
        factory.setUpMultipleExistingUserInfoRecords(odsCode, nhsNumber)
    }

    @Given("^I am an api user wishing to get a list of NhsLoginIds that are linked to a given ods code$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsLinkedToOdsCode() {
        val factory = UserInfoFactory()
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
    }

    @Given("^I am an api user wishing to get NhsLoginIds, but the ods code I am using is not linked to any records$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsButTheOdsCodeIAmUsingIsNotLinkedToRecords() {
        val factory = UserInfoFactory()
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
        UserInfoSerenityHelpers.TARGET_ODSCODE.set("RandomOdsCode")
    }

    @Given("^I am an api user wishing to get NhsLoginIds, but the nhs number I am using is not linked to any records$")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsButThenhsNumberIAmUsingIsNotLinkedToRecords() {
        val factory = UserInfoFactory()
        factory.setUpMultipleExistingUserInfoRecordsLinkedToOdsCode()
        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set("9999999998")
    }

    @Given("^I am an api user wishing to get a list of NhsLoginIds that are linked to a given nhs number")
    fun iAmAnApiUserWishingToGetAListOfNhsLoginIdsLinkedToNhsNumber() {
        val factory = UserInfoFactory()
        factory.setUpMultipleExistingUserInfoRecordsLinkedToNhsNumber()
    }

    @When("^I get user info details based on an ods code$")
    fun iGetUserInfoDetailsBasedOnAnOdsCode() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = odsCode, nhsNumber = null)
    }

    @When("^I get user info details without ods code or nhs number$")
    fun iGetUserInfoDetailsWithoutOdsCodeOrNhsNumber() {
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = null)
    }

    @When("^I get user info details with ods code and nhs number$")
    fun iGetUserInfoDetailsWithOdsCodeAndNhsNumber() {
        val odsCode = UserInfoSerenityHelpers.TARGET_ODSCODE.getOrFail<String>()
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = odsCode, nhsNumber = nhsNumber)
    }

    @When("^I get user info details based on an nhs number$")
    fun iGetUserInfoDetailsBasedOnAnNhsNumber() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = nhsNumber)
    }

    @When("^I get user info details based on an nhs number without the api key$")
    fun iGetUserInfoDetailsBasedOnAnNhsNumberWithoutTheApiKey() {
        val nhsNumber = UserInfoSerenityHelpers.TARGET_NHSNUMBER.getOrFail<String>()
        UserInfoApi.getUserInfo(odsCode = null, nhsNumber = nhsNumber, includeApiKey = false)
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
}
