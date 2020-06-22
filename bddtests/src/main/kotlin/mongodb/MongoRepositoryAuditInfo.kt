package mongodb

data class MongoRepositoryAuditInfo(val NhsLoginSubject: String,
                                    val NhsNumber: String,
                                    val IsActingOnBehalfOfAnother: Boolean,
                                    val Supplier: String,
                                    val Operation: String,
                                    val Details: String)
