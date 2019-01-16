package mocking.emis.appointments

import constants.DateTimeFormats
import constants.ErrorResponseCodeEmis
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.appointments.helpers.AppointmentSlotsMetaHelper
import mocking.emis.models.ExceptionResponse
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_INTERNAL_SERVER_ERROR
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val TRIM_MINUTES_AND_SECONDS = 4

class AppointmentSlotsMetaBuilderEmis(
        configuration: EmisConfiguration,
        apiEndUserSessionId: String,
        apiSessionId: String,
        sessionStartDate: ZonedDateTime? = null,
        sessionEndDate: ZonedDateTime? = null,
        userPatientLinkToken: String? = null)
    : EmisMappingBuilder(configuration, method = "GET",
        relativePath = "/appointmentslots/meta"), IAppointmentSlotsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (sessionStartDate != null) {
            // We'll only match on the date and time to the first digit of the minute,
            // as it's much harder to match on seconds without creating many stubs.
            // We are creating 1 stub for 10 mins from current minute and 1 for the next 10 minutes
            val fromDateParts = convertDateToEmisTimeString(sessionStartDate).dropLast(TRIM_MINUTES_AND_SECONDS)
            requestBuilder.andQueryParameter(
                    "sessionStartDate",
                    fromDateParts,
                    "contains")
        }

        if (sessionEndDate != null) {
            requestBuilder.andQueryParameter("sessionEndDate", convertDateToEmisTimeString(sessionEndDate))
        }

        requestBuilder.andQueryParameterIfNotNull("userPatientLinkToken", userPatientLinkToken)
    }

    override fun withDelay(delayMilliseconds: Duration): AppointmentSlotsMetaBuilderEmis {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        val locations = AppointmentSlotsMetaHelper.getMetaSlotLocationsList(facade.sessions)
        val sessionHolders = AppointmentSlotsMetaHelper.getMetaSlotSessionHoldersList(facade.sessions)
        val slotSessions = AppointmentSlotsMetaHelper.getMetaSlotSessionsList(facade.sessions)

        val appointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                locations,
                sessionHolders,
                slotSessions
        )
        return respondWithSuccess(appointmentSlotsMetaResponseModel)
    }

    fun respondWithSuccess(model: GetAppointmentSlotsMetaResponseModel): Mapping {
        return respondWithBody(model)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        val exceptionResponse = ExceptionResponse(ErrorResponseCodeEmis.SERVICE_ACCESS_VIOLATION,
                "User Identity 'efa22020-9221-46a6-a0f0-6c0340b8f44d' requested services " +
                        "'AppointmentBooking' from Application 'd66ba979-60d2-49aa-be82-aec06356e41f' for linked " +
                        "patient. Available services are 'AddressChange, RecordViewer, RepeatPrescribing, " +
                        "SharedRecordAuditView'. Extra info: Services Access violation")
        return respondWithException(exceptionResponse)
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithEmisUnknownError()
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("appointment slots metadata")
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
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

    private fun convertDateToEmisTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(time)
    }
}
