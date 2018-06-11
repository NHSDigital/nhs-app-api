package mocking.defaults.dataPopulation.journies.courses

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney

class CoursesJournies(private val client: MockingClient) {
    fun create() {
        OrderableCourses(client).createFor(SuccessfulRegistrationJourney.patient)
    }
}