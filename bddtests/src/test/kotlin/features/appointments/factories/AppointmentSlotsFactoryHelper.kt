package features.appointments.factories

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.emis.models.SlotTypeStatus
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import net.serenitybdd.core.Serenity
import worker.models.appointments.SlotResponseObject
import java.time.ZonedDateTime
import java.util.*

private const val DEFAULT_NUMBER_OF_DAYS_IN_RANGE = 29L

class AppointmentSlotsFactoryHelper {

    private val supplierAdjustTime = TimeZone.getTimeZone("Europe/London").toZoneId()

    fun defaultStartDate(): ZonedDateTime {
        return ZonedDateTime.now()
                .withZoneSameInstant(supplierAdjustTime)
    }

    fun defaultEndDate(): ZonedDateTime {
        return ZonedDateTime.now()
                .toLocalDate()
                .atStartOfDay(supplierAdjustTime)
                .plusDays(DEFAULT_NUMBER_OF_DAYS_IN_RANGE)
    }

    fun storeUIDetailsOfSlotToSelect() {
        val expectedFilteredSlots = Serenity.sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        ).filteredSlots
        val dateToSelect = expectedFilteredSlots.keys.firstOrNull()
        val timeToSelect = expectedFilteredSlots[dateToSelect]?.firstOrNull()
        Serenity.setSessionVariable(AppointmentSessionVariableKeys.APPOINTMENT_TO_SELECT).to(timeToSelect)
    }

    fun getExpectedApiResponseSlotsWithSessionNames(
            facade: AppointmentSlotsResponseFacade,
            withSessionNames: Boolean
    ): List<SlotResponseObject> {
        return facade.sessions.flatMap { session ->
            session.slots.map { slot ->
                SlotResponseObject(
                        slot.slotId.toString(),
                        slot.slotTypeName!!,
                        if (withSessionNames) session.sessionType!! else "",
                        slot.startTime!!,
                        slot.endTime!!,
                        session.location!!,
                        session.staffDetails.map { staff -> staff.staffName }.toTypedArray(),
                        if (slot.channel == SlotTypeStatus.Telephone) 1 else 0
                )
            }
        }
    }

    fun getExpectedUiRepresentationOfFilteredSlotsWithSessionNames(
            facade: AppointmentFilterFacade,
            withSessionNames: Boolean
    ): AppointmentFilterFacade {
        return facade.copy(
                filteredSlots =
                facade.filteredSlots.mapValues { day ->
                    day.value.map { slot ->
                        if (!withSessionNames) slot.sessionName = null
                        slot
                    }.toSet()
                }
        )
    }
}