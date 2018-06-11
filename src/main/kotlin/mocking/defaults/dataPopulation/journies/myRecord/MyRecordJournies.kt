package mocking.dataPopulation.journies.myRecord

import mocking.MockingClient

class MyRecordJournies(private val client: MockingClient) {
    fun create() {
        MyRecordJourney(client).create()
    }
}
