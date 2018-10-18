package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class UpcomingAppointmentsFactoryEmis : UpcomingAppointmentsFactory("EMIS") {

    private val emisCancellationReason1 =
            AppointmentCancellationReason("R1_NoLongerRequired", "No longer required")

    private val emisCancellationReason2 =
            AppointmentCancellationReason("R2_UnableToAttend", "Unable to attend")

    override fun setCancellationReasons() {
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(
                arrayListOf(emisCancellationReason1, emisCancellationReason2)
        )
    }

    override fun filterUpcomingAppointmentsWhenAppropriate(
            facade: ArrayList<AppointmentSessionFacade>
    ): ArrayList<AppointmentSessionFacade> {
        // Don't need to filter for Emis
        return facade
    }

    override fun getExpectedApiResponse(facade: ArrayList<AppointmentSessionFacade>): MyAppointmentsResponse {
        val filteredFacade = filterUpcomingAppointmentsWhenAppropriate(facade)
        return MyAppointmentsResponse(
                filteredFacade.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                "${session.sessionType} - ${slot.slotTypeName}",
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

    private fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val sessionDetails = "${session.sessionType} - ${slot.slotTypeName}"
        val location = session.location
        val cliniciansNames: ArrayList<String> = ArrayList()
        session.staffDetails.forEach { staff ->
            cliniciansNames.add(staff.staffName!!)
        }
        return Slot(
                date = date,
                time = time,
                session = sessionDetails,
                location = location!!,
                clinicians = HashSet(cliniciansNames),
                id = slot.slotId
        )
    }
}
