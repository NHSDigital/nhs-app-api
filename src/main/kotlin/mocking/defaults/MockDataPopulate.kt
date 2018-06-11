package mocking.defaults

import config.Config
import mocking.MockingClient
import mocking.dataPopulation.journies.myRecord.MyRecordJournies
import mocking.dataPopulation.journies.prescriptions.PrescriptionsJournies
import mocking.defaults.dataPopulation.journies.appointmentSlots.AppointmentSlotsJournies
import mocking.defaults.dataPopulation.journies.courses.CoursesJournies
import mocking.defaults.dataPopulation.journies.im1Connection.Im1ConnectionJournies
import mocking.defaults.dataPopulation.journies.session.SessionJournies


open class MockDataPopulate(private val mockingClient: MockingClient) {

    private val defaults: MockDefaults = MockDefaults(Config.instance)

    companion object {
        @JvmStatic fun main(arguments: Array<String>) {
            val client = MockingClient.instance
            MockDataPopulate(client).populate()
        }
    }

    fun populate() {
        this.defaults.mock()

        Im1ConnectionJournies(mockingClient).create()
        SessionJournies(mockingClient).create()
        AppointmentSlotsJournies(mockingClient).create()
        PrescriptionsJournies(mockingClient).create()
        CoursesJournies(mockingClient).create()
        MyRecordJournies(mockingClient).create()
    }
}