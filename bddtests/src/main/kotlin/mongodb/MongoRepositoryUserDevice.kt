package mongodb

data class MongoRepositoryUserDevice(val _id: String,
                                     val NhsLoginId: String,
                                     val RegistrationId: String,
                                     val PnsToken: String)