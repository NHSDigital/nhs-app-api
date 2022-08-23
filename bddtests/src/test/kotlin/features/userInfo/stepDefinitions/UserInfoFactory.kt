package features.userInfo.stepDefinitions

import constants.Supplier
import cosmosSql.CosmosSqlConnection
import cosmosSql.SqlRepositoryRecordUserAndInfo
import cosmosSql.SqlRepositoryUserAndInfo
import cosmosSql.SqlRepositoryUserInfo
import mocking.defaults.dataPopulation.journeys.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journeys.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserInfo
import mongodb.MongoRepositoryUserAndInfo
import utils.addToList
import utils.setSingleton
import utils.getOrFail
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.set
import worker.models.userInfo.UserInfoV2Response
import java.util.UUID

class UserInfoFactory {

    //This functionality is gp system agnostic
    private val targetGpSystem = Supplier.EMIS

    fun setUpUser() {
        setUpUser { it }
    }

    fun setUpUser(patient: Patient) {
        SerenityHelpers.setGpSupplier(targetGpSystem)

        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = UserInfoSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        patient.subject = "${patient.subject}-$runUuid"
        patient.odsCode = "${patient.odsCode}-$runUuid"
        patient.nhsNumbers = patient.nhsNumbers.map { x -> "$x-$runUuid" }
        patient.regenerateAccessToken()

        SerenityHelpers.setPatient(patient)

        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(targetGpSystem).createFor(patient)
        MongoDBConnection.UserInfoCollection.clearCache()
    }

    fun setUpUser(getPatient: (Patient) -> Patient) {
        val patientToUse = getPatient.invoke(Patient.getDefault(targetGpSystem)).copy()

        setUpUser(patientToUse)
    }

    fun setUpExistingUserInfo() {
        val patientToUse = SerenityHelpers.getPatient()
        val authToken = patientToUse.accessToken
        UserInfoApi.postUserInfoWithGivenToken(authToken)
        MongoDBConnection.UserInfoCollection.assertNumberOfDocuments(1)

        val deletion = { deleteItemInSqlContainers(patientToUse) }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
    }

    fun setUpMultipleExistingUserInfoRecordsLinkedToOdsCode() {
        val targetOdsCode = "TargetOdsCode"
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = UserInfoSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        UserInfoSerenityHelpers.TARGET_ODSCODE.set("$targetOdsCode-$runUuid")
        setUpMultipleExistingUserInfoRecords(targetOdsCode, null, runUuid)
    }

    fun setUpMultipleExistingUserInfoRecordsLinkedToNhsNumber() {
        val targetNhsNumber = "9999999999"
        UserInfoSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = UserInfoSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        UserInfoSerenityHelpers.TARGET_NHSNUMBER.set("$targetNhsNumber-$runUuid")
        setUpMultipleExistingUserInfoRecords(null, targetNhsNumber, runUuid)
    }

    fun setUpMultipleExistingUserInfoRecords(targetOdsCode: String?, targetNhsNumber: String?, runUuid: UUID) {

        val nhsLoginId1 = "NhsLoginIdBdd1-$runUuid"
        val nhsLoginId2 = "NhsLoginIdBdd2-$runUuid"
        val nhsLoginId3 = "NhsLoginIdBdd3-$runUuid"
        val nhsLoginId4 = "NhsLoginIdBdd4-$runUuid"

        val odsCode = "${targetOdsCode ?: "odsCodeX"}-$runUuid"
        val nhsNumber = "${targetNhsNumber ?: "1111111111"}-$runUuid"

        val mongoRepositoryUserInfoEntries = arrayListOf(
            createUserInfo(
                nhsLoginId1, odsCode, "1111111112-$runUuid", "2022-06-20T13:00:00.5086666Z"),
            createUserInfo(
                nhsLoginId2, odsCode, "1111111113-$runUuid", "2022-06-20T13:01:00.5086666Z"),
            createUserInfo(
                nhsLoginId3, odsCode, nhsNumber, "2022-06-20T13:02:00.5086666Z"),
            createUserInfo(
                nhsLoginId4, "OdsCodeX-$runUuid", nhsNumber, "2022-06-20T13:03:00.5086666Z")
        )

        val expectedLoginIds = if (targetOdsCode != null) {
            arrayListOf(nhsLoginId1, nhsLoginId2, nhsLoginId3)
        }
        else if (targetNhsNumber != null) {
            arrayListOf(nhsLoginId3, nhsLoginId4)
        }
        else {
            arrayListOf()
        }

        val expectedUserInfoResponses = getExpectedUserInfoResponses(expectedLoginIds, mongoRepositoryUserInfoEntries)

        UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS.set(expectedLoginIds)
        UserInfoSerenityHelpers.EXPECTED_USERINFOS.set(expectedUserInfoResponses)

        addUserInfoToMongoCollection(mongoRepositoryUserInfoEntries)
        addUserInfoToSqlApiContainers(mongoRepositoryUserInfoEntries)
    }

    private fun createUserInfo(nhsLoginId: String, odsCode: String, nhsNumber: String, timestamp: String)
    : MongoRepositoryUserAndInfo {
        return MongoRepositoryUserAndInfo(
                NhsLoginId = nhsLoginId,
                Info = MongoRepositoryUserInfo(odsCode, nhsNumber),
                Timestamp = timestamp)
    }

    private fun getExpectedUserInfoResponses(expectedLoginIds: ArrayList<String>,
                                            mongoRepositoryUserInfoEntries: List<MongoRepositoryUserAndInfo>
    ) : List<UserInfoV2Response> {
        return mongoRepositoryUserInfoEntries
            .filter{ userInfo -> expectedLoginIds.contains(userInfo.NhsLoginId) }
            .map { userInfo ->
                UserInfoV2Response(
                    nhsLoginId = userInfo.NhsLoginId,
                    nhsNumber = userInfo.Info.NhsNumber.orEmpty(),
                    odsCode = userInfo.Info.OdsCode,
                    lastLogin = userInfo.Timestamp
                )
            }
    }

    private fun addUserInfoToMongoCollection(mongoRepositoryUserInfoEntries: List<MongoRepositoryUserAndInfo>) {
        MongoDBConnection.UserInfoCollection.clearAndInsertValues(mongoRepositoryUserInfoEntries)
    }

    private fun addUserInfoToSqlApiContainers(mongoRepositoryUserInfoEntries: List<MongoRepositoryUserAndInfo>) {
        val sqlApiRepositoryUserInfoEntries = mongoRepositoryUserInfoEntries.map { userInfo ->
            createSqlUserInfo(
                userInfo.NhsLoginId, userInfo.Info.OdsCode, userInfo.Info.NhsNumber.orEmpty(), userInfo.Timestamp)
        }

        CosmosSqlConnection.UserInfoNhsNumberContainer.insertValues(
            sqlApiRepositoryUserInfoEntries.map { x ->
                SqlRepositoryRecordUserAndInfo(x.Info.NhsNumber.orEmpty(),
                    x.id,
                    SqlRepositoryUserAndInfo(x.Info, x.id, x.TimeStamp))
            })

        CosmosSqlConnection.UserInfoOdsCodeContainer.insertValues(
            sqlApiRepositoryUserInfoEntries.map { x ->
                SqlRepositoryRecordUserAndInfo(x.Info.OdsCode,
                    x.id,
                    SqlRepositoryUserAndInfo(x.Info, x.id, x.TimeStamp))
            })

        val deletion = { deleteItemsInSqlContainers(sqlApiRepositoryUserInfoEntries) }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
    }

    private fun createSqlUserInfo(nhsLoginId: String, odsCode: String, nhsNumber: String, timestamp: String) :
        SqlRepositoryUserAndInfo {
        return SqlRepositoryUserAndInfo(
            id = nhsLoginId,
            Info = SqlRepositoryUserInfo(odsCode, nhsNumber),
            TimeStamp = timestamp
        )
    }

    private fun deleteItemsInSqlContainers(sqlUserValues: List<SqlRepositoryUserAndInfo>) {
        sqlUserValues.map { item ->
                CosmosSqlConnection.UserInfoNhsNumberContainer.deleteValue(item.id, item.Info.NhsNumber!!)
                CosmosSqlConnection.UserInfoOdsCodeContainer.deleteValue(item.id, item.Info.OdsCode)
            }
    }

    private fun deleteItemInSqlContainers(patient: Patient) {
        CosmosSqlConnection.UserInfoNhsNumberContainer.deleteValue(patient.subject, patient.nhsNumbers[0])
        CosmosSqlConnection.UserInfoOdsCodeContainer.deleteValue(patient.subject, patient.odsCode)
    }
}
