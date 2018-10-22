package features.prescriptions.stepDefinitions

import cucumber.api.DataTable
import cucumber.api.java.en.And
import cucumber.api.java.en.But
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.mappers.VisionPrescriptionMapper
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.BaseStepDefinition
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import features.sharedStepDefinitions.backend.CommonSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import features.sharedSteps.SerenityHelpers
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.patient
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourseStatus
import mocking.tpp.models.Error
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.vision.models.PrescriptionHistory
import models.Patient
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.PrescriptionsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionsListResponse
import java.time.Duration
import java.time.OffsetDateTime
import java.time.ZonedDateTime
import java.util.*
import javax.servlet.http.Cookie

open class PrescriptionsStepDefinitions : BaseStepDefinition() {

    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    val FROM_DATE = "FromDate"
    val TO_DATE = OffsetDateTime.now()
    val HTTP_EXCEPTION = "HttpException"
    val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY: Long = 6
    var numberOfPrescriptions: Int = 0
    var numOfCourses: Int = 0
    var numOfRepeats: Int = 0
    var fromDate: String? = null


    lateinit var prescriptionLoader: IPrescriptionLoader<*>


    lateinit var PrescriptionsListResponse: PrescriptionsListResponse

    lateinit var prescriptionsPage: PrescriptionsPage
    lateinit var confirmRepeatPrescriptionsOrderPage: ConfirmRepeatPrescriptionsOrderPage
    lateinit var errorPage: ErrorPage

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
        login.using(currentPatient)
        navigation.select("Prescriptions")
    }

    @Given("^I have no repeat prescriptions for (.*)$")
    fun givenIHaveNoRepeatPrescriptions(gpSystem:String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        initialize(gpSystem)
        givenIHaveXPastRepeatPrescriptions(0)
        givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(0, 0)
    }

    @And("^each repeat prescription contains (\\d+) courses of which (\\d+) are repeats$")
    fun givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(numOfCourses: Int, numOfRepeats: Int) {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)
        this.numOfCourses = numOfCourses
        this.numOfRepeats = numOfRepeats

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE,
                numOfCourses * numberOfPrescriptions,
                numOfRepeats * numberOfPrescriptions)
    }

    @And("^each repeat prescription shares the same course")
    fun givenEachRepeatPrescriptionSharesTheSameCourse() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)
        numOfCourses = 1
        numOfRepeats = 1

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE)
    }

    @Given("From date is 6 months ago and I have 10 prescriptions in the last 6 months")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)
        numberOfPrescriptions = 10
        numOfRepeats = 10
        numOfCourses = 10

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE)
    }

    @When("I get the users prescriptions with a valid cookie")
    fun whenIGetTheUsersPrescriptionsWithAValidCookie() {
        val formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(FROM_DATE)

        try {
            val sessionVariable = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            PrescriptionsListResponse = sessionVariable.prescriptions.getPrescriptionsConnection(
                    if (formattedFromDate != null) formattedFromDate.toString() else formattedFromDate)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive a list of (\\d+) prescriptions")
    fun thenIReceiveAListOfTenPrescriptions(count: Int) {

        assertNotNull(PrescriptionsListResponse)

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                assertEquals(count, PrescriptionsListResponse.prescriptions.count())
                val prescriptions = PrescriptionsListResponse.prescriptions

                // We had to use a string here and then parse the screen as kotlin did not like the date time format sent from the worker
                for (int in 0 until prescriptions.count() - 2) {
                    assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate)!! >= ZonedDateTime.parse(prescriptions[int + 1].orderDate))
                }
            }
            ProviderTypes.TPP -> {
                assertEquals(count, PrescriptionsListResponse.courses.count())
            }
            ProviderTypes.VISION -> {
                assertEquals(count, PrescriptionsListResponse.courses.count())
            }
            else -> {
                throw Exception("Invalid GP System")
            }
        }
    }

    @Given("^I am using (.*) GP System$")
    fun givenIHaveXPastRepeatPrescriptions(gpSystem: String) {
        initialize(gpSystem)
    }

    @Given("^I have (\\d+) past repeat prescriptions$")
    fun givenIHaveXPastRepeatPrescriptions(numPrescriptions: Int) {
        numberOfPrescriptions = numPrescriptions
    }

    @Then("^I see expected prescriptions$")
    fun thenISeeXPrescriptions() {
        val expectedPrescriptions = getResponseToExpectedPrescriptionFormat()

        prescriptions.assertPrescriptionsMatch(expectedPrescriptions, expectedPrescriptions.size, true)
    }

    @Then("^I see (\\d+) prescriptions$")
    fun thenISeeXPrescriptions(numPrescriptions: Int) {

        prescriptions
                .assertPrescriptionsMatch(
                        getResponseToExpectedPrescriptionFormat(),
                        numPrescriptions,
                        providerHasAllPrescriptionFields())
    }

    private fun providerHasAllPrescriptionFields(): Boolean {
        return currentProvider == ProviderTypes.EMIS || currentProvider == ProviderTypes.VISION
    }

    @And("^the patient has no prescriptions in the last 6 months")
    fun thePatientHasNoPrescriptionsInTheLastSixMonths() {

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)
        numberOfPrescriptions = 0
        numOfRepeats = 0
        numOfCourses = 0

        setupWiremockAndData(EXPECTED_DEFAULT_FROM_DATE)
    }

    @But("^I do not request a fromDate$")
    fun iDoNotRequestAFromDate() {
        fromDate = null
    }

    @But("^the GP System is too slow$")
    fun butTheGPSystemIsTooSlow() {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)
        numberOfPrescriptions = 1
        numOfRepeats = 1
        numOfCourses = 1

        setupWiremockAndDataWithDelay(EXPECTED_DEFAULT_FROM_DATE)
    }

    @When("^I request prescriptions for the last 6 months$")
    fun iRequestPrescriptionsForTheLastSixMonths() {
        try {
            PrescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getPrescriptionsConnection(fromDate)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I request prescriptions for the last 6 months with an invalid cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAnInvalidCookie() {
        try {
            PrescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getPrescriptionsConnection(fromDate, WorkerClient.getHttpContext(true))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I request prescriptions for the last 6 months with a missing cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAMissingCookie() {
        Serenity.setSessionVariable(Cookie::class).to(null)

        try {
            PrescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getPrescriptionsConnection(fromDate, WorkerClient.getHttpContext(true))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I get a response with a list of prescriptions for the last 6 months$")
    fun iGetAResponseWithAListOfPrescriptionForTheLastSixMonths() {
        assertNotNull(PrescriptionsListResponse)
        assertTrue(PrescriptionsListResponse.prescriptions.isNotEmpty())
        assertTrue(PrescriptionsListResponse.courses.isNotEmpty())
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

    @But("^the GP System has disabled prescriptions$")
    fun theGPSystemHasDisabledPrescriptions() {
        if (currentProvider == null) {
            initialize(Serenity.sessionVariableCalled<String>(CommonSteps.GP_SYSTEM))
        }
        SerenityHelpers.setPatient(currentPatient)
        PrescriptionsFactory.getForSupplier(currentProvider.toString()).disableAtGPLevel()
    }

    fun getDefaultPrescriptionsFromDate(dateNow: OffsetDateTime): OffsetDateTime {
        return dateNow.minusMonths(PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY)
    }

    @Given("prescriptions is disabled at a GP Practice level")
    fun prescriptionsIsDisabledAtAGPLevel() {
        when (currentProvider) {
            ProviderTypes.EMIS -> {
                mockingClient
                        .forEmis {
                            prescriptions.prescriptionsRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                        }

                mockingClient
                        .forEmis {
                            prescriptions.coursesRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                        }
            }
            ProviderTypes.TPP -> {
                mockingClient
                        .forTpp {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithError(
                                            Error("6",
                                                    "Error Occurred",
                                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                        }
            }
            ProviderTypes.VISION -> {
                mockingClient
                        .forVision {
                            getConfigurationRequest(
                                    MockDefaults.visionUserSessionPrescriptionDisabled)
                                    .respondWithSuccess(MockDefaults.visionConfigurationResponsePrescriptionsDisabled)
                        }
            }
            else -> {
                throw Exception("Invalid GP System")
            }
        }
    }

    @Then("I see a message informing me that I don't currently have access to this service")
    fun iSeeAMessageInformingMeThatIdontCurrentlyHaveAccessToThisService() {
        assertEquals("You are not currently able to order repeat prescriptions online", errorPage.heading.element.text)
        assertEquals("Contact your GP surgery for more information. For urgent medical help, call 111.", errorPage.errorText1.element.text)
    }

    @But("The prescriptions endpoint is timing out")
    fun butThePrescriptionsEndpointIsTimingOut() {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(currentPatient)
                            .respondWith(504, resolve = {}, milliSecondDelay = 15000)
                }
    }


    @But("The prescriptions endpoint is throwing a server error")
    fun butThePrescriptionsEndpointIsThrowingAServerError() {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(currentPatient)
                            .respondWith(500, resolve = {})
                }
    }

    @But("The courses endpoint is timing out")
    fun butTheCoursesEndpointIsTimingOut() {
        mockingClient.forEmis {
            prescriptions.coursesRequest(currentPatient)
                    .respondWith(504, resolve = {}, milliSecondDelay = 15000)
        }
    }

    @But("The courses endpoint is throwing a server error")
    fun butTheCoursesEndpointIsThrowingAServerError() {
        mockingClient.forEmis {
            prescriptions.coursesRequest(currentPatient)
                    .respondWith(500, resolve = {})
        }
    }

    @But("The prescription submission endpoint is timing out")
    fun butThePrescriptionSubmissionEndpointIsTimingOut() {
        mockingClient.forEmis { prescriptions.repeatPrescriptionSubmissionRequest(MockDefaults.patient).respondWith(504, resolve = {}, milliSecondDelay = 15000) }

        mockingClient.forTpp { prescriptions.prescriptionSubmission(MockDefaults.patientTpp, null).respondWith(200, resolve = {}, milliSecondDelay = 15000) }
    }

    @But("The prescription submission endpoint is throwing a server error")
    fun butThePrescriptionSubmissionEndpointIsThrowingAServerError() {
        mockingClient.forEmis { prescriptions.repeatPrescriptionSubmissionRequest(MockDefaults.patient).respondWith(500, resolve = {}) }
    }

    @But("The prescription submission endpoint is throwing an already ordered exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnAlreadyOrderedException() {
        mockingClient.forTpp { prescriptions.prescriptionSubmission(MockDefaults.patientTpp, null).respondWithError(Error("1", "One of the medications requested is no longer available", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a")) }
    }

    @But("The prescription submission endpoint is throwing an invalid guid exception")
    fun butThePrescriptionSubmissionEndpointIsThrowingAnInvalidGuidException() {
        mockingClient.forTpp { prescriptions.prescriptionSubmission(MockDefaults.patientTpp, null).respondWithError(Error("1", "There was an error processing your request", "1f907c07-9063-4d3a-81d7-ee8c98c54f4a")) }
    }

    @Then("I see the appropriate error message for a prescription timeout")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionTimeout() {

        val pageHeader = prescriptionsPage.timeoutPageHeader
        val header = prescriptionsPage.timeoutHeader
        val message = prescriptionsPage.timeoutMessage
        val retryButtonText = prescriptionsPage.timeoutRetryButtonText

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertPageHeader(pageHeader)
    }

    @Then("I see the appropriate error message for a prescription server error")
    fun thenISeeTheAppropriateErrorMessageForAPrescriptionServerError() {

        val pageHeader = prescriptionsPage.serverErrorPageHeader
        val header = prescriptionsPage.serverErrorHeader
        val message = prescriptionsPage.serverErrorMessage

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertNoRetryButton()
                .assertPageHeader(pageHeader)
    }

    @Then("I see the appropriate error message for a course request error")
    fun thenISeeTheAppropriateErrorMessageForACourseRequestError() {
        
        val pageHeader = confirmRepeatPrescriptionsOrderPage.serverErrorPageHeader
        val header = confirmRepeatPrescriptionsOrderPage.serverErrorHeader
        val message = confirmRepeatPrescriptionsOrderPage.serverErrorMessage
        val retryButtonText = confirmRepeatPrescriptionsOrderPage.serverErrorRetryButtonText

        errorPage.assertHeaderText(header)
                .assertMessageText(message)
                .assertRetryButtonText(retryButtonText)
                .assertPageHeader(pageHeader)
    }

    @Then("I select (\\d+) prescription to order")
    fun iSelectXPrescriptionsToOrder(prescriptionToOrder: Int) {
        prescriptions.selectSubscriptionsToOrder(prescriptionToOrder)
        prescriptions.clickContinue()
        prescriptions.clickConfirmAndOrderRepeat()
    }

    @And("each course has (.*)")
    fun eachCourseHasX(contents: String) {
        val showDosage = contents.toLowerCase().contains("dosage")
        val showQuantity = contents.toLowerCase().contains("quantity")

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        prescriptionLoader.loadData(
                numberOfPrescriptions,
                numOfRepeats,
                numOfRepeats,
                showDosage, showQuantity)

        when (currentProvider) {
            ProviderTypes.EMIS -> {

                mockingClient
                        .forEmis {
                            prescriptions.prescriptionsRequest(patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionRequestsGetResponse)
                        }
            }
            ProviderTypes.TPP -> {

                mockingClient
                        .forTpp {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                        }
            }
            ProviderTypes.VISION -> {

                mockingClient
                        .forVision {
                            getPrescriptionHistoryRequest(MockDefaults.visionUserSession)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                        }
            }
        }


    }

    private fun setupWiremockAndData(fromdate: OffsetDateTime,
                                     numOfCourses: Int = this.numOfCourses,
                                     numOfRepeats: Int = this.numOfRepeats) {

        if (!::prescriptionLoader.isInitialized) {
            val gpSystem = Serenity.sessionVariableCalled<String>(CommonSteps.GP_SYSTEM)
            initialize(gpSystem)
        }

        prescriptionLoader.loadData(
                numberOfPrescriptions,
                numOfCourses,
                numOfRepeats)

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                mockingClient
                        .forEmis {
                            prescriptions.prescriptionsRequest(currentPatient, fromdate, TO_DATE)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionRequestsGetResponse)
                        }
            }
            ProviderTypes.TPP -> {
                mockingClient
                        .forTpp {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                        }

            }
            ProviderTypes.VISION -> {
                mockingClient
                        .forVision {
                            getPrescriptionHistoryRequest(MockDefaults.visionUserSession)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                        }
            }
        }
    }

    private fun getResponseToExpectedPrescriptionFormat(): List<HistoricPrescription> {

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                return EmisPrescriptionMapper.Map(prescriptionLoader.data as PrescriptionRequestsGetResponse)
            }
            ProviderTypes.TPP -> {
                return TppPrescriptionMapper.Map(prescriptionLoader.data as ListRepeatMedicationReply)
            }
            ProviderTypes.VISION -> {
                return VisionPrescriptionMapper.map(prescriptionLoader.data as PrescriptionHistory)
            }
        }
        return ArrayList()
    }

    private fun setupWiremockAndDataWithDelay(fromdate: OffsetDateTime) { //}, numPrescriptions: Int, numOfCourses: Int, numOfRepeats: Int) {

        val delay: Long = 31

        if (!::prescriptionLoader.isInitialized) {
            var gpSystem = Serenity.sessionVariableCalled<String>(CommonSteps.GP_SYSTEM)
            initialize(gpSystem)
        }


        prescriptionLoader.loadData(
                numberOfPrescriptions,
                numOfRepeats,
                numOfRepeats)

        when (currentProvider) {
            ProviderTypes.EMIS -> {

                mockingClient
                        .forEmis {
                            prescriptions.prescriptionsRequest(currentPatient, fromdate, TO_DATE)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionRequestsGetResponse).delayedBy(Duration.ofSeconds(delay))
                        }
            }
            ProviderTypes.TPP -> {

                mockingClient
                        .forTpp {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply).delayedBy(Duration.ofSeconds(delay))
                        }
            }
            ProviderTypes.VISION -> {

                mockingClient
                        .forVision {
                            getPrescriptionHistoryRequest(MockDefaults.visionUserSession)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory).delayedBy(Duration.ofSeconds(delay))
                        }
            }
        }
    }

    @And("^courses have status$")
    fun givenCoursesHaveStatus(statuses: DataTable) {

        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        var statusIndex = 0
        val data = statuses.raw()

        val prgr = prescriptionLoader.data as PrescriptionRequestsGetResponse

        for (prescription in prgr.prescriptionRequests) {
            for (course in prescription.requestedMedicationCourses) {
                val status = data.get(statusIndex).get(0)
                course.requestedMedicationCourseStatus = RequestedMedicationCourseStatus.valueOf(status)
                statusIndex++
            }
        }

        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(MockDefaults.patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prgr)
                }
    }

    private fun initialize(gpSystem: String) {
        currentProvider = ProviderTypes.valueOf(gpSystem)

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                currentPatient = EMIS_PATIENT
                prescriptionLoader = EmisPrescriptionLoader
            }
            ProviderTypes.TPP -> {
                currentPatient = TPP_PATIENT
                prescriptionLoader = TppPrescriptionLoader
            }
            ProviderTypes.VISION -> {
                currentPatient = VISION_PATIENT
                prescriptionLoader = VisionPrescriptionLoader
            }
        }
    }
}
