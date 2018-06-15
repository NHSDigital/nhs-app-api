package mocking.emis.medications

import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.*
import mocking.models.Mapping
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_OK

class EmisMedicationsBuilder(configuration: EmisConfiguration,
                           linkToken: String,
                           apiEndUserSessionId: String,
                           apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/record") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
                .andQueryParameter("itemType", "Medication", "equalTo")
    }

    fun respondWithSuccess(medicationsResponse: MedicationsResponse): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(medicationsResponse)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(500,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'RecordViewer' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}
