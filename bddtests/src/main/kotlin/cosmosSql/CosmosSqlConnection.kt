package cosmosSql

import config.Config
import com.azure.cosmos.CosmosContainer
import com.azure.cosmos.CosmosClientBuilder
import com.azure.cosmos.models.CosmosItemResponse
import com.azure.cosmos.models.PartitionKey

class CosmosSqlConnection(private val containerName: String, private val endpoint: String, private val key: String) {

    fun <T> insertValues(values: List<ISqlRepositoryRecord<T>>) {
        values.forEach{ item -> insertValue(item) }
    }

    fun <T> insertValue(value: ISqlRepositoryRecord<T>) {
        onContainer { container ->
            container.createItem(value.repositoryRecord, PartitionKey(value.partitionKeyValue), null)
        }
    }

    fun deleteValue(id: String, partitionKeyValue: String) {
        onContainer { container ->
            container.deleteItem(id, PartitionKey(partitionKeyValue), null)
        }
    }

    fun <T> getValueWhere(itemType: Class<T>, id: String, partitionKeyValue: String) : CosmosItemResponse<T> {
        return onContainer { container ->
            container.readItem(id, PartitionKey(partitionKeyValue), null, itemType)
        }
    }

    private fun <TReturn> onContainer(action: (CosmosContainer) -> TReturn): TReturn {
        val cosmosClientClient = CosmosClientBuilder()
                .endpoint(endpoint)
                .key(key)
                .gatewayMode()
                .buildClient()
        val database = cosmosClientClient.getDatabase(developmentDatabaseName)
        val container = database.getContainer(containerName)
        val actionResult = action.invoke(container)
        cosmosClientClient.close()
        return actionResult
    }

    companion object {

        private const val userInfoNhsNumberContainerName = "info_nhs_number"
        private const val userInfoOdsCodeContainerName = "info_ods_code"
        private const val commsHubSendersContainerName = "senders"
        private const val developmentDatabaseName = "comms-dev-sql"

        val UserInfoNhsNumberContainer = CosmosSqlConnection(
                userInfoNhsNumberContainerName,
                Config.instance.cosmosSqlEndpoint,
                Config.instance.cosmosSqlKey)
        val UserInfoOdsCodeContainer = CosmosSqlConnection(
                userInfoOdsCodeContainerName,
                Config.instance.cosmosSqlEndpoint,
                Config.instance.cosmosSqlKey)
        val CommsHubSendersContainer = CosmosSqlConnection(
            commsHubSendersContainerName,
            Config.instance.cosmosSqlEndpoint,
            Config.instance.cosmosSqlKey)
    }
}
