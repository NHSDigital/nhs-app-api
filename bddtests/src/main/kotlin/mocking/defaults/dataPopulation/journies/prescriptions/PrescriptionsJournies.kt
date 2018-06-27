package mocking.dataPopulation.journies.prescriptions

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsHistoryJourney
import mocking.defaults.dataPopulation.journies.prescriptions.RepeatPrescriptionsJourney

class PrescriptionsJournies(private val client: MockingClient) {
    fun create() {
        PrescriptionsHistoryJourney(client).createFor(SuccessfulRegistrationJourney.patient)
        RepeatPrescriptionsJourney(client).createFor(SuccessfulRegistrationJourney.patient)
    }
}