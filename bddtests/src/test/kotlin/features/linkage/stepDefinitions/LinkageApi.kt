package features.linkage.stepDefinitions

import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

open class LinkageApi {

    companion object {

        fun get(linkage: LinkageInformationFacade) {
            try {
                val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .authentication.getLinkageKey(linkage)
                Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        fun post(linkage: LinkageInformationFacade, delay : Int? = null) {
            try {
                val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .authentication.postLinkageKey(CreateLinkageRequest(
                        linkage.odsCode,
                        linkage.nhsNumber,
                        linkage.identityToken,
                        linkage.emailAddress,
                        linkage.dateOfBirth,
                        linkage.surname), delay)

                Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }
    }
}