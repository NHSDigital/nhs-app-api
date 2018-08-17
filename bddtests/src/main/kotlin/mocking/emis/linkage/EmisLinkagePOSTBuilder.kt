package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.*
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisLinkagePOSTBuilder(addNhsUserRequest: AddNhsUserRequest)
    : EmisMappingBuilder(null, method = "POST", relativePath = "/users/nhs") {

    init {
        requestBuilder.andJsonBody(addNhsUserRequest,"equalToJson", GsonFactory.asPascal)
    }

    fun respondWithSuccessfullyCreated(addNhsUserResponse: AddNhsUserResponse): Mapping {
        return respondWith(HttpStatus.SC_CREATED) {
            andJsonBody(addNhsUserResponse, GsonFactory.asPascal)
        }
    }

    fun respondWithPatientAlreadyHasAnOnlineAccount(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_CONFLICT)
    }

    private fun respondWithStandardErrorResponse(errorResponse: ErrorResponse, httpStatus: Int): Mapping {
        return respondWithBody(errorResponse, httpStatus)
    }

    fun respondWithNoRegisteredOnlineUserFound(): Mapping {
        val errorResponse = ErrorResponse(-1104)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithPracticeNotLive(): Mapping {
        val errorResponse = ErrorResponse(-1401)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientMarkedAsArchived(): Mapping {
        val errorResponse = ErrorResponse(-1552)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientNonCompetentOrUnder16(): Mapping {
        val errorResponse = ErrorResponse(-1553)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithInternalServerError(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
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
