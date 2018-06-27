package mocking.defaults.dataPopulation.journies.appointmentSlots

import mocking.MockingClient

class AppointmentSlotsJournies(private val client: MockingClient) {
    fun create() {
        AvailableSlotsJourney(client).create()
    }
}