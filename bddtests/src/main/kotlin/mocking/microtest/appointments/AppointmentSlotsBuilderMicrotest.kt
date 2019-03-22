package mocking.microtest.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_OK
import java.time.Duration
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

private const val NUMBER_OF_SECONDS_IN_A_MINUTE = 60

class AppointmentSlotsBuilderMicrotest(fromDateTime: ZonedDateTime? = null,
                                       toDateTime: ZonedDateTime? = null
) : MicrotestMappingBuilder("GET", "/appointment-slots"), IAppointmentSlotsBuilder {

    init {
        if (fromDateTime != null) {
            val fromDateParts = convertDateToMicrotestDateString(fromDateTime)
            requestBuilder.andQueryParameter(
                    "fromDate",
                    fromDateParts,
                    "contains")
        }

        if (toDateTime != null) {
            requestBuilder.andQueryParameter(
                    "toDate",
                    convertDateToMicrotestDateString(toDateTime))
        }
    }

    override fun withDelay(delayMilliseconds: Duration): AppointmentSlotsBuilderMicrotest {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(facade: AppointmentSlotsResponseFacade): Mapping {
        return respondWithBody(
                GetAppointmentsResponseModel(
                        facade.sessions.flatMap { session ->
                            session.slots.map { slot ->
                                val startTime = convertStringToMicrotestTimeString(slot.startTime!!)
                                val endTime = convertStringToMicrotestTimeString(slot.endTime!!)
                                AppointmentSlot(
                                        slot.slotId!!.toString(),
                                        facade.slotTypes.find {
                                            slotType -> slot.slotTypeId == slotType.slotTypeId
                                        }!!.slotTypeName,
                                        startTime,
                                        setDuration(startTime, endTime),
                                        endTime,
                                        facade.locations.find { location -> session.locationId == location.locationId
                                        }!!.locationName,
                                        session.staffDetails.map { clinician ->
                                            facade.staffDetails.find { staff -> clinician == staff
                                                    .staffDetailsid
                                            }!!.staffName
                                        },
                                        slot.channel.toString()
                                )
                            }
                        }
                )
        )
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("appointment slots")
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }

    private fun convertStringToMicrotestTimeString(time: String): String {
        return convertDateToMicrotestTimeString(ZonedDateTime.parse(time))
    }

    private fun convertDateToMicrotestTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        return queryDateFormat.format(time)
    }

    private fun convertDateToMicrotestDateString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.dateWithoutTimeFormat)
        return queryDateFormat.format(time)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        TODO("not implemented")
    }

    override fun respondWithUnknownException(): Mapping {
        TODO("not implemented")
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asIs)
                    .andDelay(delayMillisecs)
        }
    }

    private fun setDuration(startTime: String, endTime: String?): String {
        val format = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        val startTimeAsLocalDateTime = ZonedDateTime.of(LocalDateTime.parse(startTime, format), ZoneId.of
        ("Europe/London"))
        val endTimeAsLocalDateTime = ZonedDateTime.of(LocalDateTime.parse(endTime, format), ZoneId.of
        ("Europe/London"))
        return (
                (endTimeAsLocalDateTime.toEpochSecond() - startTimeAsLocalDateTime.toEpochSecond())
                        / NUMBER_OF_SECONDS_IN_A_MINUTE
                ).toString() + " Minutes"
    }
}