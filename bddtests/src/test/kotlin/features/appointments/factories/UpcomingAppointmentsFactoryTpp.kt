package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class UpcomingAppointmentsFactoryTpp : UpcomingAppointmentsFactory("TPP") {

    override val cancellationReasonRequired: Boolean = false

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = emptyList<AppointmentCancellationReason>()
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    override fun filterUpcomingAppointmentsWhenAppropriate(
            facade: ArrayList<AppointmentSessionFacade>
    ): List<AppointmentSessionFacade> {
        // Need to filter out any appointments that are no longer "upcoming"
        return facade.filter { session ->
            session.slots.any { slot -> !slot.slotInThePast!! }
        }
    }

    override fun getExpectedApiResponse(facade: ArrayList<AppointmentSessionFacade>): MyAppointmentsResponse {
        val filteredFacade = filterUpcomingAppointmentsWhenAppropriate(facade)
        return MyAppointmentsResponse(
                filteredFacade.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                session.sessionDetails!!,
                                slot.startTime!!,
                                slot.endTime!!,
                                session.location!!
                        )
                    }
                }
        )
    }

    override fun getExpectedUiRepresentationOfSlots(facade: MyAppointmentsFacade): List<Slot> {
        return facade.slots?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                val startDate = gpDateTimeFormat.parse(slot.startTime)
                val date = slotDateFormat(startDate)
                val time = slotTimeFormat(startDate)
                val sessionDetails = session.sessionDetails
                val location = session.location
                Slot(
                        date = date,
                        time = time,
                        session = sessionDetails!!,
                        location = location!!,
                        id = slot.slotId
                )
            }
        } ?: emptyList()
    }
}
