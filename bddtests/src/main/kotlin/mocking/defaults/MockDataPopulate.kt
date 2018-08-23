package mocking.defaults

import config.Config
import mocking.MockingClient
import mocking.data.myrecord.AllergiesData
import mocking.data.myrecord.ConsultationsData
import mocking.data.myrecord.ImmunisationsData
import mocking.data.myrecord.MedicationsData
import mocking.data.myrecord.ProblemsData
import mocking.data.myrecord.TestResultsData
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
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import models.prescriptions.MedicationCourse
import org.apache.http.HttpStatus
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import worker.models.session.UserSessionRequest
import java.io.File
import java.time.Duration

const val BASE_NFT_DATA_DIR = "src/main/kotlin/mocking/defaults/dataPopulation/nft"
const val BASE_MOCK_DATA_DIR = "src/main/kotlin/mocking/defaults/dataPopulation/mockEnvironmentData"
const val CONNECTION_TOKEN_SUFFIX_LENGTH = 12
const val NUMBER_OF_PRESCRIPTIONS: Int = 5
const val NUMBER_OF_COURSES: Int = 5
const val NUMBER_OF_REPEAT_PRESCRIPTIONS: Int = 5
const val TIMEOUT_DELAY_MILLISECONDS: Int = 71000
const val TIMEOUT_DELAY_SECONDS: Long = 71
const val EMIS_RESULT_COUNT: Int = 6

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
                    "mockenvironment" -> {
                        MockDataPopulate(client).populateEMISStubEnvironment()
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

    private fun populateEMISStubEnvironment() {
        mockingClient.clearWiremock()
        mockingClient.favicon()

        val patientOne = generateUniquePatientData("1", CONNECTION_TOKEN_SUFFIX_LENGTH)
        generateEMISStubs(patientOne)
        generateEMISMyMedicalRecordsStubs(patientOne)

        val patientTwo = generateUniquePatientData("2", CONNECTION_TOKEN_SUFFIX_LENGTH)
        generateEMISStubs(patientTwo)
        generateEMISMyMedicalRecordsNotEnabledErrorStub(patientTwo)

        val patientThree = generateUniquePatientData("3", 2)
        generateEMISStubs(patientThree)
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

            SuccessfulRegistrationJourney(mockingClient).create(patient)
        }
    }

    private fun getFileContents(relativePath: String): String {
        return getFileContents(relativePath, BASE_NFT_DATA_DIR)
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

    private fun generateEMISAppointmentStubs(patientForStubEnvironment: Patient) {
        // GET /emis/appointments
        val appointmentsBody = getFileContents("appointments/GetEmisAppointments.json", BASE_MOCK_DATA_DIR)

        mockingClient.forEmis {
            viewMyAppointmentsRequest(patient = patientForStubEnvironment)
                    .respondWithSuccess(appointmentsBody)
        }
        // GET /emis/appointmentslots/meta
        val getAppointmentSlotsMetaBody =
                getFileContents("appointments/GetEmisAppointmentSlotsMeta.json", BASE_MOCK_DATA_DIR)

        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient = patientForStubEnvironment)
                    .respondWithSuccessJson(getAppointmentSlotsMetaBody)
        }

        // GET /emis/appointmentslots
        val getAppointmentSlotsBody =
                getFileContents("appointments/GetEmisAppointmentSlots.json", BASE_MOCK_DATA_DIR)

        mockingClient.forEmis {
            appointmentSlotsRequest(patient = patientForStubEnvironment)
                    .respondWithSuccessJson(getAppointmentSlotsBody)
        }

        // POST /emis/appointments
        val postAppointmentRequestBody =
                getFileContents("appointments/PostEmisAppointment.json", BASE_MOCK_DATA_DIR)

        mockingClient.forEmis {
            bookAppointmentSlotRequest(patientForStubEnvironment, BookAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "give me a good response"))
                    .respondWithSuccessJson(postAppointmentRequestBody)
        }

        mockingClient.forEmis {
            bookAppointmentSlotRequest(patientForStubEnvironment, BookAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "give me a service not available response"))
                    .respondWithUnavailableException()
        }

        mockingClient.forEmis {
            bookAppointmentSlotRequest(patientForStubEnvironment, BookAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "give me an appointment not found response"))
                    .respondWithExceptionWhenNotAvailable()
        }

        mockingClient.forEmis {
            bookAppointmentSlotRequest(patientForStubEnvironment, BookAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "give me a service not enabled response"))
                    .respondWithExceptionWhenNotEnabled()
        }

        //POST /emis/appointments - time out scenario
        mockingClient.forEmis {
            bookAppointmentSlotRequest(patientForStubEnvironment, BookAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "give me a time out response"))
                    .respondWith(HttpStatus.SC_OK, TIMEOUT_DELAY_MILLISECONDS, resolve = {})
        }

        mockingClient.forEmis {
            cancelAppointmentRequest(patientForStubEnvironment, CancelAppointmentSlotFacade(patientForStubEnvironment.userPatientLinkToken, 1, "cancel appointment"))
                    .respondWithSuccess()
        }
    }

    private fun generateEMISPrescriptionsStubs(patientForStubEnvironment: Patient) {
        val prescriptionsDataLoader = EmisPrescriptionLoader
        prescriptionsDataLoader.loadData(
                noPrescriptions = NUMBER_OF_PRESCRIPTIONS,
                noCourses = NUMBER_OF_COURSES,
                noRepeats = NUMBER_OF_REPEAT_PRESCRIPTIONS,
                showDosage = true,
                showQuantity = true
        )

        // GET /emis/prescriptionrequests  Success - Prescription History
        mockingClient.forEmis {
            prescriptionsRequest(patientForStubEnvironment)
                    .respondWithSuccess(prescriptionsDataLoader.data)
                    .whenScenarioStateIs("Started")
        }

        // GET /emis/courses
        val coursesLoader = EmisCoursesLoader
        coursesLoader.loadData(
                maxCourses = 1,
                numOfRepeats = 1,
                numCanBeRequested = 1,
                includeDosage = true,
                includeQuantity = true
        )

        mockingClient.forEmis {
            coursesRequest(patientForStubEnvironment)
                    .respondWithSuccess(CourseRequestsGetResponse(coursesLoader.data))
        }

        //Repeat prescription submission request - error scenario
        val courseListForOrderingPrescription = coursesLoader.data
        var uuids: MutableList<String> = mutableListOf()
        uuids.add(courseListForOrderingPrescription[0].medicationCourseGuid)

        //Success scenario - Prescription ordered
        val prescriptionSubmissionRequestGood = PrescriptionSubmissionRequest(uuids, "give me a good response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestGood)
                    .respondWithCreated()
                    .whenScenarioStateIs("Started")
        }

        //Error scenario - Prescriptions not Enabled
        val prescriptionSubmissionRequestNotEnabled = PrescriptionSubmissionRequest(uuids, "give me prescription not enabled response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestNotEnabled)
                    .respondWithPrescriptionsNotEnabled()
                    .whenScenarioStateIs("Started")
        }

        //Error scenario - Prescriptions not submitted
        val prescriptionSubmissionRequestNotSubmitted = PrescriptionSubmissionRequest(uuids, "give me prescription not submitted response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestNotSubmitted)
                    .respondWithGenericInternalServerError()
                    .whenScenarioStateIs("Started")
        }

        //Error scenario - Pending request in last 30 days
        val prescriptionSubmissionRequestWithPendingRequest = PrescriptionSubmissionRequest(uuids, "give me already pending request response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestWithPendingRequest)
                    .respondWithAlreadyAPendingRequestInTheLast30Days()
                    .whenScenarioStateIs("Started")
        }

        //Error scenario - Course invalid request
        val prescriptionSubmissionRequestInvalid = PrescriptionSubmissionRequest(uuids, "give me course invalid response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestInvalid)
                    .respondWithBadRequestErrorIndicatingACourseIsInvalid()
                    .whenScenarioStateIs("Started")
        }

        //Error scenario - Prescriptions time out
        val prescriptionSubmissionRequestTimeOut = PrescriptionSubmissionRequest(uuids, "give me a time out response")
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patientForStubEnvironment, prescriptionSubmissionRequestTimeOut)
                    .respondWithCreated().delayedBy(Duration.ofSeconds(TIMEOUT_DELAY_SECONDS))
                    .whenScenarioStateIs("Started")
        }
    }

    private fun generateEMISMyMedicalRecordsStubs(patientForStubEnvironment: Patient) {
        // GET /emis/record (testResultsRequest)
        mockingClient.forEmis {
            testResultsRequest(patientForStubEnvironment).respondWithSuccess(TestResultsData.getTestResultsForEmis(EMIS_RESULT_COUNT))
        }

        // GET /emis/record (immunisationsRequest)
        mockingClient.forEmis {
            immunisationsRequest(patientForStubEnvironment).respondWithSuccess(ImmunisationsData.getImmunisationsData())
        }

        // GET /emis/record (allergiesRequest)
        mockingClient.forEmis {
            allergiesRequest(patientForStubEnvironment).respondWithSuccess(AllergiesData.getEmisAllergyRecordsWithDifferentDateParts())
        }

        // GET /emis/record (medicationsRequest)
        mockingClient.forEmis {
            medicationsRequest(patientForStubEnvironment).respondWithSuccess(MedicationsData.getEmisMedicationData())
        }

        // GET /emis/record (problemsRequest)
        mockingClient.forEmis {
            problemsRequest(patientForStubEnvironment).respondWithSuccess(ProblemsData.getProblemsData())
        }

        // GET /emis/record (consultationsRequest)
        mockingClient.forEmis {
            consultationsRequest(patientForStubEnvironment).respondWithSuccess(ConsultationsData.getMultipleConsultationRecords())
        }
    }

    private fun generateEMISMyMedicalRecordsNotEnabledErrorStub(patientForStubEnvironment: Patient) {
        // GET /emis/record (testResultsRequest)
        mockingClient.forEmis {
            testResultsRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }

        // GET /emis/record (immunisationsRequest)
        mockingClient.forEmis {
            immunisationsRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }

        // GET /emis/record (allergiesRequest)
        mockingClient.forEmis {
            allergiesRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }

        // GET /emis/record (medicationsRequest)
        mockingClient.forEmis {
            medicationsRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }

        // GET /emis/record (problemsRequest)
        mockingClient.forEmis {
            problemsRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }

        // GET /emis/record (consultationsRequest)
        mockingClient.forEmis {
            consultationsRequest(patientForStubEnvironment).respondWithExceptionWhenNotEnabled()
        }
    }

    private fun generateEMISStubs(forPatient: Patient) {
        CitizenIdSessionCreateJourney(mockingClient).createFor(forPatient)
        EmisSessionCreateJourneyFactory(mockingClient).createFor(forPatient)
        generateEMISAppointmentStubs(forPatient)
        generateEMISPrescriptionsStubs(forPatient)
    }

    private fun generateUniquePatientData(uniqueId: String, length: Int): Patient {
        val pad = uniqueId.padStart(length, '0')
        //do not add end user session id here
        val patient = Patient.picaJones.copy(
                firstName = "Test",
                surname = "Patient$pad",
                cidUserSession = UserSessionRequest(
                        authCode = "authCode$pad",
                        codeVerifier = "codeVerifier$pad",
                        redirectUrl = Config.instance.cidRedirectUri
                ),
                accessToken = "accessToken$pad",
                connectionToken = "00000000-0000-0000-0000-$pad",
                userPatientLinkToken = "userPatientLinkToken$pad"
        )
        return patient
    }
}