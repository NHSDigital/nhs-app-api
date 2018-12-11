package mocking.vision.linkage

import constants.ErrorResponseCodeVision
import mocking.GsonFactory
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.vision.models.error.VisionError
import mocking.vision.models.error.VisionRestApiErrorResponse
import mocking.vision.models.linkage.LinkageKeyGetResponse
import mocking.vision.models.linkage.LinkageKeyPostRequest
import org.apache.http.HttpStatus

class VisionLinkagePOSTBuilder(orgId: String, linkageKeyPostRequest: LinkageKeyPostRequest)
    : MappingBuilder(method = "POST", url = "/vision/linkage/organisations/$orgId/onlineservices/linkage") {

    init {
        requestBuilder.andJsonBody(linkageKeyPostRequest,"equalToJson", GsonFactory.asIs)
    }

    fun respondWithSuccessfullyCreated(linkageKeyGetResponse: LinkageKeyGetResponse): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(linkageKeyGetResponse, GsonFactory.asIs)
        }
    }

    fun respondWithErrorInvalidNhsNumber(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.INVALID_NHS_NUMBER
                    )
            )
            andJsonBody(error, GsonFactory.asIs)
        }
    }

    fun respondWithErrorPatientRecordNotFound(): Mapping {
        return respondWith(HttpStatus.SC_NOT_FOUND) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.PATIENT_RECORD_NOT_FOUND
                    )
            )
            andJsonBody(error, GsonFactory.asIs)
        }
    }

    fun respondWithErrorLinkageKeyAlreadyExists(): Mapping {
        return respondWith(HttpStatus.SC_CONFLICT) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.LINKAGE_KEY_ALREADY_EXISTS
                    )
            )
            andJsonBody(error, GsonFactory.asIs)
        }
    }

    fun respondWithErrorInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR){}
    }
}
