package mocking.microtest.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import org.apache.http.HttpStatus.SC_OK
import utils.TimeConverter
import java.time.Duration
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

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
                GetAppointmentSlotsResponseModel(
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
                                        TimeConverter.setDuration(startTime, endTime),
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

    private fun convertStringToMicrotestTimeString(time: String): String {
        return convertDateToMicrotestTimeString(ZonedDateTime.parse(time))
    }

    private fun convertDateToMicrotestTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(time)
    }

    private fun convertDateToMicrotestDateString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.dateWithoutTimeFormat)
        return queryDateFormat.format(time)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondWithForbiddenError()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithBadGateway()
    }

    private fun respondWithBody(body: Any, statusCode: Int = SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asIs)
                    .andDelay(delayMillisecs)
        }
    }
}
