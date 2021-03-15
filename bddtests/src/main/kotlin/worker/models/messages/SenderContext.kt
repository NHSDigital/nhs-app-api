package worker.models.messages

data class SenderContext(var supplierId: String? = null,
                         var communicationId: String? = null,
                         var transmissionId: String? = null,
                         var communicationCreatedDateTime: String? = null,
                         var requestReference: String? = null,
                         var campaignId: String? = null,
                         var odsCode: String? = null,
                         var nhsNumber: String? = null,
                         var nhsLoginId: String? = null)
