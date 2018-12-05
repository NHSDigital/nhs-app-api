package mongodb

import com.mongodb.MongoClient
import config.Config

class MongoDBConnection {

    companion object {
        private const val developmentDatabaseName = "development"
        private const val im1CacheCollectionName = "im1cache"

        @JvmStatic
        fun main(arguments: Array<String>? = null) {
            val mc = MongoClient(Config.instance.mongoDbHost, Config.instance.mongoDbPort.toInt())
            mc.listDatabaseNames().forEach { databaseName ->
                println("\t$databaseName")
                print(getContentsFromDatabase(databaseName, mc))
            }
            mc.close()
        }

        fun getContentsFromDatabase(
                databaseName: String = developmentDatabaseName,
                existingMongoDbConnection: MongoClient? = null
        ): String {
            val mc = existingMongoDbConnection ?:
            MongoClient(Config.instance.mongoDbHost, Config.instance.mongoDbPort.toInt())
            val db = mc.getDatabase(databaseName)
            var contents = "\n"
            db.listCollectionNames().forEach { collectionName ->
                contents += "\t\t$collectionName\n"
                val collection = db.getCollection(collectionName)
                val documents = collection.find()
                documents.forEach { document ->
                    contents += "\t\t\t${document.toJson()}\n"
                }
            }
            if (existingMongoDbConnection == null) mc.close()
            return contents
        }

        fun getNumberOfDocumentsFromIm1Cache(): Long {
            val mc = MongoClient(Config.instance.mongoDbHost, Config.instance.mongoDbPort.toInt())
            val numberOfDocuments = mc
                    .getDatabase(developmentDatabaseName)
                    .getCollection(im1CacheCollectionName)
                    .countDocuments()
            mc.close()
            return numberOfDocuments
        }

        fun clearIm1Cache() {
            val mc = MongoClient(Config.instance.mongoDbHost, Config.instance.mongoDbPort.toInt())
            mc.getDatabase(developmentDatabaseName).getCollection(im1CacheCollectionName).drop()
            mc.close()
        }
    }
}
