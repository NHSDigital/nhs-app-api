package mongodb

import com.google.gson.GsonBuilder
import com.mongodb.MongoClient
import com.mongodb.client.MongoDatabase
import config.Config
import org.junit.Assert
import java.lang.reflect.Type

class MongoDBConnection(private val collectionName: String, private val host: String, private val port: Int) {

    fun clearCache() {
        val mongoClient = getMongoClient()
        mongoClient.getDatabase(developmentDatabaseName).getCollection(collectionName).drop()
        mongoClient.close()
        assertNumberOfDocuments(0)
    }

    fun assertNumberOfDocuments(expected: Int) {
        val mongoClient = getMongoClient()
        val mongoDatabase = mongoClient.getDatabase(developmentDatabaseName)
        val numberOfDocuments = mongoDatabase
                .getCollection(collectionName)
                .countDocuments().toInt()
        Assert.assertEquals(
                "Number of documents in $collectionName. Present: ${getContents(mongoDatabase)}",
                expected,
                numberOfDocuments)
        mongoClient.close()
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

    private fun getContents(mongoDatabase: MongoDatabase): String {
        val collection = mongoDatabase.getCollection(collectionName)
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

