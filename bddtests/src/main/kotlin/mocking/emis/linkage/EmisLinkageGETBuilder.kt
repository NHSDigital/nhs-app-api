package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.ExceptionResponse
import mocking.emis.models.GetLinkageResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisLinkageGETBuilder(nhsNumber: String, odsCode: String)
    : EmisMappingBuilder(null, method = "GET", relativePath = "/patient/linkage") {

    init {
        requestBuilder
                .andHeader(HEADER_NHS_NUMBER, nhsNumber)
                .andHeader(HEADER_ODS_CODE, odsCode)
    }

    fun respondWithSuccess(getLinkageResponse: GetLinkageResponse): Mapping {

        return respondWithSuccessAny(getLinkageResponse)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }

    fun respondWithNotFoundException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Not Found Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithForbiddenException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Forbidden Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_FORBIDDEN)
    }

    fun respondWithNotImplementedException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Not Implemented")
        return respondWithException(exceptionResponse, HttpStatus.SC_NOT_IMPLEMENTED)
    }

    fun respondWithBadGatewayException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Bad Gateway")
        return respondWithException(exceptionResponse, HttpStatus.SC_BAD_GATEWAY)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse, httpStatus: Int): Mapping {
        return respondWithBody(exceptionResponse, httpStatus)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }

}
