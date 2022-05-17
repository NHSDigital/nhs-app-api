package mongodb

data class MongoRepositoryMessageSenderContext(val SupplierId: String?,
                                               val SenderId: String?,
                                               val CommunicationId: String?,
                                               val TransmissionId: String?,
                                               val CommunicationCreatedDateTime: String?,
                                               val RequestReference: String?,
                                               val CampaignId: String?,
                                               val OdsCode: String?,
                                               val NhsNumber: String?,
                                               val NhsLoginId: String?)
