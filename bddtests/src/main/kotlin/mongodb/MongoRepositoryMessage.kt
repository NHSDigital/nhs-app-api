package mongodb

data class MongoRepositoryMessage(val NhsLoginId: String,
                                  val Sender: String,
                                  val Version: Int,
                                  val MessageId: String,
                                  val Body: String)