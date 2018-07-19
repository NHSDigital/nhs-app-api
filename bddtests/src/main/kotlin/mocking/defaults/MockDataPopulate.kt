package mocking.defaults

import kotlin.text.Charsets
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
import mocking.emis.appointments.CancelAppointmentRequest
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import worker.models.appointments.BookAppointmentSlotRequest
import worker.models.session.UserSessionRequest
import java.io.File

const val BASE_NFT_DATA_DIR = "src/main/kotlin/mocking/defaults/dataPopulation/nft"
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

            val index: String = i.toString()
            val pad=index.padStart(CONNECTION_TOKEN_SUFFIX_LENGTH,'0')
            val patient = Patient.montelFrye.copy(
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

            // Authentication

            CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
            EmisSessionCreateJourneyFactory(mockingClient).createFor(patient)


            // Appointment Mocks

            // GET /emis/appointments
            //
            val appointmentsBody = getFileContents("appointments/GetEmisAppointments.json")
            
            mockingClient.forEmis {
                appointmentGetRequest(patient = patient)
                        .respondWithSuccess(appointmentsBody)
            }

            // GET /emis/appointmentslots/meta
            //
            val getAppointmentSlotsMetaBody =
                    getFileContents("appointments/GetEmisAppointmentSlotsMeta.json")
                            

            mockingClient.forEmis {
                appointmentSlotsMetaRequest(patient = patient)
                        .respondWithSuccess(getAppointmentSlotsMetaBody)
                
            }
                    

            // GET /emis/appointmentslots
            //
            val getAppointmentSlotsBody =
                    getFileContents("appointments/GetEmisAppointmentSlots.json")
                            

            mockingClient.forEmis {
                appointmentSlotsRequest(patient = patient)
                        .respondWithSuccess(getAppointmentSlotsBody)
            }

            // POST /emis/appointments
            //
            val postAppointmentRequestBody =
                    getFileContents("appointments/PostEmisAppointment.json")
                            

            mockingClient.forEmis {
                bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken, 1, "NFT Test Book Slot"))
                        .respondWithSuccess(postAppointmentRequestBody)
            }

            // DELETE /emis/appointments
            //
            val deleteAppointmentRequestBody =
                    getFileContents("appointments/DeleteEmisAppointment.json")
                            

            mockingClient.forEmis {
                cancelAppointmentRequest(patient, CancelAppointmentRequest(patient.userPatientLinkToken, 1, "No longer required"))
                        .respondWithSuccess(deleteAppointmentRequestBody)
            }
        }
    }
    
    private fun getFileContents(relativePath: String): String {
        return File("$BASE_NFT_DATA_DIR/$relativePath")
                .bufferedReader(
                        charset = Charsets.UTF_8,
                        bufferSize = DEFAULT_BUFFER_SIZE
                )
                .readText()
                .replace("\t", "")
                .replace("\n","")
    }
}