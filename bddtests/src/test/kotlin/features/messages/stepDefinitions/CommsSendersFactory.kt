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
            Name = "Bdd-Test-GP-Name-$runUuid"
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
