package mocking.microtest

import mocking.microtest.prescriptions.PrescriptionRequestPost
import mocking.models.Mapping
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
}
