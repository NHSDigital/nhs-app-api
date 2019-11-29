package mocking.defaults.dataPopulation.journies.im1Connection

import constants.Supplier
import mocking.MockingClient

class Im1ConnectionJournies(private val client: MockingClient) {
    fun create() {
        AlreadyRegisteredJourney(client).create()
        NoOnlineUserFoundJourney(client).create()
        SuccessfulRegistrationJourney(client).create(gpSystem = Supplier.EMIS)
    }
}