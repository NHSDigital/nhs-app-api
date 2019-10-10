package mongodb

data class MongoRepositoryMessage(val NhsLoginId: String,
                                  val Sender: String,
                                  val Version: Int,
                                  val Body: String,
                                  val Read: Boolean)
