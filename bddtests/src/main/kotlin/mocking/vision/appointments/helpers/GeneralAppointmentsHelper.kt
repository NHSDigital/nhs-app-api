package mocking.vision.appointments.helpers

import constants.DateTimeFormats
import mocking.vision.models.appointments.SlotType
import mocking.vision.models.appointments.CancellationReasons
import mocking.vision.models.appointments.Reason
import mocking.vision.models.appointments.ReasonDescription
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.References
import mocking.vision.models.appointments.Slot
import mocking.vision.models.appointments.Slots
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import java.time.LocalDateTime
import java.time.ZoneId
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

class GeneralAppointmentsHelper {

    companion object {

        private const val NUMBER_OF_SECONDS_IN_A_MINUTE = 60

        fun extractSlotsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): Slots {
            return Slots(
                    slotsResponseFacade.sessions.flatMap { session ->
                        session.slots.map { slot ->
                            Slot(
                                    slot.slotId!!.toString(),
                                    convertDateToVisionTime(slot.startTime!!),
                                    calculateDuration(
                                            convertDateToVisionTime(slot.startTime!!),
                                            convertDateToVisionTime(slot.endTime!!)
                                    ),
                                    session.staffDetails.first().staffDetailsid.toString(),
                                    session.locationid,
                                    slot.slotTypeId,
                                    session.sessionId
                            )
                        }
                    }
            )
        }

        fun extractReferencesFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): References {
            return References(
                    extractLocationsFromFacade(slotsResponseFacade),
                    extractOwnersFromFacade(slotsResponseFacade),
                    extractSessionsFromFacade(slotsResponseFacade),
                    extractSlotTypesFromFacade(slotsResponseFacade)
            )
        }

        private fun extractLocationsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                List<mocking.vision.models.appointments.Location> {
            return slotsResponseFacade.sessions.map { session ->
                mocking.vision.models.appointments.Location(session.locationid, session.location!!)
            }.distinct()
        }

        private fun extractOwnersFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<Owner> {
            return slotsResponseFacade.sessions.flatMap { session ->
                session.staffDetails.map { clinician ->
                    Owner(clinician.staffDetailsid!!, clinician.staffName!!)
                }
            }.distinct()
        }

        private fun extractSessionsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                List<mocking.vision.models.appointments.Session> {
            return slotsResponseFacade.sessions.map { session ->
                mocking.vision.models.appointments.Session(session.sessionId, session.sessionType!!)
            }
        }

        private fun extractSlotTypesFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<SlotType> {
            return slotsResponseFacade.sessions.flatMap { session ->
                session.slots.map { slot ->
                    SlotType(slot.slotTypeId!!, slot.slotTypeName!!)
                }
            }.distinct()
        }

        fun extractVPCancelReasonsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade)
                : CancellationReasons {
            val facadeReasons = slotsResponseFacade.cancellationReasons
            val visionReasons = mutableListOf<Reason>()
            facadeReasons?.forEach { reason ->
                val visionReason = Reason(reason.id, listOf(ReasonDescription(reason.displayName)))
                visionReasons.add(visionReason)
            }
            return CancellationReasons(visionReasons)
        }

        fun convertDateToVisionTime(time: String): String {
            val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
            val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
            ("Europe/London"))
            val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            return queryDateFormat.format(dateToPass)
        }

        fun calculateDuration(startTime: String, endTime: String?): Int? {
            val format = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            val startTimeAsLocalDateTime = ZonedDateTime.of(LocalDateTime.parse(startTime, format), ZoneId.of
            ("Europe/London"))
            val endTimeAsLocalDateTime = ZonedDateTime.of(LocalDateTime.parse(endTime, format), ZoneId.of
            ("Europe/London"))
            return (
                    (endTimeAsLocalDateTime.toEpochSecond() - startTimeAsLocalDateTime.toEpochSecond())
                            / NUMBER_OF_SECONDS_IN_A_MINUTE
                    )
                    .toInt()
        }
    }
}
