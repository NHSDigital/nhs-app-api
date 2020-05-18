package features.linkage.stepDefinitions

import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import worker.WorkerClient
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

open class LinkageApi {

    companion object {

        fun get(linkage: LinkageInformationFacade) {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getLinkageKey(linkage)
            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)
        }

        fun post(linkage: LinkageInformationFacade, delay: Int? = null) {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postLinkageKey(CreateLinkageRequest(
                            linkage.odsCode,
                            linkage.nhsNumber,
                            linkage.identityToken,
                            linkage.emailAddress,
                            linkage.dateOfBirth,
                            linkage.surname), delay)

            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)
        }
    }
}