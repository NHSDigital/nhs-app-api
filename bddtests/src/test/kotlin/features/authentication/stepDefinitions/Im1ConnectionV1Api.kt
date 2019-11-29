package features.authentication.stepDefinitions

import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.setSessionVariable
import utils.SerenityHelpers
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.patient.Im1ConnectionRequest
import worker.models.patient.Im1ConnectionResponse

class Im1ConnectionV1Api {

    companion object {
        fun post(im1ConnectionRequest: Im1ConnectionRequest) {
            try {
                val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .authentication.postIm1Connection(im1ConnectionRequest)
                setSessionVariable(Im1ConnectionResponse::class).to(result)
                AuthenticationSerenityHelpers.IM1_CONNECTION_RESPONSE.set(result)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }
    }
}

