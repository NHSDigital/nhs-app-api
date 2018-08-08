package mocking.defaults

import config.Config
import kotlin.text.Charsets
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.dataPopulation.journies.myRecord.MyRecordJournies
import mocking.dataPopulation.journies.prescriptions.PrescriptionsJournies
import mocking.defaults.dataPopulation.journies.appointmentSlots.AppointmentSlotsJournies
import mocking.defaults.dataPopulation.journies.courses.CoursesJournies
import mocking.defaults.dataPopulation.journies.im1Connection.Im1ConnectionJournies
import mocking.defaults.dataPopulation.journies.linkage.LinkageJournies
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionJournies
import mocking.emis.models.CourseRequestsGetResponse
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import models.prescriptions.MedicationCourse
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

    fun populateForJustLoggedIn(){
        mockingClient.clearWiremock()
        mockingClient.favicon()

        Im1ConnectionJournies(mockingClient).create()
        SessionJournies(mockingClient).create()
        LinkageJournies(mockingClient).create()
    }

    fun populate() {
        populateForJustLoggedIn()

        AppointmentSlotsJournies(mockingClient).create()
        PrescriptionsJournies(mockingClient).create()
        CoursesJournies(mockingClient).create()
        MyRecordJournies(mockingClient).create()
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
                            codeVerifier = "codeVerifier$pad",
                            redirectUrl = Config.instance.cidRedirectUri
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
            val appointmentsBody = getFileContents("appointments/GetEmisAppointments.json")
            
            mockingClient.forEmis {
                viewMyAppointmentsRequest(patient = patient)
                        .respondWithSuccess(appointmentsBody)
            }

            // GET /emis/appointmentslots/meta
            val getAppointmentSlotsMetaBody =
                    getFileContents("appointments/GetEmisAppointmentSlotsMeta.json")

            mockingClient.forEmis {
                appointmentSlotsMetaRequest(patient = patient)
                        .respondWithSuccessJson(getAppointmentSlotsMetaBody)
            }

            // GET /emis/appointmentslots
            val getAppointmentSlotsBody =
                    getFileContents("appointments/GetEmisAppointmentSlots.json")

            mockingClient.forEmis {
                appointmentSlotsRequest(patient = patient)
                        .respondWithSuccessJson(getAppointmentSlotsBody)
            }

            // POST /emis/appointments
            val postAppointmentRequestBody =
                    getFileContents("appointments/PostEmisAppointment.json")

            mockingClient.forEmis {
                bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken, 1, "NFT Test Book Slot"))
                        .respondWithSuccessJson(postAppointmentRequestBody)
            }

            // DELETE /emis/appointments
            val deleteAppointmentRequestBody =
                    getFileContents("appointments/DeleteEmisAppointment.json")

            mockingClient.forEmis {
                cancelAppointmentRequest(patient, CancelAppointmentSlotFacade(patient.userPatientLinkToken, 1, "No longer required"))
                        .respondWithSuccessJson(deleteAppointmentRequestBody)
            }

            // Prescriptions
            // GET /emis/prescriptionrequests
            val prescriptionsDataLoader = EmisPrescriptionLoader
            prescriptionsDataLoader.loadData(
                    noPrescriptions = 5,
                    noCourses = 5,
                    noRepeats = 5,
                    showDosage = true,
                    showQuantity = true
            )

            mockingClient.forEmis {
                prescriptionsRequest(patient)
                        .respondWithSuccess(prescriptionsDataLoader.data)
                        .inScenario(pad)
                        .whenScenarioStateIs("Started")
            }

            // GET /emis/courses
            val coursesLoader = EmisCoursesLoader
            coursesLoader.loadData(
                    maxCourses = 5,
                    numOfRepeats = 5,
                    numCanBeRequested = 5,
                    includeDosage = true,
                    includeQuantity = true)

            mockingClient.forEmis {
                coursesRequest(patient)
                        .respondWithSuccess(CourseRequestsGetResponse(coursesLoader.data as List<MedicationCourse>))
            }

            mockingClient.forEmis {
                repeatPrescriptionSubmissionRequest(patient)
                        .respondWithCreated()
                        .inScenario(pad)
                        .whenScenarioStateIs("Started")
            }

            // My medical Record
            // GET /emis/record (testResultsRequest)
            mockingClient.forEmis {
                testResultsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/TestResults.json"))
            }

            // GET /emis/record (immunisationsRequest)
            mockingClient.forEmis {
                immunisationsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/Immunisations.json"))
            }

            // GET /emis/record (allergiesRequest)
            mockingClient.forEmis {
                allergiesRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/Allergies.json"))
            }

            // GET /emis/record (medicationsRequest)
            mockingClient.forEmis {
                medicationsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/Medications.json"))
            }

            // GET /emis/record (problemsRequest)
            mockingClient.forEmis {
                problemsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/Problems.json"))
            }

            // GET /emis/record (consultationsRequest)
            mockingClient.forEmis {
                consultationsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/Consultations.json"))
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