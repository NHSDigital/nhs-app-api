package mocking.stubs.appointments.factories

import constants.Supplier
import mocking.emis.models.AppointmentCancellationReason
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.MyAppointmentsFacade
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.historicalAppointmentsFromSessions
import mockingFacade.appointments.helpers.MyAppointmentFacadeHelper.Companion.upcomingAppointmentsFromSessions
import models.Slot
import net.serenitybdd.core.Serenity
import worker.models.appointments.AppointmentResponseObject
import worker.models.appointments.MyAppointmentsResponse

class MyAppointmentsFactoryTpp : MyAppointmentsFactory(Supplier.TPP) {


    override fun getDefaultCancellationReasons(): List<AppointmentCancellationReason> {
        val reasons = emptyList<AppointmentCancellationReason>()
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
                                slot.slotDetails,
                                "",
                                slot.startTime!!,
                                slot.endTime!!,
                                appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                                emptyList(),
                                "false",
                                slot.channel
                        )
                    }
                },
                pastAppointments.flatMap { session ->
                    session.slots.map { slot ->
                        AppointmentResponseObject(
                                slot.slotId.toString(),
                                slot.slotDetails,
                                "",
                                slot.startTime!!,
                                slot.endTime!!,
                                appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                                emptyList(),
                                null,
                                slot.channel
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
        val slotDetails = slot.slotDetails

        return Slot(
                date = date,
                time = time,
                slotType = slotDetails,
                location = appointmentSlotsFactoryHelper.getLocationNameFromId(session),
                id = slot.slotId
        )
    }

    override fun mockMyAppointments(
            appointmentType: IMyAppointmentsBuilder.AppointmentType,
            mapping:
            (IMyAppointmentsBuilder.() -> Mapping)
    ) {
        if (appointmentType == IMyAppointmentsBuilder.AppointmentType.BOTH ||
                appointmentType == IMyAppointmentsBuilder.AppointmentType.PAST_ONLY) {
            appointmentMapper.requestMapping {
                mapping(viewMyAppointmentsRequest(
                        patient,
                        appointmentType = IMyAppointmentsBuilder.AppointmentType.PAST_ONLY
                ))
            }
        }
        if (appointmentType == IMyAppointmentsBuilder.AppointmentType.BOTH ||
                appointmentType == IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY) {
            appointmentMapper.requestMapping {
                mapping(viewMyAppointmentsRequest(
                        patient,
                        appointmentType =
                        IMyAppointmentsBuilder.AppointmentType.UPCOMING_ONLY
                ))
            }
        }
    }
}
