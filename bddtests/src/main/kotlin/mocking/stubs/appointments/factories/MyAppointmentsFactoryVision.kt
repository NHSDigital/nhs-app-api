package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryVision : MyAppointmentsFactory(Supplier.VISION) {

    private val defaultReasons = arrayListOf(
            AppointmentCancellationReason("1", "Reason 1"),
            AppointmentCancellationReason("2", "Reason 2")
    )

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = defaultReasons
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        return MyAppointmentsResponse(
                sessions.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                                session.sessionType!!,
                                slot.startTime!!,
                                slot.endTime!!,
                                appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                                appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session),
                                (slot.slotInThePast ?: false).toString()
                        )
                    }
                }
        )
    }

    override fun getExpectedUiRepresentationOfUpcomingSlots(facade: MyAppointmentsFacade): List<Slot> {
        return facade.myAppointments?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        } ?: emptyList()
    }

    override fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot> {
        return emptyList()  // Not yet implemented for Vision
    }
}
