package mocking.emis.prescriptionsSubmission

import mocking.emis.*
import mocking.models.Mapping
import org.apache.http.HttpStatus
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest

class EmisPrescriptionsSubmissionBuilder(
        configuration: EmisConfiguration,
        apiEndUserSessionId: String?,
        apiSessionId: String?,
        userPatientLinkToken: String?,
        prescriptionSubmissionRequest: PrescriptionSubmissionRequest?)
    : EmisMappingBuilder(configuration, method = "POST", relativePath = "/prescriptionrequests") {

    init {
        if (apiEndUserSessionId != null) {
            requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        }

        if (apiSessionId != null) {
            requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)
        }

        if (prescriptionSubmissionRequest != null) {
            val emisRequest = mocking.emis.models.PrescriptionSubmissionRequest(prescriptionSubmissionRequest.courseIds, prescriptionSubmissionRequest.specialRequest)

            if (userPatientLinkToken != null) {
                emisRequest.UserPatientLinkToken = userPatientLinkToken
            }

            requestBuilder.andJsonBody(emisRequest)
        }
    }

    fun respondWithCreated(): Mapping {
        return respondWith(HttpStatus.SC_CREATED) { build() }
    }

    fun respondWithAlreadyAPendingRequestInTheLast30Days(): Mapping {
        return respondWithStandardError(-1455, HttpStatus.SC_CONFLICT)
    }

    fun respondWithPrescriptionsNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    fun respondWithGenericInternalServerError(): Mapping {
        return respondWithException(0, "")
    }

    fun respondWithBadRequestErrorIndicatingACourseIsInvalid(): Mapping {
        return respondWithBadRequest("The request is invalid.", "requestModel.MedicationCourseGuids[0]")
    }
}
