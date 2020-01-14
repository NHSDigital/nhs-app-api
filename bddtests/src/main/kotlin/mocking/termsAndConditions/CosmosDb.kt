package mocking

import com.microsoft.azure.cosmosdb.ConnectionPolicy
import com.microsoft.azure.cosmosdb.ConsistencyLevel
import com.microsoft.azure.cosmosdb.Document
import com.microsoft.azure.cosmosdb.FeedOptions
import com.microsoft.azure.cosmosdb.PartitionKey
import com.microsoft.azure.cosmosdb.RequestOptions
import com.microsoft.azure.cosmosdb.rx.AsyncDocumentClient
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.addToList
import java.time.OffsetDateTime


object CosmosDb
{

    fun clearTermsAndConditionsAcceptance()
    {
        clearAcceptance()
        addCleanUp()
    }

    fun addTermsAndConditionsAcceptance(consentDate: OffsetDateTime)
    {
        val nhsNumber = SerenityHelpers.getPatient().nhsNumbers.single()
        val cosmosClient = connectToCosmos()

        val collectionLink = System.getenv("TERMS_CONDITIONS_COLLECTION_LINK")
        val current = consentDate.toString()
        val documentDefinition = Document()
        documentDefinition.set("Id", "6d9e1f57-ef28-4328-8d91-478599e02fad")
        documentDefinition.set("NhsNumber", nhsNumber)
        documentDefinition.set("ConsentGiven", true)
        documentDefinition.set("DateOfConsent", current)

        cosmosClient.createDocument(collectionLink, documentDefinition, null, false)
                .single()
                .subscribe()

        addCleanUp()
    }

    private fun connectToCosmos():AsyncDocumentClient
    {
        val masterKey = System.getenv("TERMS_CONDITIONS_COSMOS_AUTH_KEY")
        val host = System.getenv("TERMS_CONDITIONS_COSMOS_ENDPOINT_URI")

        return AsyncDocumentClient
                .Builder()
                .withServiceEndpoint(host)
                .withMasterKeyOrResourceToken(masterKey)
                .withConnectionPolicy(ConnectionPolicy.GetDefault())
                .withConsistencyLevel(ConsistencyLevel.Eventual)
                .build()
    }

    private fun addCleanUp() {
        val cleanUp = { clearAcceptance() }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(cleanUp)
    }

    private fun clearAcceptance() {
        val nhsNumber = SerenityHelpers.getPatient().nhsNumbers.single()
        val cosmosClient = connectToCosmos()

        val collectionLink = System.getenv("TERMS_CONDITIONS_COLLECTION_LINK")
        val filterQuery = "SELECT * FROM consent r where r.NhsNumber = '$nhsNumber'"
        val feedOptions = FeedOptions()
        feedOptions.partitionKey = PartitionKey(nhsNumber)
        val feedResponses = cosmosClient.queryDocuments(collectionLink, filterQuery, feedOptions).toBlocking()

        val requestOptions = RequestOptions()
        requestOptions.partitionKey = PartitionKey(nhsNumber)
        feedResponses.toIterable().forEach {
            feedResponse -> feedResponse.results.forEach {
                item -> cosmosClient.deleteDocument(item.selfLink, requestOptions).subscribe()
            }
        }
    }
}
