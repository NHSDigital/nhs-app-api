package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.emis.*
import mocking.emis.models.ExceptionResponse
import mocking.emis.models.AddVerificationResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import mocking.emis.models.AddVerificationRequest
import mocking.emis.models.ErrorResponse

class EmisLinkageGETBuilder(addVerificationRequest: AddVerificationRequest)
    : EmisMappingBuilder(null, method = "POST", relativePath = "/me/verifications") {

    init {
        requestBuilder.andJsonBody(addVerificationRequest,"equalToJson", GsonFactory.asPascalSerializeNulls)
    }

    fun respondWithSuccessfullyRetrieved(addVerificationResponse: AddVerificationResponse): Mapping {
        // This isn't a mistake, emis returns conflict in this scenario.
        return respondWith(HttpStatus.SC_CONFLICT) {
            andJsonBody(addVerificationResponse, GsonFactory.asPascal)
        }
    }

    fun respondWithSuccessfullyRetrievedFirstTime(addVerificationResponse: AddVerificationResponse): Mapping {
        return respondWith(HttpStatus.SC_CREATED) {
            andJsonBody(addVerificationResponse, GsonFactory.asPascal)
        }
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

    fun respondWithInternalServerError(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    fun respondWithPatientNotRegisteredAtPractice(): Mapping {
        val errorResponse = ErrorResponse(-1551)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_NOT_FOUND)
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

    fun respondWithAccountStatusInvalid(): Mapping {
        val errorResponse = ErrorResponse(-1107)
        return respondWithStandardErrorResponse(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    private fun respondWithStandardErrorResponse(errorResponse: ErrorResponse, httpStatus: Int): Mapping {
        return respondWithBody(errorResponse, httpStatus)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}
