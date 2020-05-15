package features.messages.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import utils.set
import worker.JsonPatch
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.messages.MessageRequest
import worker.models.messages.MessagesResponse

class MessagesApi {
    companion object {

        fun getSummary(authToken: String?): Array<MessagesResponse>? {
            return get(authToken, true)
        }

        fun getFromSender(authToken: String?, targetSender:String): Array<MessagesResponse>? {
            return get(authToken, false, targetSender)
        }

        fun get(authToken: String?, summary:Boolean, targetSender:String? =null): Array<MessagesResponse>? {
            try {
                val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .get(authToken, summary, targetSender)
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.set(response)
                return response
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
            return null
        }

        fun postSetup(
                request: MessageRequest,
                nhsLoginId: String,
                includeApiKey :Boolean = true) = post(request, nhsLoginId, includeApiKey, true)

        fun post(request: MessageRequest,
                 nhsLoginId: String,
                 includeApiKey :Boolean = true,
                 propagateException: Boolean = false) {
            try {
                Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .post(request, nhsLoginId, includeApiKey)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
                if (propagateException) {
                    throw httpException
                }
            }
        }

        fun patch(authToken: String?, messageId:String, patch : JsonPatch){
            try {
                Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .patch(authToken, messageId, patch)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }
    }
}