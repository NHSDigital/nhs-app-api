package mocking.emis.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.AppointmentSession
import mocking.emis.models.AppointmentSlot
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import org.apache.http.HttpStatus.SC_OK
import org.junit.Assert
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val TRIM_MINUTES_AND_SECONDS = 4

class AppointmentSlotsBuilderEmis(configuration: EmisConfiguration,
                                  patient: Patient,
                                  fromDateTime: ZonedDateTime? = null,
                                  toDateTime: ZonedDateTime? = null
)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointmentslots"), IAppointmentSlotsBuilder {

    init {
        val apiEndUserSessionId = patient.endUserSessionId
        val apiSessionId = patient.sessionId
        val linkToken = patient.userPatientLinkToken

        requestBuilder.andHeader(HEADER_API_END_USER_SESSION_ID, apiEndUserSessionId)
        requestBuilder.andHeader(HEADER_API_SESSION_ID, apiSessionId)

        if (fromDateTime != null) {
            // We'll only match on the date and time to the first digit of the minute,
            // as it's much harder to match on seconds without creating many stubs.
            // We are creating 1 stub for 10 mins from current minute and 1 for the next 10 minutes.
            val fromDateParts = convertDateToEmisTimeString(fromDateTime).dropLast(TRIM_MINUTES_AND_SECONDS)
            requestBuilder.andQueryParameter(
                    "fromDateTime",
                    fromDateParts,
                    "contains")
        }

        if (toDateTime != null) {
            requestBuilder.andQueryParameter(
                    "toDateTime",
                    convertDateToEmisTimeString(toDateTime))
        }

        if (!linkToken.isEmpty()) requestBuilder.andQueryParameter("userPatientLinkToken", linkToken)
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

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("appointment slots")
    }

    private fun extractSessionsFromFacade(facade: AppointmentSlotsResponseFacade): List<AppointmentSession> {
        return facade.sessions.map { session ->
            val slots = session.slots.map { slot ->
                val slotTypeName = facade.slotTypes.find { slotType -> slot.slotTypeId == slotType.slotTypeId }
                Assert.assertNotNull("Corresponding slot type not found. " +
                        "Couldn't find ${slot.slotTypeId}", slotTypeName)
                AppointmentSlot(
                        slot.slotId!!,
                        convertStringToEmisTimeString(slot.startTime!!),
                        convertStringToEmisTimeString(slot.endTime!!),
                        slotTypeName!!.slotTypeName,
                        slot.channel,
                        when (slot.telephoneNumber) {
                            "telephoneNumberToEnter" -> ""
                            else -> slot.telephoneNumber
                        }
                )
            }
            AppointmentSession(
                    session.sessionDate,
                    session.sessionId,
                    slots
            )
        }
    }

    private fun convertStringToEmisTimeString(time: String): String {
        return convertDateToEmisTimeString(ZonedDateTime.parse(time))
    }

    private fun convertDateToEmisTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(time)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithEmisUnknownError()
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
                    .andDelay(delayMillisecs)
        }
    }
}
