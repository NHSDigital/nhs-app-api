package mocking.microtest

import mocking.GsonFactory
import mocking.microtest.prescriptions.PatientRequest
import mocking.microtest.prescriptions.PrescriptionOrderPartialSuccessResponse
import mocking.microtest.prescriptions.PrescriptionRequestPost
import mocking.models.Mapping
import mockingFacade.prescriptions.PartialSuccessFacade
import org.apache.http.HttpStatus

class PrescriptionSubmissionBuilderMicrotest (
        nhsNumber: String,
        odsCode: String,
        prescriptionRequestsPost: PrescriptionRequestPost?)
    : MicrotestMappingBuilder("POST", "/prescriptions") {

    init {
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, nhsNumber)

        if (prescriptionRequestsPost != null) {

            requestBuilder.andJsonBody(prescriptionRequestsPost)
        }
    }

    fun respondWithCreated(): Mapping {
        return respondWith(HttpStatus.SC_CREATED) { build() }
    }

    fun respondWithPartiallySuccessful(partialSuccess: PartialSuccessFacade) : Mapping {
        val body = PrescriptionOrderPartialSuccessResponse()

        for (i in partialSuccess.successfulMedications.indices) {
            body.PatientRequests.add(PatientRequest(
                    "success_$i",
                    "${partialSuccess.successfulMedications[i]}",
                    "success",
                    ""))
        }

        for (i in partialSuccess.unsuccessfulMedications.indices) {
            body.PatientRequests.add(PatientRequest(
                    "failed_$i",
                    "${partialSuccess.unsuccessfulMedications[i]}",
                    "failed",
                    ""))
        }

        return respondWith(HttpStatus.SC_ACCEPTED) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
