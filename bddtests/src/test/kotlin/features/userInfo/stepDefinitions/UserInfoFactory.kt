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

    fun setUpMultipleExistingUserInfoRecords(odsCode: String?, nhsNumber: String?, runUuid: UUID) {

        val nhsLoginId1 = "NhsLoginIdBdd1-$runUuid"
        val nhsLoginId2 = "NhsLoginIdBdd2-$runUuid"
        val nhsLoginId3 = "NhsLoginIdBdd3-$runUuid"
        val nhsLoginId4 = "NhsLoginIdBdd4-$runUuid"

        val odsCodePlusUuid = "${odsCode ?: "odsCodeX"}-$runUuid"
        val nhsNumberPlusUuid = "${nhsNumber ?: "1111111111"}-$runUuid"

        val values = arrayListOf(
            createUserInfo(nhsLoginId1, odsCodePlusUuid, "1111111111-$runUuid"),
            createUserInfo(nhsLoginId2, odsCodePlusUuid, "1111111112-$runUuid"),
            createUserInfo(nhsLoginId3, odsCodePlusUuid, nhsNumberPlusUuid),
            createUserInfo(nhsLoginId4, "OdsCodeX-$runUuid", nhsNumberPlusUuid)
        )

        val sqlUserValues = arrayListOf(
            createSqlUserInfo(nhsLoginId1, odsCodePlusUuid, "1111111111-$runUuid"),
            createSqlUserInfo(nhsLoginId2, odsCodePlusUuid, "1111111112-$runUuid"),
            createSqlUserInfo(nhsLoginId3, odsCodePlusUuid, nhsNumberPlusUuid),
            createSqlUserInfo(nhsLoginId4, "OdsCodeX-$runUuid", nhsNumberPlusUuid)
        )

        if (odsCode != null) {
            UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS.set(arrayListOf(nhsLoginId1, nhsLoginId2, nhsLoginId3))
        }
        if (nhsNumber != null) {
            UserInfoSerenityHelpers.EXPECTED_NHSLOGINIDS.set(arrayListOf(nhsLoginId3, nhsLoginId4))
        }
        MongoDBConnection.UserInfoCollection.clearAndInsertValues(values)

        CosmosSqlConnection.UserInfoNhsNumberContainer.insertValues(
            sqlUserValues.map { x ->
                    SqlRepositoryRecordUserAndInfo(x.Info.NhsNumber.orEmpty(),
                            x.id,
                        SqlRepositoryUserAndInfo(x.Info, x.id))
                })

        CosmosSqlConnection.UserInfoOdsCodeContainer.insertValues(
            sqlUserValues.map { x ->
                SqlRepositoryRecordUserAndInfo(x.Info.OdsCode,
                    x.id,
                    SqlRepositoryUserAndInfo(x.Info, x.id))
                })

        val deletion = { deleteItemsInSqlContainers(sqlUserValues) }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
    }

    private fun createUserInfo(nhsLoginId: String, odsCode: String, nhsNumber: String) : MongoRepositoryUserAndInfo {
        return MongoRepositoryUserAndInfo(
                NhsLoginId = nhsLoginId,
                Info = MongoRepositoryUserInfo(odsCode, nhsNumber))
    }

    private fun createSqlUserInfo(nhsLoginId: String, odsCode: String, nhsNumber: String) : SqlRepositoryUserAndInfo {
        return SqlRepositoryUserAndInfo(
            id = nhsLoginId,
            Info = SqlRepositoryUserInfo(odsCode, nhsNumber)
        )
    }

    private fun deleteItemsInSqlContainers(sqlUserValues: ArrayList<SqlRepositoryUserAndInfo>) {

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
