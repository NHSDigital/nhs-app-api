package features.prescriptions.stepDefinitions

import constants.ErrorResponseCodeTpp
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
import utils.SerenityHelpers
import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.defaults.EmisMockDefaults
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.models.RequestedMedicationCourseStatus
import mocking.tpp.models.Error
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.defaults.VisionMockDefaults
import mocking.vision.models.PrescriptionHistory
import models.Patient
import models.prescriptions.HistoricPrescription
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionsListResponse
import java.time.Duration
import java.time.OffsetDateTime
import java.time.ZonedDateTime

private const val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY = 6L
private const val DATE_GREATER_THAN_SIX_MONTHS = 7L
private const val DELAY_IN_SECONDS = 31L
private const val NUM_OF_PRESCRIPTIONS = 10

@Suppress("LargeClass", "Do not duplicate this suppression in other classes, " +
        "if possible, break down steps into functional areas")
open class PrescriptionsStepDefinitions : BaseStepDefinition() {

    @Steps
    lateinit var prescriptions: PrescriptionsSteps

    private lateinit var headerNative: HeaderNative

    val mockingClient = MockingClient.instance

    private val fromDateKey = "FromDate"
    private val toDate = OffsetDateTime.now()
    private var numberOfPrescriptions: Int = 0
    private var numOfCourses: Int = 0
    private var numOfRepeats: Int = 0
    private var fromDate: String? = null

    lateinit var prescriptionLoader: IPrescriptionLoader<*>

    lateinit var prescriptionsListResponse: PrescriptionsListResponse
    lateinit var errorPage: ErrorPage

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps

    @Then("^I see prescriptions page loaded$")
    fun iSeePrescriptionsPageLoaded() {
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
        navigation.select(NavBarNative.NavBarType.PRESCRIPTIONS)
        prescriptions.isLoaded()
    }

    @Given("^I have no repeat prescriptions for (.*)$")
    fun givenIHaveNoRepeatPrescriptions(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        initialize(gpSystem)
        givenIHaveXPastRepeatPrescriptions(0)
        givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(0, 0)
    }

    @And("^each repeat prescription contains (\\d+) courses of which (\\d+) are repeats$")
    fun givenEachRepeatPrescriptionContainsXCoursesOfWhichXAreRepeats(numOfCourses: Int, numOfRepeats: Int) {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        this.numOfCourses = numOfCourses
        this.numOfRepeats = numOfRepeats

        setupWiremockAndData(expectedDefaultFromDate,
                numOfCourses * numberOfPrescriptions,
                numOfRepeats * numberOfPrescriptions)
    }

    @And("^each repeat prescription shares the same course")
    fun givenEachRepeatPrescriptionSharesTheSameCourse() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        numOfCourses = 1
        numOfRepeats = 1

        setupWiremockAndData(expectedDefaultFromDate)
    }

    @Given("From date is 6 months ago and I have 10 prescriptions in the last 6 months")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        numberOfPrescriptions = NUM_OF_PRESCRIPTIONS
        numOfRepeats = NUM_OF_PRESCRIPTIONS
        numOfCourses = NUM_OF_PRESCRIPTIONS

        setupWiremockAndData(expectedDefaultFromDate)
    }

    @When("I get the users prescriptions with a valid cookie")
    fun whenIGetTheUsersPrescriptionsWithAValidCookie() {
        val formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(fromDateKey)

        try {
            val sessionVariable = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
            prescriptionsListResponse = sessionVariable.prescriptions.getPrescriptionsConnection(
                    if (formattedFromDate != null) formattedFromDate.toString() else formattedFromDate)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("I receive a list of (\\d+) prescriptions")
    fun thenIReceiveAListOfTenPrescriptions(count: Int) {

        assertNotNull(prescriptionsListResponse)

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                assertEquals(count, prescriptionsListResponse.prescriptions.count())
                val prescriptions = prescriptionsListResponse.prescriptions

                // We had to use a string here and then parse the screen as
                // kotlin did not like the date time format sent from the worker
                for (int in 0 until prescriptions.count() - 2) {
                    assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate)!! >=
                            ZonedDateTime.parse(prescriptions[int + 1].orderDate))
                }
            }
            ProviderTypes.TPP -> {
                assertEquals(count, prescriptionsListResponse.courses.count())
            }
            ProviderTypes.VISION -> {
                assertEquals(count, prescriptionsListResponse.courses.count())
            }
            else -> {
                throw NotImplementedError("Invalid GP System")
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

    @But("^I do not request a fromDate$")
    fun iDoNotRequestAFromDate() {
        fromDate = null
    }

    @But("^the GP System is too slow$")
    fun butTheGPSystemIsTooSlow() {
        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)
        numberOfPrescriptions = 1
        numOfRepeats = 1
        numOfCourses = 1

        setupWiremockAndDataWithDelay(expectedDefaultFromDate)
    }

    @When("^I request prescriptions for the last 6 months$")
    fun iRequestPrescriptionsForTheLastSixMonths() {
        try {
            prescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getPrescriptionsConnection(fromDate)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @When("^I request prescriptions for the last 6 months with an invalid cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAnInvalidCookie() {
        try {
            prescriptionsListResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getPrescriptionsConnection(fromDate, WorkerClient.getHttpContext(true))
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I get a response with a list of prescriptions for the last 6 months$")
    fun iGetAResponseWithAListOfPrescriptionForTheLastSixMonths() {
        assertNotNull(prescriptionsListResponse)
        assertTrue(prescriptionsListResponse.prescriptions.isNotEmpty())
        assertTrue(prescriptionsListResponse.courses.isNotEmpty())
    }

    @But("^a fromDate in an unexpected format$")
    fun aFromDateInAnUnexpectedFormat() {
        fromDate = "13/66/99999999T"

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @But("^a fromDate in the future$")
    fun aFromDateInTheFuture() {
        fromDate = toDate.plusMonths(1).toString()

        givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    }

    @But("^a fromDate greater than 6 months ago$")
    fun aFromDateGreaterThanSixMonths() {
        fromDate = toDate.minusMonths(DATE_GREATER_THAN_SIX_MONTHS).toString()

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

    @But("^the GP System session has expired when viewing prescriptions$")
    fun theGPSystemSessionHasExpired() {
        if (currentProvider == null) {
            initialize(Serenity.sessionVariableCalled<String>(CommonSteps.GP_SYSTEM))
        }
        SerenityHelpers.setPatient(currentPatient)
        PrescriptionsFactory.getForSupplier(currentProvider.toString()).gpSessionHasExpired()
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
                                            Error(ErrorResponseCodeTpp.NO_ACCESS,
                                                    "Error Occurred",
                                                    "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                        }
            }
            ProviderTypes.VISION -> {
                mockingClient
                        .forVision {
                            authentication.getConfigurationRequest(
                                    VisionMockDefaults.visionUserSessionPrescriptionDisabled)
                                    .respondWithSuccess(VisionMockDefaults
                                            .visionConfigurationResponsePrescriptionsDisabled)
                        }
            }
            else -> {
                throw NotImplementedError("Invalid GP System")
            }
        }
    }

    @Then("I see a message informing me that I don't currently have access to this service")
    fun iSeeAMessageInformingMeThatIdontCurrentlyHaveAccessToThisService() {
        headerNative.waitForPageHeaderText("Repeat prescriptions unavailable")

        assertEquals("You are not currently able to order repeat prescriptions online", errorPage.heading.element.text)
        assertEquals("Contact your GP surgery for more information. " +
                "For urgent medical help, call 111.", errorPage.errorText1.element.text)
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

        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

        prescriptionLoader.loadData(
                numberOfPrescriptions,
                numOfRepeats,
                numOfRepeats,
                showDosage, showQuantity)

        when (currentProvider) {
            ProviderTypes.EMIS -> {

                mockingClient
                        .forEmis {
                            prescriptions.prescriptionsRequest(
                                    EmisMockDefaults.patientEmis,
                                    expectedDefaultFromDate,
                                    toDate)
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
                            prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
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
                            prescriptions.prescriptionsRequest(currentPatient, fromdate, toDate)
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
                            prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults
                                    .getVisionUserSession(VisionMockDefaults.patientVision))
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                        }
            }
        }
    }

    private fun getResponseToExpectedPrescriptionFormat(): List<HistoricPrescription> {

        return when (currentProvider) {
            ProviderTypes.EMIS -> EmisPrescriptionMapper.map(prescriptionLoader.data as PrescriptionRequestsGetResponse)
            ProviderTypes.TPP -> TppPrescriptionMapper.map(prescriptionLoader.data as ListRepeatMedicationReply)
            ProviderTypes.VISION -> VisionPrescriptionMapper.map(prescriptionLoader.data as PrescriptionHistory)
            else -> ArrayList()
        }
    }

    private fun setupWiremockAndDataWithDelay(fromdate: OffsetDateTime) {

        if (!::prescriptionLoader.isInitialized) {
            val gpSystem = Serenity.sessionVariableCalled<String>(CommonSteps.GP_SYSTEM)
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
                            prescriptions.prescriptionsRequest(currentPatient, fromdate, toDate)
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionRequestsGetResponse)
                                    .delayedBy(Duration.ofSeconds(DELAY_IN_SECONDS))
                        }
            }
            ProviderTypes.TPP -> {

                mockingClient
                        .forTpp {
                            prescriptions.listRepeatMedication(currentPatient)
                                    .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                                    .delayedBy(Duration.ofSeconds(DELAY_IN_SECONDS))
                        }
            }
            ProviderTypes.VISION -> {

                mockingClient
                        .forVision {
                            prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults
                                    .getVisionUserSession(VisionMockDefaults.patientVision))
                                    .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                                    .delayedBy(Duration.ofSeconds(DELAY_IN_SECONDS))
                        }
            }
        }
    }

    @And("^courses have status$")
    fun givenCoursesHaveStatus(statuses: DataTable) {

        val expectedDefaultFromDate = getDefaultPrescriptionsFromDate(toDate)

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
                    prescriptions.prescriptionsRequest(
                            EmisMockDefaults.patientEmis,
                            expectedDefaultFromDate, toDate)
                            .respondWithSuccess(prgr)
                }
    }

    private fun initialize(gpSystem: String) {
        currentProvider = ProviderTypes.valueOf(gpSystem)
        currentPatient = Patient.getDefault(gpSystem)

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                prescriptionLoader = EmisPrescriptionLoader
            }
            ProviderTypes.TPP -> {
                prescriptionLoader = TppPrescriptionLoader
            }
            ProviderTypes.VISION -> {
                prescriptionLoader = VisionPrescriptionLoader
            }
        }
    }
}
