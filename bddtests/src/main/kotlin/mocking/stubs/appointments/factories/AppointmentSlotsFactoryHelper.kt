package mocking.stubs.appointments.factories

import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mocking.emis.models.SlotTypeStatus
import mockingFacade.appointments.AppointmentFilterFacade
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import mockingFacade.appointments.metadata.LocationFacade
import mockingFacade.appointments.metadata.SlotTypeFacade
import mockingFacade.appointments.metadata.StaffDetailsFacade
import net.serenitybdd.core.Serenity
import worker.models.appointments.SlotResponseObject
import java.time.ZonedDateTime
import java.util.*

private const val DEFAULT_NUMBER_OF_DAYS_IN_RANGE = 113L

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
                        facade.slotTypes.find { slotType -> slot.slotTypeId == slotType.slotTypeId }!!.slotTypeName,
                        if (withSessionNames) session.sessionType!! else "",
                        slot.startTime!!,
                        slot.endTime!!,
                        facade.locations.find { location -> session.locationId == location.locationId }!!.locationName,
                        session.staffDetails.map { clinician ->
                            facade.staffDetails.find { staff ->
                                clinician == staff.staffDetailsid
                            }!!.staffName
                        }.toTypedArray(),
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

    fun getLocationNameFromId(session: AppointmentSessionFacade): String {
        val possibleLocations = Serenity.sessionVariableCalled<List<LocationFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_LOCATIONS_KEY
        )
        return possibleLocations.find { location -> session.locationId == location.locationId }!!.locationName
    }

    fun getSlotTypeNameFromId(slot: AppointmentSlotFacade): String {
        val possibleSlotTypes = Serenity.sessionVariableCalled<List<SlotTypeFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_TYPE_KEY
        )
        return possibleSlotTypes.find { slotType -> slot.slotTypeId == slotType.slotTypeId }!!.slotTypeName
    }

    fun getClinicianNamesFromIds(session: AppointmentSessionFacade): List<String> {
        val possibleClinicians = Serenity.sessionVariableCalled<List<StaffDetailsFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_CLINICIANS_KEY
        )
        return session.staffDetails.map { clinician ->
            possibleClinicians.find { staff ->
                clinician == staff.staffDetailsid
            }!!.staffName
        }
    }

    fun extractStaffDetailsFacadesByIds(ids: List<Int>): List<StaffDetailsFacade> {
        val possibleClinicians = Serenity.sessionVariableCalled<List<StaffDetailsFacade>>(
                AppointmentsSlotsExampleBuilderWithExpectations.AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_CLINICIANS_KEY
        )
        return possibleClinicians.filter {staff ->
            ids.contains(staff.staffDetailsid)
        }
    }
}
