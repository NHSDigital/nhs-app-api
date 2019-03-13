package mocking.microtest

import mocking.MappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus
import utils.SerenityHelpers

const val HEADER_API_ODS_CODE = "NHSO-ODS-Code"
const val HEADER_API_NHS_NUMBER = "NHSO-Nhs-Number"

open class MicrotestMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, "/microtest/patient$relativePath") {

    init {
        val patient = SerenityHelpers.getPatient()
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, patient.odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, patient.nhsNumbers.first())
    }

    var appointments = MicrotestMappingBuilderAppointments()

    fun respondWithCorruptedContent(content: String): Mapping {
        return respondWith(HttpStatus.SC_OK) { andHtmlBody(content) }
    }

    override fun respondWithServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andJsonBody("")
        }
    }
}
