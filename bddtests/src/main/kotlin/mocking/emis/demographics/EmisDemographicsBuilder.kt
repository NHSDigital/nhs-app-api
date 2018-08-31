package mocking.emis.demographics

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class EmisDemographicsBuilder(configuration: EmisConfiguration,
                              linkToken: String,
                              apiEndUserSessionId: String,
                              apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/demographics") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
    }

    fun respondWithSuccess(patient: Patient, patientIdentifiers: Array<PatientIdentifier>): Mapping {
        val responseBody = EmisDemographicsResponse(patient, patientIdentifiers = patientIdentifiers)

        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun respondWithSuccess(demographicResponse: EmisDemographicsResponse): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(demographicResponse)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'RecordViewer' " +
                "from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. " +
                "Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
