package mocking.microtest.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import utils.TimeConverter
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class GetAppointmentBuilderMicrotest(val patient: Patient)
    : MicrotestMappingBuilder(method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {
    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        return respondWithBody(
            GetAppointmentsResponseModel(
                facade.myAppointments!!.sessions.flatMap { session ->
                    session.slots.map { slot ->
                        val startTime = convertStringToMicrotestTimeString(slot.startTime!!)
                        val endTime = convertStringToMicrotestTimeString(slot.endTime!!)
                        Appointment(
                            slot.slotId!!.toString(),
                            facade.myAppointments.slotTypes.find {
                                    slotType -> slot.slotTypeId == slotType.slotTypeId
                            }!!.slotTypeName,
                            startTime,
                            TimeConverter.setDuration(startTime, endTime),
                            endTime,
                            facade.myAppointments.locations.find { location -> session.locationId == location.locationId
                            }!!.locationName,
                            session.staffDetails.map { clinician ->
                                facade.myAppointments.staffDetails.find { staff -> clinician == staff
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

    override fun respondWithSuccess(body: String): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("<<randomtag/>>")
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithUnknownExceptionError()
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondWith(HttpStatus.SC_FORBIDDEN) {  andJsonBody("") }
    }

    private fun convertStringToMicrotestTimeString(time: String): String {
        return convertDateToMicrotestTimeString(ZonedDateTime.parse(time))
    }

    private fun convertDateToMicrotestTimeString(time: ZonedDateTime?): String {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
        return queryDateFormat.format(time)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asIs)
                .andDelay(delayMillisecs)
        }
    }
}
