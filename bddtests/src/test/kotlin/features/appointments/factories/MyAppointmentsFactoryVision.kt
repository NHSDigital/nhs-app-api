package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryVision : MyAppointmentsFactory("VISION") {

    private val defaultReasons = arrayListOf(
            AppointmentCancellationReason("1", "Reason 1"),
            AppointmentCancellationReason("2", "Reason 2")
    )

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = defaultReasons
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    override fun filterUpcomingAppointmentsWhenAppropriate(
            facade: ArrayList<AppointmentSessionFacade>
    ): ArrayList<AppointmentSessionFacade> {
        // Don't need to filter for Vision
        return facade
    }

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val filteredFacade = filterUpcomingAppointmentsWhenAppropriate(facade.myAppointments!!.sessions)
        return MyAppointmentsResponse(
                filteredFacade.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                "${session.sessionType!!} - ${slot.slotTypeName}",
                                slot.startTime!!,
                                slot.endTime!!,
                                session.location!!,
                                session.staffDetails.map { staff ->
                                    staff.staffName!!
                                },
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
