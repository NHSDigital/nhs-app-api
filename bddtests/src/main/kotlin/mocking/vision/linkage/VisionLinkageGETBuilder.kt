package mocking.vision.linkage

import constants.ErrorResponseCodeVision
import mocking.GsonFactory
import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import mocking.vision.models.error.VisionError
import mocking.vision.models.error.VisionRestApiErrorResponse
import mocking.vision.models.linkage.LinkageKeyGetResponse
import org.apache.http.HttpStatus

class VisionLinkageGETBuilder(orgId: String, nhsNumber: String) : IErrorMappingBuilder,
        MappingBuilder(method = "GET", url = "/vision/linkage/organisations/$orgId/onlineservices/linkage") {

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

    fun respondWithErrorNoUserAssociatedWithNHSNumber(): Mapping {
        return respondWithError(
                HttpStatus.SC_NOT_FOUND,
                ErrorResponseCodeVision.UNIQUE_RECORD_COULD_NOT_BE_FOUND,
                "No user associated with the nhs number."
        )
    }

    fun respondWithErrorNoApiKeyAssociatedWithNHSNumber(): Mapping {
        return respondWithError(
                HttpStatus.SC_NOT_FOUND,
                ErrorResponseCodeVision.UNIQUE_RECORD_COULD_NOT_BE_FOUND,
                "No API key associated with the nhs number."
        )
    }

    fun respondWithErrorInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR){}
    }

    override fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        return respondWith(httpStatusCode) {
            val error = VisionRestApiErrorResponse(
                    VisionError(
                            code = errorCode,
                            diagnostic = message?:""
                    )
            )
            andJsonBody(error, GsonFactory.asPascal)
        }
    }
}

