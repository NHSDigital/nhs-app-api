package mocking.microtest

import mocking.MappingBuilder
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.SerenityHelpers

const val HEADER_API_ODS_CODE = "NHSO-ODS-Code"
const val HEADER_API_NHS_NUMBER = "NHSO-Nhs-Number"

open class MicrotestMappingBuilder(method: String, relativePath: String = "")
    : MappingBuilder(method, "/microtest/patient$relativePath"), IErrorMappingBuilder {

    init {
        val patient = SerenityHelpers.getPatient()
        requestBuilder
                .andHeader(HEADER_API_ODS_CODE, patient.odsCode)
                .andHeader(HEADER_API_NHS_NUMBER, patient.nhsNumbers.first())
    }

    var appointments = MicrotestMappingBuilderAppointments()
    var demographics = MicrotestMappingBuilderDemographics()
    var myRecord = MicrotestMappingBuilderMyRecord()
    var prescriptions = MicrotestMappingBuilderPrescriptions()

    fun respondWithCorruptedContent(content: String): Mapping {
        return respondWith(HttpStatus.SC_OK) { andHtmlBody(content) }
    }

    fun respondWithForbiddenError(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) {  andJsonBody("""
            {
                "Error": "The patient does not have the necessary permissions within the GP system. (appointments)"
            }
        """.trimIndent()) }
    }
    fun respondWithBadGateway(): Mapping {
        return respondWith(HttpStatus.SC_BAD_GATEWAY){}
    }

    override fun respondWithServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andJsonBody("")
        }
    }

    fun respondWithUnknownExceptionError(): Mapping {
        return respondWith(HttpStatus.SC_BAD_GATEWAY) {
            andJsonBody("")
        }
    }

    fun respondWithInternalServerError(): Mapping {
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody("")
        }
    }

    fun respondWithConflictError(): Mapping {
        return respondWith(HttpStatus.SC_CONFLICT){ andJsonBody(""" +
            {
                ""Error"" : ""Conflict. The chosen appointment slot is not available for booking.""
            }
            """.trimIndent()) }
    }

    fun respondWithExceptionWhenRequiredFieldMissing(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andJsonBody("")
        }
    }

    override fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        Assert.assertEquals("Test set up is incorrect, Microtest does not support error codes", "", errorCode)
        Assert.assertEquals("Test set up is incorrect, Microtest does not support error messages", "", message)
        return respondWith(httpStatusCode) {}
    }
}
