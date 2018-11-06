package mocking.emis

import constants.ErrorResponseCodeEmis
import mocking.MappingBuilder
import mocking.emis.models.BadRequestResponse
import mocking.emis.models.ErrorResponse
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus

const val HEADER_API_APPLICATION_ID = "X-API-ApplicationId"
const val HEADER_API_END_USER_SESSION_ID = "X-API-EndUserSessionId"
const val HEADER_API_SESSION_ID = "X-API-SessionId"
const val HEADER_API_VERSION = "X-API-Version"
const val QUERY_PARAM_USER_PATIENT_LINK_TOKEN = "userPatientLinkToken"

open class EmisMappingBuilder(configuration: EmisConfiguration?,
                              method: String, relativePath: String = "")
    : MappingBuilder(method, "/emis$relativePath") {

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
        return respondWithStandardError(ErrorResponseCodeEmis.SERVICE_ACCESS_VIOLATION.toInt(), HttpStatus.SC_FORBIDDEN)
    }

    fun respondWithStandardError(internalResponseCode: Int, httpResponseCode: Int): Mapping {
        val responseBody = ErrorResponse(internalResponseCode)
        return respondWith(httpResponseCode) {
            andJsonBody(responseBody)
                    .build()
        }
    }

    fun respondWithCorruptedContent(content: String): Mapping {
        return respondWith(HttpStatus.SC_OK) { andHtmlBody(content) }
    }

    override fun respondWithServiceUnavailable(content: String): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andJsonBody(content)
        }
    }

    protected fun respondWithException(internalResponseCode: Int, message: String): Mapping {

        val responseBody = ExceptionResponse(internalResponseCode.toLong(), message)

        return respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR) {
            andJsonBody(responseBody)
                    .build()
        }
    }
}
