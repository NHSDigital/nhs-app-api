package features.prescriptions.stepDefinitions

import cucumber.api.java.en.*
import features.authentication.steps.LoginSteps
import features.prescriptions.PrescriptionsData
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.CourseRequestsGetResponse
import mocking.emis.models.MedicationCourse
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.PrescriptionType
import models.Patient
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus
import org.joda.time.DateTime
import org.junit.Assert
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
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

    lateinit var prescriptionsPage: PrescriptionsPage
    lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage

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
        Assert.assertNotNull(prescriptionListResponse)
        Assert.assertTrue(prescriptionListResponse.response.prescriptions.isNotEmpty())
        Assert.assertTrue(prescriptionListResponse.response.courses.isNotEmpty())
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

    @Given ("prescriptions is disabled at a GP Practice level")
    fun prescriptionsIsDisabledAtAGPLevel() {
        mockingClient
            .forEmis {
                prescriptionsRequest(patient).respondWithPrescriptionsNotEnabled()
            }

        mockingClient
            .forEmis {
                coursesRequest(patient).respondWithPrescriptionsNotEnabled()
            }
    }

    @Then("I see a message informing me that I don't currently have access to this service")
    fun iSeeAMessageInformingMeThatIdontCurrentlyHaveAccessToThisService() {
        val em = prescriptions.prescriptions.getErrorText()

        val errorTitle = "Sorry, you don't currently have access to this service"
        val errorContent = "Contact your GP surgery for more information."

        Assert.assertEquals("$errorTitle$errorContent", em)
    }

    @But("The prescriptions endpoint is timing out")
    fun butThePrescriptionsEndpointIsTimingOut(){
        mockingClient
                .forEmis {
                    prescriptionsRequest(patient)
                            .respondWith(504, resolve = {}, milliSecondDelay = 10000)
                }
    }


    @But("The prescriptions endpoint is throwing a server error")
    fun butThePrescriptionsEndpointIsThrowingAServerError(){
        mockingClient
                .forEmis {
                    prescriptionsRequest(patient)
                            .respondWith(500, resolve = {})
                }
    }

    @But("The courses endpoint is timing out")
    fun butTheCoursesEndpointIsTimingOut() {
        mockingClient.forEmis { coursesRequest(patient)
                .respondWith(504, resolve = {}, milliSecondDelay = 10000)
        }
    }

    @But("The courses endpoint is throwing a server error")
    fun butTheCoursesEndpointIsThrowingAServerError() {
        mockingClient.forEmis { coursesRequest(patient)
                .respondWith(500, resolve = {}) }
    }

    @But("The prescription submission endpoint is timing out")
    fun butThePrescriptionSubmissionEndpointIsTimingOut() {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(MockDefaults.patient).respondWith(504, resolve = {}, milliSecondDelay = 10000) }
    }

    @But("The prescription submission endpoint is throwing a server error")
    fun butThePrescriptionSubmissionEndpointIsThrowingAServerError() {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(MockDefaults.patient).respondWith(500, resolve = {}) }
    }

    @Then("I see the appropriate error message for a prescription timeout")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionTimeout() {

        var pageTitle = prescriptionsPage.timeoutPageTitle
        var pageHeader = prescriptionsPage.timeoutPageHeader
        var header = prescriptionsPage.timeoutHeader
        var subHeader = prescriptionsPage.timeoutSubHeader
        var message = prescriptionsPage.timeoutMessage
        var retryButtonText = prescriptionsPage.timeoutRetryButtonText

        prescriptions.assertCorrectErrorMessageShown(pageTitle, pageHeader, header, subHeader, message, retryButtonText)
    }

    @Then("I see the appropriate error message for a prescription server error")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionServerError() {

        var pageTitle = prescriptionsPage.serverErrorPageTitle
        var pageHeader = prescriptionsPage.serverErrorPageHeader
        var header = prescriptionsPage.serverErrorHeader
        var subHeader = prescriptionsPage.serverErrorSubHeader
        var message = prescriptionsPage.serverErrorMessage
        var retryButtonText = prescriptionsPage.serverErrorretryButtonText

        prescriptions.assertCorrectErrorMessageShown(pageTitle, pageHeader, header, subHeader, message, retryButtonText)
    }

    @Then("I see the appropriate error message for a course request error")
    fun thenISeeTheAppropriateErrorMessageForACourseRequestError() {

        var pageTitle = confirmRepeatPrescriptionsOrderPage.serverErrorPageTitle
        var pageHeader = confirmRepeatPrescriptionsOrderPage.serverErrorPageHeader
        var header = confirmRepeatPrescriptionsOrderPage.serverErrorHeader
        var subHeader = confirmRepeatPrescriptionsOrderPage.serverErrorSubHeader
        var message = confirmRepeatPrescriptionsOrderPage.serverErrorMessage
        var retryButtonText = confirmRepeatPrescriptionsOrderPage.serverErrorRetryButtonText

        prescriptions.assertCorrectErrorMessageShown(pageTitle, pageHeader, header, subHeader, message, retryButtonText)
    }

    @Then("I select (\\d+) prescription to order")
    fun iSelectXPrescriptionsToOrder(prescriptionToOrder: Int){
        prescriptions.selectSubscriptionsToOrder(prescriptionToOrder)
        prescriptions.clickContinue()
        prescriptions.clickConfirmAndOrderRepeat()
    }

    @Then("I am kicked back to the login page")
    fun thenIAmKickedBackToTheLoginPage() {
        login.assertPageIsDisplayed()

        // There is a bug with NHSO-415, which means that this banner isn't shown on a server-session timeout. Uncomment once bug is fixed.
        // login.assertTimeoutBannerIsShown()
    }
}
