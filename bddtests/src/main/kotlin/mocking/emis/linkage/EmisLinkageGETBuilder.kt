package mocking.emis.linkage

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisMappingBuilder
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
        val exceptionResponse = ExceptionResponse(EmisResponseCode.EXCEPTION,
                "Forbidden Exception")
        return respondWithBody(exceptionResponse, HttpStatus.SC_FORBIDDEN)
    }

    fun respondWithNotImplementedException(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.EXCEPTION,
                "Not Implemented")
        return respondWithBody(exceptionResponse, HttpStatus.SC_NOT_IMPLEMENTED)
    }

    fun respondWithBadGatewayException(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.EXCEPTION,
                "Bad Gateway")
        return respondWithBody(exceptionResponse, HttpStatus.SC_BAD_GATEWAY)
    }

    fun respondWithInternalServerError(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithBody(errorResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    fun respondWithPatientNotRegisteredAtPractice(): Mapping {
        val errorResponse = ErrorResponse(EmisResponseCode.PATIENT_NOT_REGISTERED_AT_PRACTICE.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithNoRegisteredOnlineUserFound(): Mapping {
        val errorResponse = ErrorResponse(EmisResponseCode.NO_REGISTERED_ONLINE_USER_FOUND.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithPracticeNotLive(): Mapping {
        val errorResponse = ErrorResponse(EmisResponseCode.PRACTICE_NOT_LIVE.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientMarkedAsArchived(): Mapping {
        val errorResponse = ErrorResponse(EmisResponseCode.PATIENT_MARKED_AS_ARCHIVED.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientNonCompetentOrUnder16(): Mapping {
        val errorResponse = ErrorResponse(EmisResponseCode.PATIENT_NON_COMPETENT_OR_UNDER_16.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithAccountStatusInvalid(): Mapping {
        val errorResponse = ErrorResponse(-EmisResponseCode.ACCOUNT_STATUS_INVALID.toInt())
        return respondWithBody(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}
