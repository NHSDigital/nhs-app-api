package mongodb

import com.google.gson.GsonBuilder
import com.mongodb.MongoClient
import com.mongodb.client.MongoCollection
import config.Config
import org.bson.Document
import org.junit.Assert
import pages.ELEMENT_RETRY_TIME
import pages.MILLISECONDS_IN_A_SECOND
import pages.TIME_TO_WAIT_FOR_ELEMENT
import java.lang.reflect.Type

class MongoDBConnection(private val collectionName: String, private val host: String, private val port: Int) {

    fun clearCache() {
        val mongoClient = getMongoClient()
        mongoClient.getDatabase(developmentDatabaseName).getCollection(collectionName).drop()
        mongoClient.close()
        assertNumberOfDocuments(0)
    }

    fun assertNumberOfDocuments(expected: Int){
        val mongoClient = getMongoClient()
        val mongoDatabase = mongoClient.getDatabase(developmentDatabaseName)
        var retryCount = (TIME_TO_WAIT_FOR_ELEMENT / ELEMENT_RETRY_TIME).toInt()
        while(retryCount>=0) {
            val collection = mongoDatabase.getCollection(collectionName)
            val numberOfDocuments = collection.countDocuments().toInt()
            if(numberOfDocuments == expected){break;}
            else {
                when (retryCount) {
                    0 -> Assert.fail(
                            "Number of documents in $collectionName. " +
                                    "\nExpected:$expected, Actual:$numberOfDocuments " +
                                    "\nPresent: ${formatContents(collection)}")
                    else -> {
                        println("Number of documents in $collectionName. " +
                                "Expected:$expected, Actual:$numberOfDocuments. RETRYING")
                        retryCount--
                        Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
                    }
                }
            }
        }
    }

    fun <T> getValues(type: Type): List<T> {
        val mongoClient = getMongoClient()
        val mongoDatabase = mongoClient.getDatabase(developmentDatabaseName)
        val collection = mongoDatabase.getCollection(collectionName)
        val documents = collection.find()
        val values = documents.map { document ->
            GsonBuilder().create().fromJson<T>(document.toJson(), type)
        }.toList()
        mongoClient.close()
        return values
    }

    private fun formatContents(collection: MongoCollection<Document>): String {
        val documents = collection.find()
        val contents = documents.map { document -> "\t\t\t${document.toJson()}" }.joinToString("\n")
        return "\n\t\t$collectionName\n\t$contents\n"
    }

    private fun getMongoClient(): MongoClient {
        return MongoClient(host, port)
    }

    companion object {
        private const val im1CacheCollectionName = "im1cache"
        private const val userDevicesCollectionName = "devices"
        private const val developmentDatabaseName = "development"

        val Im1CacheCollection = MongoDBConnection(
                im1CacheCollectionName,
                Config.instance.mongoDbHost,
                Config.instance.mongoDbPort.toInt())
        val UserDevicesCollection = MongoDBConnection(
                userDevicesCollectionName,
                Config.instance.usersMongoDbHost,
                Config.instance.usersMongoDbPort.toInt())
    }
}

