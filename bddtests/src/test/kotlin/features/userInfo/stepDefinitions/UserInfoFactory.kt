package features.userInfo.stepDefinitions

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserInfo
import mongodb.MongoRepositoryUserAndInfo
import utils.SerenityHelpers
import utils.set

class UserInfoFactory {

    val mockingClient = MockingClient.instance

    //This functionality is gp system agnostic
    private val targetGpSystem = "EMIS"

    fun setUpUser() {
        val gpSystem = targetGpSystem
        val patientToUse = Patient.getDefault(targetGpSystem)
        SerenityHelpers.setGpSupplier(gpSystem)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patientToUse)
        MongoDBConnection.UserInfoCollection.clearCache()
    }

    fun setUpExistingUserInfo() {
        val patientToUse = SerenityHelpers.getPatient()
        val authToken = patientToUse.accessToken
        UserInfoApi.postUserInfoWithGivenToken(authToken)
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(1)
    }

    fun setUpMultipleExistingUserInfoRecordsLinkedToOdsCode() {
        val targetOdsCode = "TargetOdsCode"
        UserInfoSerenityHelpers.TARGET_ODSCODE.set(targetOdsCode)
        setUpMultipleExistingUserInfoRecords(targetOdsCode, null)
    }

    fun setUpMultipleExistingUserInfoRecordsLinkedToNhsNumber() {
        val targetNhsNumber = "9999999999"
        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set(targetNhsNumber)
        setUpMultipleExistingUserInfoRecords(null, targetNhsNumber)
    }

    fun setUpMultipleExistingUserInfoRecords(odsCode: String?, nhsNumber: String?) {

        val nhsLoginId1 = "NhsLoginId1"
        val nhsLoginId2 = "NhsLoginId2"
        val nhsLoginId3 = "NhsLoginId3"
        val nhsLoginId4 = "NhsLoginId4"

        val values = arrayListOf(
                createUserInfo(nhsLoginId1, odsCode, "1111111111"),
                createUserInfo(nhsLoginId2, odsCode, "1111111112"),
                createUserInfo(nhsLoginId3, odsCode, nhsNumber),
                createUserInfo(nhsLoginId4, "OdsCodeX", nhsNumber)
        )

        if (odsCode != null) {
            UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS.set(arrayListOf(nhsLoginId1, nhsLoginId2, nhsLoginId3))
        }
        if (nhsNumber != null) {
            UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS.set(arrayListOf(nhsLoginId3, nhsLoginId4))
        }
        MongoDBConnection.UserInfoCollection.clearAndInsertValues(values)
    }

    private fun createUserInfo(nhsLoginId: String, odsCode: String?, nhsNumber: String?) : MongoRepositoryUserAndInfo {
        return MongoRepositoryUserAndInfo(
                _id = nhsLoginId,
                Info = MongoRepositoryUserInfo(odsCode ?: "OdsCodeX",
                        nhsNumber ?: "1111111111",
                        false))
    }
}
