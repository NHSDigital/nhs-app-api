package mocking.emis.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val UNKNOWN_HTTP_STATUS_CODE: Long = -9999

class AppointmentSlotsBuilderEmis(configuration: EmisConfiguration,
                                  patient: Patient,
                                  fromDateTime: String?,
                                  toDateTime: String?)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots"), IAppointmentSlotsBuilder {

    init {
        val apiEndUserSessionId = patient.endUserSessionId
        val apiSessionId = patient.sessionId
        val linkToken = patient.userPatientLinkToken

        requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (!fromDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(
                name = "fromDateTime", value = convertDateToEmisTime(fromDateTime!!))
        if (!toDateTime.isNullOrEmpty()) requestBuilder.andQueryParameter(
                name = "toDateTime", value = convertDateToEmisTime(toDateTime!!))
        if (!linkToken.isEmpty()) requestBuilder.andQueryParameter(
                name = "userPatientLinkToken", value = linkToken)
    }

    override fun withDelay(delayMilliseconds: Duration): AppointmentSlotsBuilderEmis {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        return respondWithBody(
                GetAppointmentSlotsResponseModel(
                        extractSessionsFromFacade(facade)
                )
        )
    }

    private fun extractSessionsFromFacade(facade: AppointmentSlotsResponseFacade): List<AppointmentSession> {
        return facade.sessions.map { session ->
            val slots = session.slots.map { slot ->
                AppointmentSlot(
                        slot.slotId!!,
                        convertDateToEmisTime(slot.startTime!!),
                        convertDateToEmisTime(slot.endTime!!),
                        slot.slotTypeName
                )
            }
            AppointmentSession(
                    session.sessionDate,
                    session.sessionId,
                    slots
            )
        }
    }

    private fun convertDateToEmisTime(time: String): String {
        val dateToPass = ZonedDateTime.parse(time)
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(dateToPass)
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(UNKNOWN_HTTP_STATUS_CODE,
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