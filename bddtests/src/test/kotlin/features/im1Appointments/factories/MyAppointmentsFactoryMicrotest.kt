package features.im1Appointments.factories

import features.im1Appointments.factories.AppointmentsBookingFactory.Companion.telephoneNumberToEnter
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper
import models.Slot
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryMicrotest : MyAppointmentsFactory("MICROTEST") {

    override fun getExpectedApiResponse(facade: MyAppointmentsFacade): MyAppointmentsResponse {
        val sessions = facade.myAppointments!!.sessions
        val pastAppointments = MyAppointmentFacadeHelper.historicalAppointmentsFromSessions(sessions)
        val upcomingAppointments = MyAppointmentFacadeHelper.upcomingAppointmentsFromSessions(sessions)
        return MyAppointmentsResponse(
                flatMapAppointments(upcomingAppointments, facade, "false"),
                flatMapAppointments(pastAppointments, facade, null)
        )
    }

    private fun flatMapAppointments(appointments: List<AppointmentSessionFacade>,
                                    facade: MyAppointmentsFacade,
                                    disableCancellation: String?): List<AppointmentResponseObject> {
        return appointments.flatMap { session ->
            session.slots.map { slot ->
                AppointmentResponseObject(
                        slot.slotId.toString(),
                        facade.myAppointments!!.slotTypes.find { slotType ->
                            slot.slotTypeId == slotType.slotTypeId
                        }!!.slotTypeName,
                        "",
                        slot.startTime!!,
                        slot.endTime!!,
                        facade.myAppointments!!.locations.find { location ->
                            session.locationId == location.locationId
                        }!!.locationName,
                        session.staffDetails.map { clinician ->
                            facade.myAppointments!!.staffDetails.find { staff ->
                                clinician == staff
                                        .staffDetailsid
                            }!!.staffName
                        },
                        disableCancellation,
                        slot.channel,
                        slot.telephoneNumber
                )
            }
        }
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
        val historicalAppointments =
                MyAppointmentFacadeHelper.historicalAppointmentsFromSessions(facade.myAppointments!!.sessions)
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
        val location = appointmentSlotsFactoryHelper.getLocationNameFromId(session)
        val cliniciansNames = appointmentSlotsFactoryHelper.getClinicianNamesFromIds(session)
        return Slot(
                date = date,
                time = time,
                slotType = appointmentSlotsFactoryHelper.getSlotTypeNameFromId(slot),
                location = location,
                clinicians = HashSet(cliniciansNames),
                id = slot.slotId,
                //this here
                channel = slot.channel.toString(),
                telephoneNumber = when (slot.telephoneNumber) {
                    telephoneNumberToEnter -> ""
                    else -> slot.telephoneNumber
                }
        )
    }
}
