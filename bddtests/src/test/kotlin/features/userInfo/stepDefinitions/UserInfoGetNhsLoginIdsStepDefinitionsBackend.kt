package features.userInfo.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import utils.getOrFail
import utils.set
import utils.setSingleton
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
