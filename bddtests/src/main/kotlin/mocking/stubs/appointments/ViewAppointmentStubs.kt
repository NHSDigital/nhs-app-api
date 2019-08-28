package mocking.stubs.appointments

import mocking.data.appointments.AppointmentsSlotsExample
import mocking.stubs.appointments.factories.MyAppointmentsFactory

class ViewAppointmentStubs {

    private val appointmentSlotsExample = AppointmentsSlotsExample()

    fun createHistoricalUpcomingAppointments(gpService : String){
        val historicalAppointments = appointmentSlotsExample.getHistoricalAppointmentSession()
        val upcomingAppointments = appointmentSlotsExample.getExampleWithAppointmentWithinCutoffTime()
        val allAppointments = arrayListOf(historicalAppointments, upcomingAppointments)
        val viewAppointmentFactory = MyAppointmentsFactory.getForSupplier(gpService)
        viewAppointmentFactory.createSuccessfulMyAppointmentsResponse(
                appointmentSlotsExample.getGenericExample(allAppointments)
        )}
}