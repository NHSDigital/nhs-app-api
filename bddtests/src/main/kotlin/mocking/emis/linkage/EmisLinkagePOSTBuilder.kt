package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.ExceptionResponse
import mocking.emis.models.GetLinkageResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import worker.models.linkage.CreateLinkageRequest

class EmisLinkagePOSTBuilder(createLinkageRequest: CreateLinkageRequest)
    : EmisMappingBuilder(null, method = "POST", relativePath = "/patient/linkage") {

    init {
        requestBuilder.andJsonBody(createLinkageRequest,"equalToJson", GsonFactory.asPascal)
    }

    fun respondWithSuccess(getLinkageResponse: GetLinkageResponse): Mapping {

        return respondWithSuccessAny(getLinkageResponse)
    }

    private fun respondWithSuccessAny(body: Any): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(body, GsonFactory.asPascal)
        }
    }

    fun respondWithConflictException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Conflict Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_CONFLICT)
    }

    fun respondWithNotFoundException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Not Found Exception")
        return respondWithException(exceptionResponse, HttpStatus.SC_NOT_FOUND)
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
