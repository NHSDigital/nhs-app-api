package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK

class EmisAppointmentSlotsMetaBuilder(configuration: EmisConfiguration,
                                      apiEndUserSessionId: String,
                                      apiSessionId: String,
                                      sessionStartDate: String? = null,
                                      sessionEndDate: String? = null,
                                      userPatientLinkToken: String? = null)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots/meta") {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (sessionStartDate != null) requestBuilder.andQueryParameter("sessionStartDate", sessionStartDate)
        if (sessionEndDate != null) requestBuilder.andQueryParameter("sessionEndDate", sessionEndDate)
        if (userPatientLinkToken != null) requestBuilder.andQueryParameter("userPatientLinkToken", userPatientLinkToken)
    }

    fun respondWithSuccess(model: GetAppointmentSlotsMetaResponseModel): Mapping {
        return respondWithBody(model)
    }

    fun respondWithExceptionWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(-1030,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services 'AppointmentBooking' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked patient. Available services are 'AddressChange, RecordViewer, RepeatPrescribing, SharedRecordAuditView'. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(-9999,
                "Unknown Exception")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.UPPER_CAMEL_CASE)
                    .create())
        }
    }
}