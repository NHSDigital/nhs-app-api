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
import java.time.format.DateTimeFormatter

class MongoDBConnection(private val collectionName: String, private val host: String, private val port: Int) {

    fun clearCache() {
        onCollection { collection ->
            collection.drop()
            assertNumberOfDocuments(0, collection)
        }
    }

    fun assertNumberOfDocuments(expected: Int) {
        onCollection { collection -> assertNumberOfDocuments(expected, collection) }
    }

    fun <T> getValues(type: Type): List<T> {
        val gsonBuilder = GsonBuilder().create()
        return onCollection { collection ->
            val documents = collection.find()
            documents.map {
                document ->
                val jsonDocument = document.toJson()
                gsonBuilder.fromJson<T>(jsonDocument, type)
            }.toList()
        }
    }

    fun <T> clearAndInsertValues(values: List<T>) {
        val gsonBuilder = GsonBuilder().create()
        val jsonToInsert = values.map { value -> gsonBuilder.toJson(value) }
        clearAndInsertJson(jsonToInsert)
    }

    fun <T> clearAndInsertValue(value: T) {
        clearAndInsertValues(listOf(value))
    }

    fun clearAndInsertJson(values: List<String>) {
        val documentsToInsert = values.map { value -> Document.parse(value) }
        onCollection { collection ->
            collection.drop()
            assertNumberOfDocuments(0, collection)
            collection.insertMany(documentsToInsert)
            assertNumberOfDocuments(values.count(), collection)
        }
    }

    private fun <TReturn> onCollection(action: (MongoCollection<Document>) -> TReturn): TReturn {
        val mongoClient = MongoClient(host, port)
        val mongoDatabase = mongoClient.getDatabase(developmentDatabaseName)
        val collection = mongoDatabase.getCollection(collectionName)
        val actionResult = action.invoke(collection)
        mongoClient.close()
        return actionResult
    }

    private fun assertNumberOfDocuments(expected: Int, collection: MongoCollection<Document>) {
        var retryCount = (TIME_TO_WAIT_FOR_ELEMENT / ELEMENT_RETRY_TIME).toInt()
        while (retryCount >= 0) {
            val numberOfDocuments = collection.countDocuments().toInt()
            if (numberOfDocuments == expected) {
                break
            } else {
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

    private fun formatContents(collection: MongoCollection<Document>): String {
        val documents = collection.find()
        val contents = documents.map { document -> "\t\t\t${document.toJson()}" }.joinToString("\n")
        return "\n\t\t$collectionName\n\t$contents\n"
    }

    companion object {
        private const val im1CacheCollectionName = "im1cache"
        private const val userDevicesCollectionName = "devices"
        private const val userInfoCollectionName = "info"
        private const val messagesCollectionName = "messages"
        private const val developmentDatabaseName = "development"
        private const val termsAndConditionsCollectionName = "consent"
        val mongoDateFormatter: DateTimeFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ssz")

        val Im1CacheCollection = MongoDBConnection(
                im1CacheCollectionName,
                Config.instance.sessionMongoDbHost,
                Config.instance.sessionMongoDbPort.toInt())
        val UserDevicesCollection = MongoDBConnection(
                userDevicesCollectionName,
                Config.instance.usersMongoDbHost,
                Config.instance.usersMongoDbPort.toInt())
        val MessagesCollection = MongoDBConnection(
                messagesCollectionName,
                Config.instance.messagesMongoDbHost,
                Config.instance.messagesMongoDbPort.toInt())
        val UserInfoCollection = MongoDBConnection(
                userInfoCollectionName,
                Config.instance.usersMongoDbHost,
                Config.instance.usersMongoDbPort.toInt())
        val TermsAndconditionsCollection = MongoDBConnection(
                termsAndConditionsCollectionName,
                Config.instance.consentMongoDbHost,
                Config.instance.consentMongoDbPort.toInt())
    }
}
