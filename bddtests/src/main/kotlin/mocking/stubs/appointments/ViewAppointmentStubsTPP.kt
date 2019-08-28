package mocking.stubs.appointments

import mocking.data.appointments.AppointmentsSlotsExample
import mocking.stubs.appointments.factories.MyAppointmentsFactory
import utils.set

class ViewAppointmentStubsTPP {

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    fun createHistoricalAndUpcomingAppointmentsTPP(gpSupplier: String){
        val historicalAppointments = appointmentSlotsExample.getHistoricalAppointmentSession()
        val upcomingAppointments = appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime()
        val allAppointments = arrayListOf(historicalAppointments, upcomingAppointments)
        SerenitySessionSlotId.APPOINTMENTONE.set(upcomingAppointments.slots.first().slotId)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpSupplier)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(allAppointments)
        )}
}