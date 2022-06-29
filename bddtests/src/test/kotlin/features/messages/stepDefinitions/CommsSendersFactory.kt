package features.messages.stepDefinitions

import cosmosSql.CosmosSqlConnection
import cosmosSql.SqlRepositoryCommsSender
import cosmosSql.SqlRepositoryRecordCommsSender
import worker.models.messages.CommsSenderRequest
import utils.addToList
import utils.setSingleton
import utils.getOrFail
import utils.GlobalSerenityHelpers
import utils.set
import java.util.UUID

class CommsSendersFactory {

    fun setUpCommsSender(isStored: Boolean = true) {

        MessagesSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = MessagesSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        val record = SqlRepositoryCommsSender(
            id = "Ods-Code-$runUuid".toUpperCase(),
            Name = "Bdd-Test-GP-Name-$runUuid",
            TimeStamp = "2022-06-21T13:00:00.0000000Z"
        )

        MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.set(record)

        if(isStored){
            CosmosSqlConnection.CommsHubSendersContainer.insertValue(
                SqlRepositoryRecordCommsSender(record.id, record.id, record)
            )

            val deletion = { deleteItemInSqlContainer(record) }
            GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
        }
    }

    fun setUpMultipleCommsSenders(recordsAreStale: Boolean) {
        MessagesSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = MessagesSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        val records = arrayListOf(
                SqlRepositoryCommsSender("Ods-Stale-1-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-1-$runUuid",
                        "2022-06-20T13:00:00.0000000Z"),
                SqlRepositoryCommsSender("Ods-Stale-2-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-2-$runUuid",
                        "2022-06-19T13:00:00.0000000Z"),
                SqlRepositoryCommsSender("Ods-Stale-3-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-3-$runUuid",
                        "2022-06-18T13:00:00.0000000Z"),
                SqlRepositoryCommsSender("Ods-Stale-4-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-4-$runUuid",
                        "2022-06-17T13:00:00.0000000Z"),
                SqlRepositoryCommsSender("Ods-Stale-5-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-5-$runUuid",
                        "2022-06-16T13:00:00.0000000Z"),
                SqlRepositoryCommsSender("Ods-Stale-6-$runUuid".toUpperCase(),
                        "Bdd-GP-Name-Stale-6-$runUuid",
                        "2022-06-15T13:00:00.0000000Z")
                )

        MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.set(
            records.map{ record -> record.id }.toList()
        )

        if (recordsAreStale) {
            MessagesSerenityHelpers.TARGET_LAST_UPDATED_BEFORE.set("2022-06-21")
        }
        else {
            MessagesSerenityHelpers.TARGET_LAST_UPDATED_BEFORE.set("2020-01-01")
        }

        CosmosSqlConnection.CommsHubSendersContainer.insertValues(
                records.map{record ->
                    SqlRepositoryRecordCommsSender(record.id, record.id, record)})

        records.forEach{record ->
            val deletion = { deleteItemInSqlContainer(record) }
            GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
        }
    }

    fun createCommsSender() {

        MessagesSerenityHelpers.RUN_UUID.setSingleton(UUID.randomUUID())
        val runUuid = MessagesSerenityHelpers.RUN_UUID.getOrFail<UUID>()

        val record = CommsSenderRequest(
            id = "Ods-Code-$runUuid".toUpperCase(),
            name = "Test-GP-Name-$runUuid"
        )

        MessagesSerenityHelpers.EXPECTED_COMMS_SENDER.set(record)
    }

    fun deleteItemInSqlContainer(sender: SqlRepositoryCommsSender) {
        CosmosSqlConnection.CommsHubSendersContainer.deleteValue(sender.id, sender.id)
    }
}
