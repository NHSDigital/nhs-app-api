package mocking.emis.appointments

import com.google.gson.FieldNamingPolicy
import com.google.gson.GsonBuilder
import mocking.GsonFactory
import mocking.defaults.MockDefaults
import mocking.emis.*
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import mocking.emis.models.ExceptionResponse
import mocking.models.Mapping
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK

class EmisAppointmentSlotsBuilder(configuration: EmisConfiguration,
                                  apiEndUserSessionId: String,
                                  apiSessionId: String,
                                  fromDateTime: String?,
                                  toDateTime: String?,
                                  linkToken: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots") {

    init {
        if (apiEndUserSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, MockDefaults.patient.endUserSessionId)
        else requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        if (apiSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_SESSION_ID, MockDefaults.patient.sessionId)
        else requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (!fromDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "fromDateTime", value = fromDateTime!!)
        if (!toDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "toDateTime", value = toDateTime!!)
        if (!linkToken.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "userPatientLinkToken", value = linkToken!!)
    }

    fun respondWithSuccess(slots: ArrayList<AppointmentSlot>, sessionId: Int?, sessionDate: String?): Mapping {
        val session = AppointmentSession(sessionDate, sessionId, slots)
        val model = GetAppointmentSlotsResponseModel(arrayListOf(session))
        return respondWithBody(model)
    }
    fun respondWithSuccess(model: GetAppointmentSlotsResponseModel): Mapping {
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
            andJsonBody(body, GsonFactory.asPascal)
        }
    }
}