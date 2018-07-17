package mocking.defaults.dataPopulation.journies.appointmentSlots

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory

class AppointmentSlotsJournies(private val client: MockingClient) {
    fun create() {
        EmisSessionCreateJourneyFactory(client)
    }
}