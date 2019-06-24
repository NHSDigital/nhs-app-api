package mocking.microtest

import mocking.GsonFactory
import mocking.microtest.prescriptions.CoursesGetResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class CoursesBuilderMicrotest (
        nhsNumber: String,
        odsCode: String)
    : MicrotestMappingBuilder("GET", "/courses") {

    init {
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, nhsNumber)
    }

    fun respondWithSuccess(coursesGetResponse: CoursesGetResponse): Mapping {
        return respondWithSuccessAny(coursesGetResponse)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
