package mocking.emis.appointments

import mocking.GsonFactory
import mocking.defaults.MockDefaults
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration

class AppointmentSlotsBuilderEmis(configuration: EmisConfiguration,
                                  apiEndUserSessionId: String,
                                  apiSessionId: String,
                                  fromDateTime: String?,
                                  toDateTime: String?,
                                  linkToken: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots"), IAppointmentSlotsBuilder {

    init {
        if (apiEndUserSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, MockDefaults.patient.endUserSessionId)
        else requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        if (apiSessionId.isEmpty()) requestBuilder.andHeader(HEADER_API_SESSION_ID, MockDefaults.patient.sessionId)
        else requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (!fromDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "fromDateTime", value = fromDateTime!!)
        if (!toDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "toDateTime", value = toDateTime!!)
        if (!linkToken.isNullOrEmpty()) requestBuilder.andQueryParameter(name = "userPatientLinkToken", value = linkToken!!)
    }

    override fun withDelay(delayMilliseconds : Duration):AppointmentSlotsBuilderEmis{
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(model: AppointmentSlotsResponseFacade): Mapping {
        return respondWithBody(model)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithUnknownException(): Mapping {
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
                    .andDelay(delayMillisecs)
        }
    }
}