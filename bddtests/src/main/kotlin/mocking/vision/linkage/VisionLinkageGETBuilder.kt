package mocking.vision.linkage

import constants.ErrorResponseCodeVision
import mocking.GsonFactory
import mocking.MappingBuilder
import mocking.models.Mapping
import mocking.vision.models.error.VisionError
import mocking.vision.models.error.VisionRestApiErrorResponse
import mocking.vision.models.linkage.LinkageKeyGetResponse
import org.apache.http.HttpStatus

class VisionLinkageGETBuilder(orgId: String, nhsNumber: String)
    : MappingBuilder(method = "GET", url = "/vision/linkage/organisations/$orgId/onlineservices/linkage") {

    init {
        requestBuilder
                .andQueryParameter("nhsNumber", nhsNumber)
    }

    fun respondWithSuccessfullyRetrieved(linkageKeyGetResponse: LinkageKeyGetResponse): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(linkageKeyGetResponse, GsonFactory.asPascal)
        }
    }

    fun respondWithErrorInvalidNhsNumber(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.INVALID_NHS_NUMBER
                    )
            )
            andJsonBody(error, GsonFactory.asPascal)
        }
    }

    fun respondWithErrorPatientRecordNotFound(): Mapping {
        return respondWith(HttpStatus.SC_NOT_FOUND) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.PATIENT_RECORD_NOT_FOUND
                    )
            )
            andJsonBody(error, GsonFactory.asPascal)
        }
    }

    fun respondWithErrorLinkageKeyRevoked(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = ErrorResponseCodeVision.LINKAGE_KEY_REVOKED
                    )
            )
            andJsonBody(error, GsonFactory.asPascal)
        }
    }

    fun respondWithErrorInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR){}
    }
}
