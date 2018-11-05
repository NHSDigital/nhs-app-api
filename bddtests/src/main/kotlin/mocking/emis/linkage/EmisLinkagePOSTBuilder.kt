package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import constants.ErrorResponseCodeEmis
import mocking.GsonFactory
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
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
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.NO_REGISTERED_ONLINE_USER_FOUND.toInt())
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithPracticeNotLive(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PRACTICE_NOT_LIVE.toInt())
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientMarkedAsArchived(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PATIENT_MARKED_AS_ARCHIVED.toInt())
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientNonCompetentOrUnder16(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PATIENT_NON_COMPETENT_OR_UNDER_16.toInt())
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithInternalServerError(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    fun respondWithBadGatewayException(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.EXCEPTION,
                "Bad Gateway")
        return respondWithBody(exceptionResponse, HttpStatus.SC_BAD_GATEWAY)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }

}
