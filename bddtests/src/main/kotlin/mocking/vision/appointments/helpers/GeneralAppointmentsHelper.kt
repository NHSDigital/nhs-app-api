package mocking.vision.appointments.helpers

import constants.DateTimeFormats
import mocking.vision.models.appointments.CancellationReasons
import mocking.vision.models.appointments.Location
import mocking.vision.models.appointments.Owner
import mocking.vision.models.appointments.Reason
import mocking.vision.models.appointments.ReasonDescription
import mocking.vision.models.appointments.References
import mocking.vision.models.appointments.Slot
import mocking.vision.models.appointments.SlotType
import mocking.vision.models.appointments.Slots
import mocking.vision.models.appointments.BookingReason
import mocking.emis.practices.NecessityOption
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
                                    session.staffDetails.first().toString(),
                                    session.locationId,
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
            return slotsResponseFacade.locations.map { location ->
                Location(location.locationId, location.locationName)
            }
        }

        private fun extractOwnersFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<Owner> {
            return slotsResponseFacade.staffDetails.map { staffDetails ->
                Owner(staffDetails.staffDetailsid, staffDetails.staffName)
            }
        }

        private fun extractSessionsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade):
                List<mocking.vision.models.appointments.Session> {
            return slotsResponseFacade.sessions.map { session ->
                mocking.vision.models.appointments.Session(session.sessionId, session.sessionType!!)
            }
        }

        private fun extractSlotTypesFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade): List<SlotType> {
            return slotsResponseFacade.slotTypes.map { slotType ->
                SlotType(slotType.slotTypeId, slotType.slotTypeName)
            }
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

        fun extractVPBookingReasonsFromFacade(slotsResponseFacade: AppointmentSlotsResponseFacade)
                : BookingReason {
            val addReason = slotsResponseFacade.bookingReasonNecessityOption != NecessityOption.NOT_ALLOWED

            return BookingReason(
                    add = addReason
            )
        }

        private fun convertDateToVisionTime(time: String): String {
            val currentDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
            val dateToPass = ZonedDateTime.of(LocalDateTime.parse(time, currentDateFormat), ZoneId.of
            ("Europe/London"))
            val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithoutTimezone)
            return queryDateFormat.format(dateToPass)
        }

        private fun calculateDuration(startTime: String, endTime: String?): Int? {
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

        // Data related to this is used for passing into the Vision Configuration
        enum class VisionMetadata {
            LOCATIONS,
            OWNERS
        }
    }
}
