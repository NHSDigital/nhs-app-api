package features.messages.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.set
import worker.WorkerClient
import worker.models.messages.CommsSenderRequest
import worker.models.messages.CommsSenderResponse


class CommsSendersApi {
    companion object {

        fun get(senderId: String, includeApiKey:Boolean = true): CommsSenderResponse? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .commsSenders
                .get(senderId, includeApiKey)
            MessagesSerenityHelpers.GET_COMMS_SENDERS_RESPONSE.set(response)
            return response
        }

        fun getLastUpdatedBefore(lastUpdatedBefore: String, limit: Int, includeApiKey: Boolean = true): List<String>? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .commsSenders
                .getLastUpdatedBefore(lastUpdatedBefore, limit, includeApiKey)
            MessagesSerenityHelpers.GET_COMMS_SENDERS_RESPONSE.set(response)
            return response
        }

        fun post(sender: CommsSenderRequest, includeApiKey:Boolean = true): CommsSenderResponse? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .commsSenders
                .post(sender, includeApiKey)
            MessagesSerenityHelpers.CREATE_COMMS_SENDERS_RESPONSE.set(response)
            return response
        }

    }
}
