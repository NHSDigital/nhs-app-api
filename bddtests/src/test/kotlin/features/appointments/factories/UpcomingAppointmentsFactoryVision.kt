package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity

class UpcomingAppointmentsFactoryVision : UpcomingAppointmentsFactory("VISION") {

    override fun setCancellationReasons() {
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(
                emptyList<AppointmentCancellationReason>()
        )
    }

    override fun getExpectedUiRepresentationOfSlots(facade: MyAppointmentsFacade): List<Slot> {
        return facade.slots?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        } ?: emptyList()
    }

    private fun getExpectedUiRepresentationOfSlot(slot: AppointmentSlotFacade, session: AppointmentSessionFacade): Slot {
        val startDate = dateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val sessionDetails = "${session.sessionDetails}"
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
