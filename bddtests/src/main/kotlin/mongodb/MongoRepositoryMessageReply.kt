package mongodb

data class MongoRepositoryMessageReply(val Options: List<MongoRepositoryMessageReplyOption>?,
                                       val Response: String?,
                                       val ResponseSentDateTime: String?,
                                       var Status: String?,
                                       var ResponseCompletedDateTime: String?)
