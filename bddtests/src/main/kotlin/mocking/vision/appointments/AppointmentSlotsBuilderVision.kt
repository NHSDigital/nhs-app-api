package mocking.vision.appointments
import constants.DateTimeFormats
import constants.DateTimeFormats.Companion.dateWithoutTimeFormat
import mocking.JSonXmlConverter
import constants.ErrorResponseCodeVision
import mocking.JSonXmlConverter.wrapAroundXmlTag
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionMappingBuilder
import mocking.vision.appointments.AppointmentSlotsBuilderVision.Companion.FOUR_WEEKS_OF_DAYS_PLUS_ONE
import mocking.vision.appointments.helpers.AppointmentsSlotsHelper
import mocking.vision.appointments.helpers.GeneralAppointmentsHelper
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.AvailableAppointmentsResponse
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import java.time.Duration
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class AppointmentSlotsBuilderVision(
        patient: Patient,
        fromDateString: String?,
        toDateString: String?
) : VisionMappingBuilder(), IAppointmentSlotsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.availableAppointmentsName,
            VisionConstants.availableAppointmentsVersion
    )

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val userSession = VisionUserSession(
                patient.rosuAccountId,
                patient.apiKey,
                patient.odsCode,
                patient.patientId
        )

        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
                .andBody(userSession.patientId, "contains")
                .andBody(
                        wrapAroundXmlTag("vision:dateRange",
                                wrapAroundXmlTag(
                                        "vision:from",
                                        convertDateToVisionTime(fromDateString ?: getDefaultFromDate())
                                ).plus(

                                        wrapAroundXmlTag(
                                                "vision:to",
                                                convertToDateToVisionTime(toDateString ?: getDefaultToDate())
                                        )
                                )
                        ),
                        "contains")
    }

    override fun withDelay(delayMilliseconds: Duration): IAppointmentSlotsBuilder {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        val availableAppointmentsResponse =
                if (!facade.sessions.isEmpty())
                    AppointmentsSlotsHelper.extractResponseFromFacade(facade)
                else
                    AvailableAppointmentsResponse()

        val locationsForPatient = Serenity.sessionVariableCalled<List<Location>>(
                GeneralAppointmentsHelper.Companion.VisionMetadata.LOCATIONS
        )
        val ownersForPatient = Serenity.sessionVariableCalled<List<Owner>>(
                GeneralAppointmentsHelper.Companion.VisionMetadata.OWNERS
        )

        requestBuilder.andBody(
                wrapAroundXmlTag("vision:owners",
                        ownersForPatient.joinToString("") { owner ->
                            wrapAroundXmlTag("vision:owner", owner.id.toString())
                        }

                ), "contains")
                .andBody(wrapAroundXmlTag("vision:locations",
                        locationsForPatient.joinToString("") { location ->
                            wrapAroundXmlTag("vision:location", location.id.toString())
                        }

                ), "contains")
        val responseBody = JSonXmlConverter.toXML(availableAppointmentsResponse)

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstants.getVisionAvailableAppointmentsResponse(responseBody, serviceDefinition))
                    .andDelay(delayMillisecs).build()
        }
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenNotEnabled() is not yet implemented in " +
                        "AppointmentsSlotsVision")
    }

    override fun respondWithUnknownException(): Mapping {

        return respondWith(HttpStatus.SC_OK) {
                andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                        serviceDefinition,
                        ErrorResponseCodeVision.NON_VISION_ERROR_CODE)).build()
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return responseWithCorruptedContent(serviceDefinition, "")
    }

    override fun respondWithUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }

    private fun convertDateToVisionTime(time: String): String {
        val dateToPass = ZonedDateTime.parse(time)
        val queryDateFormat = DateTimeFormatter.ofPattern(dateWithoutTimeFormat)
        return queryDateFormat.format(dateToPass)
    }

    private fun convertToDateToVisionTime(time: String): String {
        val dateToPass = ZonedDateTime.parse(time).minusDays(1)
        val queryDateFormat = DateTimeFormatter.ofPattern(dateWithoutTimeFormat)
        return queryDateFormat.format(dateToPass)
    }

    companion object {
        const val FOUR_WEEKS_OF_DAYS_PLUS_ONE = 29L
    }
}

fun getDefaultFromDate(): String {
    val now = ZonedDateTime.now(ZoneId.of("Europe/London"))
    return now.format(
            DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
    )
}

fun getDefaultToDate(): String {
    val fourWeeksTomorrow = ZonedDateTime.now(ZoneId.of("Europe/London")).plusDays(FOUR_WEEKS_OF_DAYS_PLUS_ONE)
    val fourWeeksTomorrowAtMidnight = setToMidnight(fourWeeksTomorrow)
    return fourWeeksTomorrowAtMidnight.format(
            DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
    )
}

private fun setToMidnight(date: ZonedDateTime): ZonedDateTime {
    return date.withHour(0).withMinute(0).withSecond(0)
}
