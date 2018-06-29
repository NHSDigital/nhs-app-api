package features.prescriptions.stepDefinitions

import cucumber.api.DataTable
import cucumber.api.java.en.*
import features.authentication.steps.LoginSteps
import features.prescriptions.loaders.EmisPrescriptionLoader
import features.prescriptions.loaders.TppPrescriptionLoader
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.patient
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsData
import mocking.emis.models.MedicationCourse
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.PrescriptionType
import mocking.emis.models.RequestedMedicationCourseStatus
import mocking.tpp.models.ListRepeatMedicationReply
import models.Patient
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.apache.http.HttpStatus
import org.joda.time.DateTime
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionsListResponse
import java.time.Duration
import java.time.OffsetDateTime
import java.time.ZonedDateTime
import pages.prescription.RepeatPrescriptionsPage;

open class PrescriptionsStepDefinitions {

    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    val emisPatient = Patient.montelFrye
    val tppPatient = Patient.kevinBarry

    val FROM_DATE = "FromDate"
    val TO_DATE = OffsetDateTime.now()
    val HTTP_EXCEPTION = "HttpException"
    val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY: Long = 6
    var numberOfPrescriptions: Int = 0
    var numOfCourses: Int = 0
    var numOfRepeats: Int = 0
    var fromDate: String? = null
    lateinit var currentGPSystem: String
    lateinit var currentPatient: Patient

    private val EMIS = "EMIS"
    private val TPP = "TPP"

    lateinit var PrescriptionsListResponse: PrescriptionsListResponse

    lateinit var emisPrescriptionsMock: PrescriptionRequestsGetResponse
    lateinit var tppMedicationDataMock: ListRepeatMedicationReply

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
        this.numOfCourses = numOfCourses
        this.numOfRepeats = numOfRepeats

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE,
                numberOfPrescriptions,
                numOfCourses * numberOfPrescriptions,
                numOfRepeats * numberOfPrescriptions )
    }

    @And("^each repeat prescription shares the same course")
    fun givenEachRepeatPrescriptionSharesTheSameCourse() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE, numberOfPrescriptions, 1, 1)
    }

    @Given("From date is 6 months ago and I have 10 prescriptions in the last 6 months")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths() {
        var EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE, 10, 10, 10)
    }

    @When("I get the users prescriptions with a valid cookie")
    fun whenIGetTheUsersPrescriptionsWithAValidCookie() {
        var formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(FROM_DATE)

        try {
            var sessionVariable = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            PrescriptionsListResponse = sessionVariable.getPrescriptionsConnection(if (formattedFromDate != null) formattedFromDate.toString() else formattedFromDate, null)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive a list of (\\d+) prescriptions")
    fun thenIReceiveAListOfTenPrescriptions(count: Int) {
        println(currentGPSystem)

        Assert.assertNotNull(PrescriptionsListResponse)

        if(currentGPSystem == EMIS){
            Assert.assertEquals(count, PrescriptionsListResponse.prescriptions.count())
            var prescriptions = PrescriptionsListResponse.prescriptions

            // We had to use a string here and then parse the screen as kotlin did not like the date time format sent from the worker
            for (int in 0 until prescriptions.count() - 2) {
                Assert.assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate)!! >= ZonedDateTime.parse(prescriptions[int + 1].orderDate))
            }
        }
        else if(currentGPSystem == TPP) {
            Assert.assertEquals(count, PrescriptionsListResponse.courses.count())
        }
        else{
            throw Exception("Invalid GP System")
        }
    }

    @Given("^I am using (.*) GP System$")
    fun givenIHaveXPastRepeatPrescriptions(gpSystem: String) {
        currentGPSystem = gpSystem

        when (currentGPSystem) {
            EMIS -> {
                currentPatient = emisPatient
            }
            TPP -> {
                currentPatient = tppPatient
            }
        }
    }

    @Given("^I have (\\d+) past repeat prescriptions$")
    fun givenIHaveXPastRepeatPrescriptions(numPrescriptions: Int) {
        numberOfPrescriptions = numPrescriptions
    }

    @Then("^I see expected prescriptions$")
    fun thenISeeXPrescriptions() {
        val expectedPrescriptions
                = getResponseToExpectedPrescriptionFormat()

        prescriptions.assertPrescriptionsMatch(expectedPrescriptions, expectedPrescriptions.size, true)
    }

    @Then("^I see (\\d+) prescriptions$")
    fun thenISeeXPrescriptions(numPrescriptions: Int) {

        var isEmis = (currentGPSystem == EMIS)

        prescriptions
                .assertPrescriptionsMatch(
                        getResponseToExpectedPrescriptionFormat(),
                        numPrescriptions,
                        isEmis)
    }

    @And("^I have a patient$")
    fun iHaveAPatient() {

        currentGPSystem = Serenity.getCurrentSession().get("GP_SYSTEM").toString()

        when (currentGPSystem) {
            EMIS -> {
                currentPatient = emisPatient
            }
            TPP -> {
                currentPatient = tppPatient
            }
        }
    }

    @And("^the patient has no prescriptions in the last 6 months")
    fun thePatientHasNoPrescriptionsInTheLastSixMonths() {

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE,0,0,0)
    }

    @But("^I do not request a fromDate$")
    fun iDoNotRequestAFromDate() {
        fromDate = null
    }

    @But("^the GP System is too slow$")
    fun butTheGPSystemIsTooSlow() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        setupWiremockAndDataWithDelay(EXPECTED_DEFAULT_FROM_DATE,1,1,1)
    }

    @When("^I request prescriptions for the last 6 months$")
    fun iRequestPrescriptionsForTheLastSixMonths() {
        try {
            PrescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(fromDate, null)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I request prescriptions for the last 6 months with an invalid cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAnInvalidCookie() {
        try {
            PrescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(fromDate, WorkerClient.getHttpContext(true))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I get a response with a list of prescriptions for the last 6 months$")
    fun iGetAResponseWithAListOfPrescriptionForTheLastSixMonths() {
        Assert.assertNotNull(PrescriptionsListResponse)
        Assert.assertTrue(PrescriptionsListResponse.prescriptions.isNotEmpty())
        Assert.assertTrue(PrescriptionsListResponse.courses.isNotEmpty())
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

        when (currentGPSystem) {
            EMIS -> {
                mockingClient
                        .forEmis {
                            prescriptionsRequest(currentPatient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                                    .respondWithPrescriptionsNotEnabled()
                        }
            }
            TPP -> {
                mockingClient
                        .forTpp {
                            listRepeatMedication(currentPatient)
                                    .respondWith(403, 0, resolve = {})
                        }
            }
        }


    }

    fun getDefaultPrescriptionsFromDate(dateNow: OffsetDateTime): OffsetDateTime {
        return dateNow.minusMonths(PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY)
    }

    @Given ("prescriptions is disabled at a GP Practice level")
    fun prescriptionsIsDisabledAtAGPLevel() {
        mockingClient
            .forEmis {
                prescriptionsRequest(currentPatient).respondWithPrescriptionsNotEnabled()
            }

        mockingClient
            .forEmis {
                coursesRequest(currentPatient).respondWithPrescriptionsNotEnabled()
            }
    }

    @Then("I see a message informing me that I don't currently have access to this service")
    fun iSeeAMessageInformingMeThatIdontCurrentlyHaveAccessToThisService() {
        val em = prescriptions.prescriptions.getErrorText()

        val errorTitle = "Sorry, you don't currently have access to this service"
        val errorContent = "Contact your GP surgery for more information."

        assertEquals("$errorTitle\n$errorContent", em)
    }

    @But("The prescriptions endpoint is timing out")
    fun butThePrescriptionsEndpointIsTimingOut(){
        mockingClient
                .forEmis {
                    prescriptionsRequest(currentPatient)
                            .respondWith(504, resolve = {}, milliSecondDelay = 15000)
                }
    }


    @But("The prescriptions endpoint is throwing a server error")
    fun butThePrescriptionsEndpointIsThrowingAServerError(){
        mockingClient
                .forEmis {
                    prescriptionsRequest(currentPatient)
                            .respondWith(500, resolve = {})
                }
    }

    @But("The courses endpoint is timing out")
    fun butTheCoursesEndpointIsTimingOut() {
        mockingClient.forEmis { coursesRequest(currentPatient)
                .respondWith(504, resolve = {}, milliSecondDelay = 15000)
        }
    }

    @But("The courses endpoint is throwing a server error")
    fun butTheCoursesEndpointIsThrowingAServerError() {
        mockingClient.forEmis { coursesRequest(currentPatient)
                .respondWith(500, resolve = {}) }
    }

    @But("The prescription submission endpoint is timing out")
    fun butThePrescriptionSubmissionEndpointIsTimingOut() {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(MockDefaults.patient).respondWith(504, resolve = {}, milliSecondDelay = 15000) }
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

    @And("each course has (.*)")
    fun eachCourseHasX(contents: String) {
        val showDosage = contents.toLowerCase().contains("dosage")
        val showQuantity = contents.toLowerCase().contains("quantity")

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        when(currentGPSystem) {
            EMIS -> {
                emisPrescriptionsMock = EmisPrescriptionLoader.loadPrescriptionsData(
                        numberOfPrescriptions,
                        numOfCourses * numberOfPrescriptions,
                        numOfRepeats * numberOfPrescriptions,
                        showDosage, showQuantity)

                mockingClient
                        .forEmis {
                            prescriptionsRequest(patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                                    .respondWithSuccess(emisPrescriptionsMock)
                        }
            }
            TPP -> {
                tppMedicationDataMock = TppPrescriptionLoader.loadRepeatMedicationData(
                        numberOfPrescriptions,
                        numOfRepeats,
                        numOfRepeats,
                        showDosage, showQuantity)

                mockingClient
                        .forTpp {
                            listRepeatMedication(currentPatient)
                                    .respondWithSuccess(tppMedicationDataMock)
                        }
            }
        }


    }

    private fun setupWiremockAndData(fromdate: OffsetDateTime, numPrescriptions: Int, numOfCourses: Int, numOfRepeats: Int) {

        //todo: John TPP
        when (currentGPSystem) {
            EMIS -> {
                emisPrescriptionsMock = EmisPrescriptionLoader.loadPrescriptionsData(numPrescriptions, numOfCourses, numOfRepeats)

                mockingClient
                        .forEmis {
                            prescriptionsRequest(currentPatient, fromdate, TO_DATE)
                                    .respondWithSuccess(emisPrescriptionsMock)
                        }
            }
            TPP -> {
                tppMedicationDataMock = TppPrescriptionLoader.loadRepeatMedicationData(numPrescriptions, numOfRepeats, numOfRepeats)

                mockingClient
                        .forTpp {
                            listRepeatMedication(currentPatient)
                                    .respondWithSuccess(tppMedicationDataMock)
                        }

            }
        }
    }

    private fun getResponseToExpectedPrescriptionFormat() : List<HistoricPrescription> {

        //todo: John TPP
        when (currentGPSystem) {
            EMIS -> {
               return EmisPrescriptionMapper.Map(emisPrescriptionsMock)
            }
            TPP -> {
                return TppPrescriptionMapper.Map(tppMedicationDataMock)
            }
        }
        return ArrayList<HistoricPrescription>()
    }

    private fun setupWiremockAndDataWithDelay(fromdate: OffsetDateTime, numPrescriptions: Int, numOfCourses: Int, numOfRepeats: Int) {

        val delay: Long = 31

        //todo: John TPP
        when (currentGPSystem) {
            EMIS -> {
                emisPrescriptionsMock = EmisPrescriptionLoader.loadPrescriptionsData(numPrescriptions, numOfCourses, numOfRepeats)

                mockingClient
                        .forEmis {
                            prescriptionsRequest(emisPatient, fromdate, TO_DATE)
                                    .respondWithSuccess(emisPrescriptionsMock).delayedBy(Duration.ofSeconds(delay))
                        }
            }
            TPP -> {
                tppMedicationDataMock = TppPrescriptionLoader.loadRepeatMedicationData(numPrescriptions, numPrescriptions, numOfRepeats)

                mockingClient
                        .forTpp {
                            listRepeatMedication(currentPatient)
                                    .respondWithSuccess(tppMedicationDataMock).delayedBy(Duration.ofSeconds(delay))
                        }
            }
        }
    }

    @And("^courses have status$")
    fun givenCoursesHaveStatus(statuses: DataTable) {

        var EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        var statusIndex = 0
        val data = statuses.raw()

        for (prescription in emisPrescriptionsMock.prescriptionRequests) {
            for (course in prescription.requestedMedicationCourses) {
                val status = data.get(statusIndex).get(0)
                course.requestedMedicationCourseStatus = RequestedMedicationCourseStatus.valueOf(status)
                statusIndex++
            }
        }

        mockingClient
                .forEmis {
                    prescriptionsRequest(MockDefaults.patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(emisPrescriptionsMock)
                }
    }
}
