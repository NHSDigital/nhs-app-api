package mocking.defaults

import config.Config
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.dataPopulation.journies.myRecord.MyRecordJournies
import mocking.dataPopulation.journies.prescriptions.PrescriptionsJournies
import mocking.defaults.dataPopulation.journies.appointmentSlots.AppointmentSlotsJournies
import mocking.defaults.dataPopulation.journies.courses.CoursesJournies
import mocking.defaults.dataPopulation.journies.im1Connection.Im1ConnectionJournies
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.linkage.LinkageJournies
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.defaults.dataPopulation.journies.session.SessionJournies
import mocking.emis.models.CourseRequestsGetResponse
import mocking.stubs.StubbedEnvironment
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import models.prescriptions.MedicationCourse
import worker.models.session.UserSessionRequest
import java.io.BufferedReader
import java.io.File
import java.io.FileNotFoundException
import java.io.IOException

const val BASE_NFT_DATA_DIR = "src/main/kotlin/mocking/defaults/dataPopulation/nft"
const val NFT_EMIS_USER_CSV = "$BASE_NFT_DATA_DIR/EmisUsers.csv"
const val CONNECTION_TOKEN_SUFFIX_LENGTH = 12

const val NUMBER_OF_PRESCRIPTIONS: Int = 5
const val NUMBER_OF_COURSES: Int = 5
const val NUMBER_OF_REPEAT_PRESCRIPTIONS: Int = 5

const val NFT_NUM_COLUMNS = 6
const val NFT_FIRST_NAME_IDX = 0
const val NFT_LAST_NAME_IDX = 1
const val NFT_DOB_IDX = 2
const val NFT_ODS_IDX = 3
const val NFT_IM1_CONNECTION_IDX = 4
const val NFT_NHS_NUMBER_IDX = 5

@Suppress("TooManyFunctions", "LargeClass", "LongMethod")
open class MockDataPopulate(private val mockingClient: MockingClient) {

    companion object {
        @JvmStatic
        fun main(arguments: Array<String>) {
            val client = MockingClient.instance
            if (arguments.isNotEmpty()) {
                val type = arguments[0]
                when (type.toLowerCase()) {
                    "nft" -> {
                        MockDataPopulate(client).populateNftStubs(numOfPatients = arguments[1].toInt())
                    }
                    "semistubbed" -> {
                        MockDataPopulate(client)
                                .populateSemiStubbedEnvironment(
                                        csvFileLocation = arguments.getOrElse(1) { NFT_EMIS_USER_CSV } )
                    }
                    "mockenvironment" -> {
                        StubbedEnvironment(client).generateEMISStubs()
                    }
                    else -> {
                        throw IllegalArgumentException("Type $type not recognised as a mock data set.")
                    }
                }
            } else {
                MockDataPopulate(client).populate()
            }
        }
    }

    fun populateForJustLoggedIn() {
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
            val pad = index.padStart(CONNECTION_TOKEN_SUFFIX_LENGTH, '0')
            val patient = Patient.montelFrye.copy(
                    firstName = "NFT.Patient",
                    surname = "Test$pad",
                    cidUserSession = UserSessionRequest(
                            authCode = "authCode$pad",
                            codeVerifier = "codeVerifier$pad",
                            redirectUrl = Config.instance.cidRedirectUri
                    ),
                    accessToken = "accessToken$pad",
                    endUserSessionId = "endUserSessionId",
                    connectionToken = "00000000-0000-0000-0000-$pad"
            )

            // Authentication
            CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
            EmisSessionCreateJourneyFactory(mockingClient).createFor(patient)

            // Appointment Mocks
            //GET /practice/ODSCODE/settings
            val settingsResponse = getFileContents("appointments/GetEmisSettings.json")

            mockingClient.forEmis {
                practiceSettingsRequest(patient)
                        .respondWithSuccessJson(settingsResponse)
            }

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
                bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken,
                                                                              1, "NFT Test Book Slot"))
                        .respondWithSuccessJson(postAppointmentRequestBody)
            }

            // DELETE /emis/appointments
            val deleteAppointmentRequestBody =
                    getFileContents("appointments/DeleteEmisAppointment.json")

            mockingClient.forEmis {
                cancelAppointmentRequest(patient, CancelAppointmentSlotFacade(patient.userPatientLinkToken,
                                                                              1, "No longer required"))
                        .respondWithSuccessJson(deleteAppointmentRequestBody)
            }

            // Prescriptions
            // GET /emis/prescriptionrequests
            val prescriptionsDataLoader = EmisPrescriptionLoader
            prescriptionsDataLoader.loadData(
                    noPrescriptions = NUMBER_OF_PRESCRIPTIONS,
                    noCourses = NUMBER_OF_COURSES,
                    noRepeats = NUMBER_OF_REPEAT_PRESCRIPTIONS,
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
                immunisationsRequest(patient).respondWithSuccessJson(getFileContents(
                        "medicalRecords/Immunisations.json"))
            }

            // GET /emis/record (allergiesRequest)
            mockingClient.forEmis {
                allergiesRequest(patient).respondWithSuccessJson(getFileContents(
                        "medicalRecords/Allergies.json"))
            }

            // GET /emis/record (medicationsRequest)
            mockingClient.forEmis {
                medicationsRequest(patient).respondWithSuccessJson(getFileContents(
                        "medicalRecords/Medications.json"))
            }

            // GET /emis/record (problemsRequest)
            mockingClient.forEmis {
                problemsRequest(patient).respondWithSuccessJson(getFileContents(
                        "medicalRecords/Problems.json"))
            }

            // GET /emis/record (consultationsRequest)
            mockingClient.forEmis {
                consultationsRequest(patient).respondWithSuccessJson(getFileContents(
                        "medicalRecords/Consultations.json"))
            }

            SuccessfulRegistrationJourney(mockingClient).create(patient)
        }
    }

    private fun populateSemiStubbedEnvironment(csvFileLocation: String) {
        mockingClient.clearWiremock()
        mockingClient.favicon()

        val patients = getPatientsFromCsv(csvFileLocation)

        for (patient in patients) {
            CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
            EmisSessionCreateJourneyFactory(mockingClient).createFor(patient)

            SuccessfulRegistrationJourney(mockingClient).create(patient)
        }
    }

    private fun getFileContents(relativePath: String): String {
        return getFileContents(relativePath, BASE_NFT_DATA_DIR)
    }

    @Suppress("TooGenericExceptionCaught")
    private fun getPatientsFromCsv(filePath: String): List<Patient> {
        val patients = ArrayList<Patient>()

        var fileReader: BufferedReader? = null

        try {
            var currentLine: String?
            var userNumber = 0

            fileReader = File(filePath)
                    .bufferedReader(
                            charset = Charsets.UTF_8,
                            bufferSize = DEFAULT_BUFFER_SIZE
                    )
            // Read CSV header
            fileReader.readLine()

            // Read the file line by line starting from the second line
            currentLine = fileReader.readLine()
            while (currentLine != null) {
                println("CURRENT LINE: $currentLine")
                val entries = currentLine.split(",")
                if (entries.isNotEmpty() && entries.size >= NFT_NUM_COLUMNS) {
                    patients.add(Patient(
                            title =  "",
                            firstName =  entries[NFT_FIRST_NAME_IDX],
                            surname =  entries[NFT_LAST_NAME_IDX],
                            dateOfBirth =  entries[NFT_DOB_IDX],
                            odsCode = entries[NFT_ODS_IDX],
                            connectionToken =  entries[NFT_IM1_CONNECTION_IDX],
                            nhsNumbers =  listOf(entries[NFT_NHS_NUMBER_IDX]),
                            cidUserSession = UserSessionRequest(
                                    authCode = "authCode$userNumber",
                                    codeVerifier = "codeVerifier$userNumber",
                                    redirectUrl = Config.instance.cidRedirectUri
                            )
                    ))
                    userNumber++
                }

                currentLine = fileReader.readLine()
            }
        } catch(e: FileNotFoundException) {
            println("Error, Could not find csv file: $filePath")
            e.printStackTrace()
        }
        catch (e: Exception) {
            println("Error when reading user csv!")
            e.printStackTrace()
        } finally {
            try {
                fileReader!!.close()
            } catch (e: IOException) {
                println("Closing fileReader Error!")
                e.printStackTrace()
            }
        }

        return patients
    }

    private fun getFileContents(relativePath: String, defaultDir: String): String {
        val pathname = "$defaultDir/$relativePath"
        return File(pathname)
                .bufferedReader(
                        charset = Charsets.UTF_8,
                        bufferSize = DEFAULT_BUFFER_SIZE
                )
                .readText()
                .replace("\t", "")
                .replace("\n", "")
    }
}
