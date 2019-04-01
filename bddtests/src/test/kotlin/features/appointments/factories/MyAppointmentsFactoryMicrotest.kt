package features.appointments.factories

import mocking.emis.models.AppointmentCancellationReason
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryMicrotest : MyAppointmentsFactory("MICROTEST") {
    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        val upcomingAppointments = MyAppointmentFacadeHelper.upcomingAppointmentsFromSessions(sessions)
        return MyAppointmentsResponse(
            upcomingAppointments.flatMap { session ->
                session.slots.map { slot ->
                    AppointmentResponseObject(
                        slot.slotId.toString(),
                        facade.myAppointments!!.slotTypes.find {
                                slotType -> slot.slotTypeId == slotType.slotTypeId
                        }!!.slotTypeName,
                        "",
                        slot.startTime!!,
                        slot.endTime!!,
                        facade.myAppointments!!.locations.find { location -> session.locationId == location.locationId
                        }!!.locationName,
                        session.staffDetails.map { clinician ->
                            facade.myAppointments!!.staffDetails.find { staff -> clinician == staff
                                .staffDetailsid
                            }!!.staffName
                        }
                    )
                }
            }
        )
    }

    override fun getExpectedUiRepresentationOfUpcomingSlots(facade: MyAppointmentsFacade): List<Slot> {
        val sessions = facade.myAppointments!!.sessions
        val upcomingAppointments = MyAppointmentFacadeHelper.upcomingAppointmentsFromSessions(sessions)
        return upcomingAppointments.flatMap { session ->
            session.slots.map { slot ->
                getExpectedUiRepresentationOfSlot(slot, session)
            }
        }
    }

    override fun getExpectedUiRepresentationOfHistoricalSlots(facade: MyAppointmentsFacade): List<Slot> {
        // Not yet implemented for Microtest
        return emptyList()
    }

    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = emptyList<AppointmentCancellationReason>()
        Serenity.setSessionVariable(AppointmentCancellationReason::class).to(reasons)
        return reasons
    }

    override fun getExpectedUiRepresentationOfSlot(
        slot: AppointmentSlotFacade, session: AppointmentSessionFacade
    ): Slot {
        val startDate = gpDateTimeFormat.parse(slot.startTime)
        val date = slotDateFormat(startDate)
        val time = slotTimeFormat(startDate)
        val location = appointmentSlotsFactoryHelper.getLocationNameFromId(session)
        val cliniciansNames = appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session)
        return Slot(
            date = date,
            time = time,
            slotType = appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
            location = location,
            clinicians = HashSet(cliniciansNames),
            id = slot.slotId
        )
    }
}
