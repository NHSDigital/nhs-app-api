package mocking.emis.linkage

import constants.ErrorResponseCodeEmis
import mocking.GsonFactory
import mocking.emis.EmisMappingBuilder
import mocking.emis.models.AddVerificationRequest
import mocking.emis.models.AddVerificationResponse
import mocking.emis.models.ErrorResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

class EmisLinkageGETBuilder(addVerificationRequest: AddVerificationRequest) :
        EmisMappingBuilder(null, method = "POST", relativePath = "/me/verifications") {

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

    fun respondWithInternalServerError(): Mapping {
        val errorResponse = ErrorResponse(0)
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    fun respondWithPatientNotRegisteredAtPractice(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PATIENT_NOT_REGISTERED_AT_PRACTICE.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithNoRegisteredOnlineUserFound(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.NO_REGISTERED_ONLINE_USER_FOUND.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_NOT_FOUND)
    }

    fun respondWithPracticeNotLive(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PRACTICE_NOT_LIVE.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientMarkedAsArchived(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PATIENT_MARKED_AS_ARCHIVED.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithPatientNonCompetentOrUnderMinimumAge(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.PATIENT_NON_COMPETENT_OR_UNDER_MINIMUM_AGE.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithAccountStatusInvalid(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.ACCOUNT_STATUS_INVALID.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }

    fun respondWithMultipleRecordsFound(): Mapping {
        val errorResponse = ErrorResponse(ErrorResponseCodeEmis.MULTIPLE_RECORDS_FOUND_WITH_NHS_NUMBER.toInt())
        return respondWithBodyAndStatus(errorResponse, HttpStatus.SC_BAD_REQUEST)
    }
}
