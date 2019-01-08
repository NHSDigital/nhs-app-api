package mocking.defaults

import config.Config
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.defaults.dataPopulation.journies.im1Connection.SuccessfulRegistrationJourney
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.EmisSessionCreateJourneyFactory
import mocking.emis.models.CourseRequestsGetResponse
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import models.prescriptions.MedicationCourse
import worker.models.session.UserSessionRequest
import java.io.File

class MockDataPopulateNft(private val mockingClient: MockingClient) {

    fun populateNftStubs(numOfPatients: Int) {

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

            populateEmisAppointmentStubs(patient)
            populateEmisPrescriptionStubs(patient, pad)
            populateEmisRecordStubs(patient)

            SuccessfulRegistrationJourney(mockingClient).create(patient)
        }
    }

    private fun populateEmisAppointmentStubs(patient:Patient){
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
            appointments.viewMyAppointmentsRequest(patient = patient)
                    .respondWithSuccess(appointmentsBody)
        }

        // GET /emis/appointmentslots/meta
        val getAppointmentSlotsMetaBody =
                getFileContents("appointments/GetEmisAppointmentSlotsMeta.json")

        mockingClient.forEmis {
            appointments.appointmentSlotsMetaRequest(patient = patient)
                    .respondWithSuccessJson(getAppointmentSlotsMetaBody)
        }

        // GET /emis/appointmentslots
        val getAppointmentSlotsBody =
                getFileContents("appointments/GetEmisAppointmentSlots.json")

        mockingClient.forEmis {
            appointments.appointmentSlotsRequest(patient = patient)
                    .respondWithSuccessJson(getAppointmentSlotsBody)
        }

        // POST /emis/appointments
        val postAppointmentRequestBody =
                getFileContents("appointments/PostEmisAppointment.json")

        mockingClient.forEmis {
            appointments.bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken,
                    1, "NFT Test Book Slot"))
                    .respondWithSuccessJson(postAppointmentRequestBody)
        }

        // DELETE /emis/appointments
        val deleteAppointmentRequestBody =
                getFileContents("appointments/DeleteEmisAppointment.json")

        mockingClient.forEmis {
            appointments.cancelAppointmentRequest(patient, CancelAppointmentSlotFacade(patient.userPatientLinkToken,
                    1, "No longer required"))
                    .respondWithSuccessJson(deleteAppointmentRequestBody)
        }
    }

    private fun populateEmisPrescriptionStubs(patient: Patient, pad: String){
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
            prescriptions.prescriptionsRequest(patient)
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
            prescriptions.coursesRequest(patient)
                    .respondWithSuccess(CourseRequestsGetResponse(coursesLoader.data as List<MedicationCourse>))
        }

        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(patient)
                    .respondWithCreated()
                    .inScenario(pad)
                    .whenScenarioStateIs("Started")
        }
    }

    private fun populateEmisRecordStubs(patient:Patient){
        // My medical Record
        // GET /emis/record (testResultsRequest)
        mockingClient.forEmis {
            myRecord.testResultsRequest(patient).respondWithSuccessJson(getFileContents("medicalRecords/TestResults" +
                    ".json"))
        }

        // GET /emis/record (immunisationsRequest)
        mockingClient.forEmis {
            myRecord.immunisationsRequest(patient).respondWithSuccessJson(getFileContents(
                    "medicalRecords/Immunisations.json"))
        }

        // GET /emis/record (allergiesRequest)
        mockingClient.forEmis {
            myRecord.allergiesRequest(patient).respondWithSuccessJson(getFileContents(
                    "medicalRecords/Allergies.json"))
        }

        // GET /emis/record (medicationsRequest)
        mockingClient.forEmis {
            myRecord.medicationsRequest(patient).respondWithSuccessJson(getFileContents(
                    "medicalRecords/Medications.json"))
        }

        // GET /emis/record (problemsRequest)
        mockingClient.forEmis {
            myRecord.problemsRequest(patient).respondWithSuccessJson(getFileContents(
                    "medicalRecords/Problems.json"))
        }

        // GET /emis/record (consultationsRequest)
        mockingClient.forEmis {
            myRecord.consultationsRequest(patient).respondWithSuccessJson(getFileContents(
                    "medicalRecords/Consultations.json"))
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

}
