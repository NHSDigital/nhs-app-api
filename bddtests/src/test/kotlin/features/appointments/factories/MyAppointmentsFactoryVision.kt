package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
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

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        return MyAppointmentsResponse(
                sessions.flatMap { session ->
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

    override fun getExpectedUiRepresentationOfSlot(
            slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val slotDetails = "${session.sessionType} - ${slot.slotTypeName}"
        val location = session.location
        val cliniciansNames: ArrayList<String> = ArrayList()
        session.staffDetails.forEach { staff ->
            cliniciansNames.add(staff.staffName!!)
        }
        return Slot(
                date = date,
                time = time,
                slotType = slotDetails,
                location = location!!,
                clinicians = setOf(session.staffDetails.first().staffName!!),
                id = slot.slotId
        )
    }

    override fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot> {
        return emptyList()  // Not yet implemented for Vision
    }
}
