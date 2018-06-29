package mocking.defaults

import mocking.MockingClient
import mocking.dataPopulation.journies.myRecord.MyRecordJournies
import mocking.dataPopulation.journies.prescriptions.PrescriptionsJournies
import mocking.defaults.dataPopulation.journies.appointmentSlots.AppointmentSlotsJournies
import mocking.defaults.dataPopulation.journies.courses.CoursesJournies
import mocking.defaults.dataPopulation.journies.im1Connection.Im1ConnectionJournies
import mocking.defaults.dataPopulation.journies.linkage.LinkageJournies
import mocking.defaults.dataPopulation.journies.session.SessionJournies


open class MockDataPopulate(private val mockingClient: MockingClient) {

    companion object {
        @JvmStatic fun main(arguments: Array<String>) {
            val client = MockingClient.instance
            MockDataPopulate(client).populate()
        }
    }

    fun populate() {
        mockingClient.clearWiremock()
        mockingClient.favicon()

        Im1ConnectionJournies(mockingClient).create()
        SessionJournies(mockingClient).create()
        AppointmentSlotsJournies(mockingClient).create()
        PrescriptionsJournies(mockingClient).create()
        CoursesJournies(mockingClient).create()
        MyRecordJournies(mockingClient).create()
        LinkageJournies(mockingClient).create()
    }
}