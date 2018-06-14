package features.prescriptions.stepDefinitions

import cucumber.api.java.en.*
import features.authentication.steps.LoginSteps
import features.prescriptions.PrescriptionsData
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import junit.framework.Assert.assertNotNull
import junit.framework.Assert.assertTrue
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.MedicationCourse
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.PrescriptionType
import models.Patient
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.joda.time.DateTime
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionListResponse
import java.time.Duration
import java.time.OffsetDateTime
import java.time.ZonedDateTime

open class PrescriptionsStepDefinitions {

    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    val mockingClient = MockingClient.instance
    val patient = Patient.montelFrye
    val FROM_DATE = "FromDate"
    val TO_DATE = OffsetDateTime.now()
    val HTTP_EXCEPTION = "HttpException"
    val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY: Long = 6
    var numberOfPrescriptions: Int = 0
    lateinit var currentPatient: Patient
    var fromDate: String? = null

    lateinit var prescriptionListResponse: PrescriptionListResponse

    lateinit var prescriptionsMock: PrescriptionRequestsGetResponse

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps

    @Then("^I see prescriptions page loaded$")
    fun iSeePrecriptionsPageLoaded() {
        prescriptions.isLoaded()
    }

    @Then("^I see no prescriptions$")
    fun iSeeNoPrescriptions() {
        prescriptions.assertNoRepeatPrescriptionsMessageShown()
    }

    @Then("^I see a message indicating that I have no repeat prescriptions$")
    fun thenISeeAMessageIndicatingThatIHaveNoRepeatPrescriptions() {
        prescriptions.assertNoRepeatPrescriptionsMessageShown()
    }

    @When("^I am on the prescriptions page")
    fun whenIAmOnThePrescriptionsPage() {
        browser.goToApp()
        login.asDefault()
        navigation.select("Prescriptions")
    }

    @And("^each repeat prescription contains (\\d+) courses of which (\\d+) are repeats$")
    fun givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(numOfCourses: Int, numOfRepeats: Int) {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        prescriptionsMock = PrescriptionsData.loadPrescriptionsData(numberOfPrescriptions, numOfCourses * numberOfPrescriptions, numOfRepeats * numberOfPrescriptions)

        mockingClient
                .forEmis {
                    prescriptionsRequest(patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsMock)
                }
    }

    @And("^each repeat prescription shares the same course")
    fun givenEachRepeatPrescriptionSharesTheSameCourse() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        prescriptionsMock = PrescriptionsData.loadPrescriptionsData(numberOfPrescriptions, 1, 1)

        mockingClient
                .forEmis {
                    prescriptionsRequest(patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsMock)
                }
    }

    @Given("From date is 6 months ago and I have 10 prescriptions in the last 6 months")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths() {
        var EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        var prescriptionsData: PrescriptionRequestsGetResponse = PrescriptionsData.loadPrescriptionsData(10, 10, 10)

        mockingClient
                .forEmis {
                    prescriptionsRequest(MockDefaults.patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsData)
                }
    }

    @When("I get the users prescriptions with a valid cookie")
    fun whenIGetTheUsersPrescriptionsWithAValidCookie() {
        var formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(FROM_DATE)

        try {
            prescriptionListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(if (formattedFromDate != null) formattedFromDate.toString() else formattedFromDate, null)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive a list of 10 prescriptions")
    fun thenIReceiveAListOfTenPrescriptions() {
        Assert.assertNotNull(prescriptionListResponse)
        Assert.assertEquals(10, prescriptionListResponse.response.prescriptions.count())
        var prescriptions = prescriptionListResponse.response.prescriptions

        // We had to use a string here and then parse the screen as kotlin did not like the date time format sent from the worker
        for (int in 0 until prescriptions.count() - 2) {
            Assert.assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate)!! >= ZonedDateTime.parse(prescriptions[int + 1].orderDate))
        }
    }

    @Given("^I have (\\d+) past repeat prescriptions$")
    fun givenIHaveXPastRepeatPrescriptions(numPrescriptions: Int) {
        numberOfPrescriptions = numPrescriptions
    }

    @Then("^I see (\\d+) prescriptions$")
    fun thenISeeXPrescriptions(numPrescriptions: Int) {
        prescriptions.assertPrescriptionsMatch(getExpectedNumPrescriptions(prescriptionsMock), numPrescriptions)
    }

    @And("^I have a patient$")
    fun iHaveAPatient() {
        currentPatient = patient
    }

    @And("^the patient has no prescriptions in the last 6 months")
    fun thePatientHasNoPrescriptionsInTheLastSixMonths() {

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        prescriptionsMock = PrescriptionsData.loadPrescriptionsData(0, 0, 0)

        mockingClient
                .forEmis {
                    prescriptionsRequest(currentPatient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsMock)
                }
    }

    @But("^I do not request a fromDate$")
    fun iDoNotRequestAFromDate() {
        fromDate = null
    }

    @But("^the GP System is too slow$")
    fun butTheGPSystemIsTooSlow() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        prescriptionsMock = PrescriptionsData.loadPrescriptionsData(1, 1, 1)

        mockingClient
                .forEmis {
                    prescriptionsRequest(currentPatient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsMock).delayedBy(Duration.ofSeconds(31))
                }
    }

    @When("^I request prescriptions for the last 6 months$")
    fun iRequestPrescriptionsForTheLastSixMonths() {
        try {
            prescriptionListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(fromDate, null)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I request prescriptions for the last 6 months with an invalid cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAnInvalidCookie() {
        try {
            prescriptionListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(fromDate, WorkerClient.getHttpContext(true))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I get a response with a list of prescriptions for the last 6 months$")
    fun iGetAResponseWithAListOfPrescriptionForTheLastSixMonths() {
        assertNotNull(prescriptionListResponse)
        assertTrue(prescriptionListResponse.response.prescriptions.isNotEmpty())
        assertTrue(prescriptionListResponse.response.courses.isNotEmpty())
    }

    @But("^a fromDate in an unexpected format$")
    fun aFromDateInAnUnexpectedFormat() {
        fromDate = "13/66/99999999T"

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @But("^a fromDate in the future$")
    fun aFromDateInTheFuture() {
        fromDate = TO_DATE.plusMonths(1).toString()

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @But("^a fromDate greater than 6 months ago$")
    fun aFromDateGreaterThanSixMonths() {
        fromDate = TO_DATE.minusMonths(7).toString()

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @But("^no cookie$")
    fun noCookie() {
        // No implementation is needed
    }

    @But("^the GP System has disabled prescriptions$")
    fun theGPSystemHasDisabledPrescriptions() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        mockingClient
                .forEmis {
                    prescriptionsRequest(currentPatient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithPrescriptionsNotEnabled()
                }
    }

    fun getDefaultPrescriptionsFromDate(dateNow: OffsetDateTime): OffsetDateTime {
        return dateNow.minusMonths(PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY)
    }

    fun getExpectedNumPrescriptions(data: PrescriptionRequestsGetResponse): ArrayList<HistoricPrescription>{

        var totalCoursesRunningTotal = 0

        var repeatCourses = data.medicationCourses.filter { it.prescriptionType == PrescriptionType.Repeat }

        var repeatCourseguids = getCourseGuids(repeatCourses)

        var historicPrescriptions = ArrayList<HistoricPrescription>()

        for (prescription in data.prescriptionRequests.toList().sortedByDescending { it.DateRequested }) {

            if (totalCoursesRunningTotal >= 100) {
                break
            }

            var datetime = DateTime.parse(prescription.DateRequested).toString("d MMM yyyy")

            var repeaCoursesInPrescription = prescription.requestedMedicationCourses.filter { it -> repeatCourseguids.contains(it.requestedMedicationCourseGuid) }

            for (courseEntry in repeaCoursesInPrescription) {


                var course = repeatCourses.toList().filter { it.medicationCourseGuid == courseEntry.requestedMedicationCourseGuid }.single()

                var historicPrescription = HistoricPrescription(
                        orderDate = datetime,
                        name = course.name,
                        dosage = course.dosage + " - " + course.quantityRepresentation
                )

                historicPrescriptions.add(historicPrescription)
            }

            totalCoursesRunningTotal += repeaCoursesInPrescription.size
        }

        return historicPrescriptions
    }

    private fun getCourseGuids(repeatCourses: List<MedicationCourse>): ArrayList<String> {
        var courseGuids = ArrayList<String>()

        repeatCourses.forEach { it -> courseGuids.add(it.medicationCourseGuid) }

        return courseGuids
    }
}