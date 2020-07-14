package features.messages.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.set
import worker.JsonPatch
import worker.WorkerClient
import worker.models.messages.MessageCreateResponse
import worker.models.messages.MessageRequest
import worker.models.messages.MessagesResponse

class MessagesApi {
    companion object {

        fun getSummary(authToken: String?): Array<MessagesResponse>? {
            return get(authToken, true)
        }

        fun getFromSender(authToken: String?, targetSender: String): Array<MessagesResponse>? {
            return get(authToken, false, targetSender)
        }

        fun get(authToken: String?, summary: Boolean, targetSender: String? = null): Array<MessagesResponse>? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .messages
                    .get(authToken, summary, targetSender)
            MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.set(response)
            return response
        }

        fun postSetup(
                request: MessageRequest,
                nhsLoginId: String,
                includeApiKey: Boolean = true): MessageCreateResponse? = post(request, nhsLoginId, includeApiKey)

        fun post(request: MessageRequest,
                 nhsLoginId: String,
                 includeApiKey: Boolean = true): MessageCreateResponse? {
            val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .post(request, nhsLoginId, includeApiKey)
            MessagesSerenityHelpers.CREATE_MESSAGE_RESPONSE.set(response)
            return response
        }

        fun patch(authToken: String?, messageId: String, patch: JsonPatch) {
            Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .messages
                    .patch(authToken, messageId, patch)
        }
    }
}
