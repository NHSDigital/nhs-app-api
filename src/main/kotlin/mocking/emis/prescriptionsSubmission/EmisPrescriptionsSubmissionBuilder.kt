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
        return respondWithException(-1002, "There is already a pending request in the past 30 days for one or more of the medication requested: 2dab3471-e5aa-4d04-bd7c-ab11baa55751")
    }

    fun respondWithPrescriptionsNotEnabled(): Mapping {
        return respondWithException(-1030, "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'RepeatPrescribing' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, AppointmentBooking, RecordViewer, SharedRecordAuditView'. Extra info: Services Access violation")
    }

    fun respondWithGenericInternalServerError(): Mapping {
        return respondWithException(0, "")
    }

    fun respondWithBadRequestErrorIndicatingACourseIsInvalid(): Mapping {
        return respondWithBadRequest("The request is invalid.", "requestModel.MedicationCourseGuids[0]")
    }
}
