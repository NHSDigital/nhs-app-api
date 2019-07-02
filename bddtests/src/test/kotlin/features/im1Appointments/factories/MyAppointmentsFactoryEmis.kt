package features.im1Appointments.factories

import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.historicalAppointmentsFromSessions
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.upcomingAppointmentsFromSessions
import models.Slot
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryEmis : MyAppointmentsFactory("EMIS") {

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        val pastAppointments = historicalAppointmentsFromSessions(sessions)
        val upcomingAppointments = upcomingAppointmentsFromSessions(sessions)
        return MyAppointmentsResponse(
                upcomingAppointments.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                                session.sessionType!!,
                                slot.startTime!!,
                                slot.endTime!!,
                                appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                                appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session)
                        )
                    }
                },
                pastAppointments.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                                session.sessionType!!,
                                slot.startTime!!,
                                slot.endTime!!,
                                appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                                appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session),
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

        return Slot(
                date = date,
                time = time,
                sessionName = session.sessionType!!,
                slotType = appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                location = appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                clinicians = appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session).toSet(),
                id = slot.slotId
        )
    }
}
