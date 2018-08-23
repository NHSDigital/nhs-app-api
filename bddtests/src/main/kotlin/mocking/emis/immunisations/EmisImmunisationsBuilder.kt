package mocking.emis.immunisations

import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_OK

class EmisImmunisationsBuilder(configuration: EmisConfiguration,
                               linkToken: String,
                               apiEndUserSessionId: String,
                               apiSessionId: String)
    : EmisMappingBuilder(configuration, "GET", "/record") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)
                .andQueryParameter("userPatientLinkToken", linkToken, "equalTo")
                .andQueryParameter("itemType", "Immunisations", "equalTo")
    }

    fun respondWithSuccess(model: ImmunisationResponseModel): Mapping {
        return respondWith(SC_OK) {
            andJsonBody(model)
                    .build()
        }
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_INTERNAL_SERVER_ERROR.toLong(),
                "Requested record access is disabled by the practice")
        return respondWithException(exceptionResponse)
    }

    fun respondWithNonDataAccessException(): Mapping {
        val exceptionResponse = ExceptionResponse(HttpStatus.SC_SERVICE_UNAVAILABLE.toLong(),
                "An Exception Occurred")
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
