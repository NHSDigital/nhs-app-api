package features.messages.stepDefinitions

import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.messages.MessageRequest

class MessagesApi {
    companion object {

        fun get(authToken: String?) {
            try {
                val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .get(authToken)
                MessagesSerenityHelpers.GET_MESSAGE_RESPONSE.set(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        fun post(request: MessageRequest, nhsLoginId: String) {
            try {
                Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .messages
                        .post(request, nhsLoginId)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }
    }
}