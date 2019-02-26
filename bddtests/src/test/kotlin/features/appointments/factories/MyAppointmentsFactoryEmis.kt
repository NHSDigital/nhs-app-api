package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.historicalAppointmentsFromSessions
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.upcomingAppointmentsFromSessions
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryEmis : MyAppointmentsFactory("EMIS") {

    private val emisCancellationReason1 =
            AppointmentCancellationReason("R1_NoLongerRequired", "No longer required")

    private val emisCancellationReason2 =
            AppointmentCancellationReason("R2_UnableToAttend", "Unable to attend")

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = arrayListOf(emisCancellationReason1, emisCancellationReason2)
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        val pastAppointments = historicalAppointmentsFromSessions(sessions)
        val upcomingAppointments = upcomingAppointmentsFromSessions(sessions)
        return MyAppointmentsResponse(
                upcomingAppointments.flatMap { session ->
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
                },
                pastAppointments.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                "${session.sessionType} - ${slot.slotTypeName}",
                                slot.startTime!!,
                                slot.endTime!!,
                                session.location!!,
                                session.staffDetails.map { staff ->
                                    staff.staffName!!
                                },
                                null
                        )
                    }
                }

        )
    }

    override fun getExpectedUiRepresentationOfUpcomingSlots(facade: MyAppointmentsFacade): List<Slot> {
        val upcomingAppointments = upcomingAppointmentsFromSessions(facade.myAppointments!!.sessions)
        return upcomingAppointments.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        }
    }

    override fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot> {
        val historicalAppointments = historicalAppointmentsFromSessions(facade.myAppointments!!.sessions)
        return historicalAppointments.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        }
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
                clinicians = HashSet(cliniciansNames),
                id = slot.slotId
        )
    }
}
