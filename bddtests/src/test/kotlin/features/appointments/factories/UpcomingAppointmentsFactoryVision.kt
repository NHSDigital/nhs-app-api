package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class UpcomingAppointmentsFactoryVision : UpcomingAppointmentsFactory("VISION") {

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

    override fun getExpectedApiResponse(facade: ArrayList<AppointmentSessionFacade>): MyAppointmentsResponse {
        val filteredFacade = filterUpcomingAppointmentsWhenAppropriate(facade)
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
                                }
                        )
                    }
                }
        )
    }

    override fun getExpectedUiRepresentationOfSlots(facade: MyAppointmentsFacade): List<Slot> {
        return facade.slots?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        } ?: emptyList()
    }

    override fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val sessionDetails = "${session.sessionType} - ${slot.slotTypeName}"
        val location = session.location
        return Slot(
                date = date,
                time = time,
                session = sessionDetails,
                location = location!!,
                clinicians = setOf(session.staffDetails.first().staffName!!),
                id = slot.slotId
        )
    }

}
