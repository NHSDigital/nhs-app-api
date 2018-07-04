package mocking.defaults

import mocking.MockingClient
import mocking.dataPopulation.journies.myRecord.MyRecordJournies
import mocking.dataPopulation.journies.prescriptions.PrescriptionsJournies
import mocking.defaults.dataPopulation.journies.appointmentSlots.AppointmentSlotsJournies
import mocking.defaults.dataPopulation.journies.courses.CoursesJournies
import mocking.defaults.dataPopulation.journies.im1Connection.Im1ConnectionJournies
import mocking.defaults.dataPopulation.journies.linkage.LinkageJournies
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionJournies
import models.Patient
import worker.models.session.UserSessionRequest

const val CONNECTION_TOKEN_SUFFIX_LENGTH = 12

open class MockDataPopulate(private val mockingClient: MockingClient) {

    companion object {
        @JvmStatic fun main(arguments: Array<String>) {
            val client = MockingClient.instance
            if (arguments.isNotEmpty()) {
                val type = arguments[0]
                when(type.toLowerCase()) {
                    "nft" -> {
                        MockDataPopulate(client).populateNftStubs(numOfPatients = arguments[1].toInt())
                    }
                    else -> { throw IllegalArgumentException("Type $type not recognised as a mock data set.")}
                }
            } else {
                MockDataPopulate(client).populate()
            }
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

    private fun populateNftStubs(numOfPatients: Int) {

        mockingClient.clearWiremock()
        mockingClient.favicon()

        for (i in 1..numOfPatients) {

            var index: String = i.toString()
            val pad=index.padStart(CONNECTION_TOKEN_SUFFIX_LENGTH,'0')
            var patient = Patient.montelFrye.copy(
                    firstName = "NFT.Patient",

                    surname = "Test$pad",
                    cidUserSession = UserSessionRequest(
                            authCode = "authCode$pad",
                            codeVerifier = "codeVerifier$pad"
                    ),
                    accessToken = "accessToken$pad",
                    endUserSessionId = "endUserSessionId$pad",
                    connectionToken = "00000000-0000-0000-0000-$pad"
            )
            CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
            EmisSessionCreateJourneyFactory(mockingClient).createFor(patient)
        }
    }
}