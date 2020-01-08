package mocking

import com.microsoft.azure.cosmosdb.ConnectionPolicy
import com.microsoft.azure.cosmosdb.ConsistencyLevel
import com.microsoft.azure.cosmosdb.Document
import com.microsoft.azure.cosmosdb.FeedOptions
import com.microsoft.azure.cosmosdb.PartitionKey
import com.microsoft.azure.cosmosdb.RequestOptions
import com.microsoft.azure.cosmosdb.rx.AsyncDocumentClient
import java.time.OffsetDateTime


object CosmosDb
{
    fun connectToCosmos(MASTER_KEY:String, HOST:String):AsyncDocumentClient
    {
        return AsyncDocumentClient.Builder()
                .withServiceEndpoint(HOST)
                .withMasterKeyOrResourceToken(MASTER_KEY)
                .withConnectionPolicy(ConnectionPolicy.GetDefault())
                .withConsistencyLevel(ConsistencyLevel.Eventual)
                .build()
    }

    fun clearTermsAndConditionsAcceptance()
    {
        val cosmosClient = connectToCosmos(System.getenv("TERMS_CONDITIONS_COSMOS_AUTH_KEY"),
                System.getenv("TERMS_CONDITIONS_COSMOS_ENDPOINT_URI"))
        val filterQuery = "SELECT * FROM consent r where r.NhsNumber = '096 876 4215'"
        val feedOptions = FeedOptions()
        feedOptions.partitionKey = PartitionKey("096 876 4215")
        val feedResponses = cosmosClient.queryDocuments(
                System.getenv("TERMS_CONDITIONS_COLLECTION_LINK"), filterQuery, feedOptions).toBlocking()
        val requestOptions = RequestOptions()
        requestOptions.partitionKey = PartitionKey("096 876 4215")
        feedResponses.toIterable().forEach { feedResponse -> feedResponse.results.forEach { item ->
            cosmosClient.deleteDocument(item.selfLink, requestOptions).subscribe()
        }
        }
    }

    fun addTermsAndConditionsAcceptance(consentDate: OffsetDateTime)
    {
        val cosmosClient = connectToCosmos(System.getenv("TERMS_CONDITIONS_COSMOS_AUTH_KEY"),
                System.getenv("TERMS_CONDITIONS_COSMOS_ENDPOINT_URI"))
        val current = consentDate.toString()
        val documentDefinition = Document()
        documentDefinition.set("Id", "6d9e1f57-ef28-4328-8d91-478599e02fad")
        documentDefinition.set("NhsNumber", "096 876 4215")
        documentDefinition.set("ConsentGiven", true)
        documentDefinition.set("DateOfConsent", current)


        cosmosClient.createDocument(System.getenv("TERMS_CONDITIONS_COLLECTION_LINK"), documentDefinition, null, false)
                .single()
                .subscribe()

    }
}
