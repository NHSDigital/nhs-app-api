package mocking.emis

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import constants.ErrorResponseCodeEmis
import mocking.JSonXmlConverter
import mocking.MappingBuilder
import mocking.emis.models.BadRequestResponse
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
import mocking.emis.practices.PracticeSettingsBuilderEmis
import mocking.gpServiceBuilderInterfaces.IErrorMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

const val HEADER_API_APPLICATION_ID = "X-API-ApplicationId"
const val HEADER_API_END_USER_SESSION_ID = "X-API-EndUserSessionId"
const val HEADER_API_SESSION_ID = "X-API-SessionId"
const val HEADER_API_VERSION = "X-API-Version"
const val QUERY_PARAM_USER_PATIENT_LINK_TOKEN = "userPatientLinkToken"

open class EmisMappingBuilder(configuration: EmisConfiguration?,
                              method: String, relativePath: String = "") : IErrorMappingBuilder,
        MappingBuilder(method, "/emis$relativePath") {

    init {
        if (configuration != null) {
            requestBuilder
                    .andHeader(HEADER_API_APPLICATION_ID, configuration.applicationId)
                    .andHeader(HEADER_API_VERSION, configuration.version)
        }
    }

    var appointments = EmisMappingBuilderAppointments(configuration)

    var myRecord = EmisMappingBuilderMyRecord(configuration)

    var prescriptions = EmisMappingBuilderPrescriptions(configuration)

    var authentication = EmisMappingBuilderAuthentication(configuration, method)

    fun respondWithBadRequest(message: String, fieldName: String): Mapping {
        val responseBody = BadRequestResponse(message, fieldName)
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun responseErrorForbiddenService(): Mapping {
        return respondWithStandardError(ErrorResponseCodeEmis.SERVICE_ACCESS_VIOLATION.toInt(), HttpStatus
                .SC_FORBIDDEN)
    }

    fun respondWithCorruptedContent(content: String): Mapping {
        return respondWith(HttpStatus.SC_OK) { andHtmlBody(content) }
    }

    fun respondWithEmisNotAuthorised(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andBody("Missing or invalid EndUserSessionId", "application/json")
        }
    }

    fun respondWithEmisUnknownError(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.UNKNOWN_EXCEPTION.toString(),
                "Unknown Exception")
        val responseBody = JSonXmlConverter.toJson(exceptionResponse)
        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody).build()
        }
    }

    override fun respondWithServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andJsonBody("")
        }
    }

    protected fun respondWithException(internalResponseCode: Int, message: String): Mapping {

        val responseBody = ExceptionResponse(internalResponseCode.toString(), message)

        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun practiceSettingsRequest(patient: Patient) = PracticeSettingsBuilderEmis(patient)

    fun respondWithStandardError(internalResponseCode: Int, httpResponseCode: Int, message: String = ""): Mapping {
        val responseBody = ErrorResponse(internalResponseCode, message)
        return respondWith(httpResponseCode) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    override fun respondWithError(httpStatusCode: Int, errorCode: String, message: String?): Mapping {
        val responseBody = ExceptionResponse(errorCode, message?:"")
        return respondWith(httpStatusCode) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    protected fun respondWithBodyAndStatus(body: Any, statusCode: Int = HttpStatus.SC_CREATED): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}
