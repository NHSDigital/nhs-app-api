package features.userInfo.stepDefinitions

import cosmosSql.CosmosSqlConnection
import cosmosSql.SqlRepositoryUserAndInfo
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import models.IdentityProofingLevel
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserAndInfo
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertNull
import org.junit.Assert.fail
import utils.addToList
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set

class UserInfoPostStepDefinitionsBackend {

    private enum class RecordDetail(val text: String) {
        NhsLoginId("NHS Login ID"),
        OdsCode("ODS Code"),
        NhsNumber("NHS Number")
    }

    @Given("^I am an api user wishing to submit their details to the user info endpoint$")
    fun iAmAnApiUserWishingToSubmitTheirDetailsToTheUserInfoEndpoint() {
        val factory = UserInfoFactory()
        factory.setUpUser()
    }

    @Given("^I am an api user with proof level 5 wishing to submit their details to the user info endpoint$")
    fun iAmAnApiUserWithProofLevel5WishingToSubmitTheirDetailsToTheUserInfoEndpoint() {
        val factory = UserInfoFactory()
        factory.setUpUser { it.copy(identityProofingLevel = IdentityProofingLevel.P5) }
    }

    @When("^I post to the user info endpoint$")
    fun iPostToTheUserInfoEndpoint() {
        val authToken = SerenityHelpers.getPatient().accessToken
        UserInfoApi.postUserInfoWithGivenToken(authToken)

        val deletion = { deleteItemsInSqlContainer() }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
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

    @Then("^a user info record has been created$")
    fun aUserInfoRecordHasBeenCreated() {
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(1)
        val userInfo = MongoDBConnection.UserInfoCollection
                .getValues<MongoRepositoryUserAndInfo>(MongoRepositoryUserAndInfo::class.java)
        assertNotNull("User info", userInfo)
        assertEquals("Number of user info documents", 1, userInfo.count())

        UserInfoSerenityHelpers.MONGO_USER_INFO_RECORD.set(userInfo.single())
    }
    
    @Then("^a user info nhs number record has been created$")
    fun aUserInfoNhsNumberRecordHasBeenCreated() {
        val patient = SerenityHelpers.getPatient()
        val patientNhsNumber = patient.nhsNumbers.firstOrNull()
        val patientNhsLoginId = patient.subject
        val userNhsNumberInfo = CosmosSqlConnection.UserInfoNhsNumberContainer
                .getValueWhere(SqlRepositoryUserAndInfo::class.java, patientNhsLoginId, patientNhsNumber.orEmpty())

        UserInfoSerenityHelpers.SQL_USER_INFO_NHS_NUMBER_RECORD.set(userNhsNumberInfo.item)

    }
    
    @Then("^a user info ods code record has been created$")
    fun aUserInfoOdsCodeRecordHasBeenCreated() {
        val patient = SerenityHelpers.getPatient()
        val patientOdsCode = patient.odsCode
        val patientNhsLoginId = patient.subject
        val userOdsCodeInfo = CosmosSqlConnection.UserInfoOdsCodeContainer
            .getValueWhere(SqlRepositoryUserAndInfo::class.java, patientNhsLoginId, patientOdsCode)

        UserInfoSerenityHelpers.SQL_USER_INFO_ODS_CODE_RECORD.set(userOdsCodeInfo.item)
    }

    @Then("^the user info record will have my (.*)$")
    fun theUserInfoRecordWillHaveMyDetail(detail: String) {
        val patient = SerenityHelpers.getPatient()
        val userAndInfoRecord = UserInfoSerenityHelpers.MONGO_USER_INFO_RECORD.getOrFail<MongoRepositoryUserAndInfo>()

        val expected = getExpectedValue(patient, detail)
        val recorded = getRecordValue(userAndInfoRecord, detail)

        assertEquals("$detail.", expected, recorded)
    }

    @Then("^the user info nhs number record will have my (.*)$")
    fun theUserInfoNhsNumberRecordWillHaveMyDetail(detail: String) {
        val patient = SerenityHelpers.getPatient()
        val userAndInfoRecord = UserInfoSerenityHelpers
            .SQL_USER_INFO_NHS_NUMBER_RECORD.getOrFail<SqlRepositoryUserAndInfo>()

        val expected = getExpectedValue(patient, detail)
        val recorded = getSqlRecordValue(userAndInfoRecord, detail)

        assertEquals("$detail.", expected, recorded)
    }

    @Then("^the user info ods code record will have my (.*)$")
    fun theUserInfoOdsCodeRecordWillHaveMyDetail(detail: String) {
        val patient = SerenityHelpers.getPatient()
        val userAndInfoRecord = UserInfoSerenityHelpers
            .SQL_USER_INFO_ODS_CODE_RECORD.getOrFail<SqlRepositoryUserAndInfo>()

        val expected = getExpectedValue(patient, detail)
        val recorded = getSqlRecordValue(userAndInfoRecord, detail)

        assertEquals("$detail.", expected, recorded)
    }

    @Then("^the user info record will not have (.*)$")
    fun theUserInfoRecordWillNotHaveDetail(detail: String) {
        val userAndInfoRecord = UserInfoSerenityHelpers.MONGO_USER_INFO_RECORD.getOrFail<MongoRepositoryUserAndInfo>()
        val expected = getRecordValue(userAndInfoRecord, detail)

        assertNull("Expected `$detail` not to have a value", expected)
    }

    @Then("^the user info ods code record will not have (.*)$")
    fun theUserInfoOdsCodeRecordWillNotHaveDetail(detail: String) {
        val userAndInfoRecord = UserInfoSerenityHelpers
            .SQL_USER_INFO_ODS_CODE_RECORD.getOrFail<SqlRepositoryUserAndInfo>()

        val expected = getSqlRecordValue(userAndInfoRecord, detail)

        assertNull("Expected `$detail` not to have a value", expected)
    }

    @Then("^there are no details available in the user info repository$")
    fun thereAreNoDetailsAvailableInTheUserInfoRepository() {
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(0)
    }

    private fun getRecordValue(userAndInfoRecord: MongoRepositoryUserAndInfo, detail: String): String? {
        return when (detail) {
            RecordDetail.NhsLoginId.text -> userAndInfoRecord.NhsLoginId
            RecordDetail.OdsCode.text -> userAndInfoRecord.Info.OdsCode
            RecordDetail.NhsNumber.text -> userAndInfoRecord.Info.NhsNumber
            else -> {
                fail("Test setup error, cannot retrieve the specified `$detail` from user and info mongo record")
                ""
            }
        }
    }

    private fun getSqlRecordValue(userAndInfoRecord: SqlRepositoryUserAndInfo, detail: String): String? {
        return when (detail) {
            RecordDetail.NhsLoginId.text -> userAndInfoRecord.id
            RecordDetail.OdsCode.text -> userAndInfoRecord.Info.OdsCode
            RecordDetail.NhsNumber.text -> userAndInfoRecord.Info.NhsNumber
            else -> {
                fail("Test setup error, cannot retrieve the specified `$detail` from user and info mongo record")
                ""
            }
        }
    }

    private fun getExpectedValue(patient: Patient, detail: String): String {
        return when (detail) {
            RecordDetail.NhsLoginId.text -> patient.subject
            RecordDetail.OdsCode.text -> patient.odsCode
            RecordDetail.NhsNumber.text -> patient.nhsNumbers.first()
            else -> {
                fail("Test setup error, cannot retrieve the specified `$detail` from patient")
                ""
            }
        }
    }

    private fun deleteItemsInSqlContainer() {
        val existingNhsNumberItem = UserInfoSerenityHelpers.SQL_USER_INFO_NHS_NUMBER_RECORD
            .getOrNull<SqlRepositoryUserAndInfo>()

        if (existingNhsNumberItem != null) {
            CosmosSqlConnection.UserInfoNhsNumberContainer
                .deleteValue(existingNhsNumberItem.id, existingNhsNumberItem.Info.NhsNumber!!)
        }

        val existingOdsCodeItem = UserInfoSerenityHelpers.SQL_USER_INFO_ODS_CODE_RECORD
            .getOrNull<SqlRepositoryUserAndInfo>()

        if (existingOdsCodeItem != null) {
            CosmosSqlConnection.UserInfoOdsCodeContainer
                .deleteValue(existingOdsCodeItem.id, existingOdsCodeItem.Info.OdsCode)
        }
    }
}
