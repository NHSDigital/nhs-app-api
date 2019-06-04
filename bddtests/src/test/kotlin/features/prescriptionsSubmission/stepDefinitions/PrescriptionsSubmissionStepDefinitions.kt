package features.prescriptionsSubmission.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.stepDefinitions.CoursesStepDefinitions
import features.courses.steps.ConfirmRepeatPrescriptionOrderSteps
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.mappers.EmisPrescriptionMapper
import features.prescriptions.mappers.TppPrescriptionMapper
import features.prescriptions.mappers.VisionPrescriptionMapper
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.BaseStepDefinition
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.defaults.VisionMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.stubs.pds.ViewSpinePdsStubs
import mocking.vision.models.PrescriptionHistory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import pages.prescription.PrescriptionsPage
import utils.SerenityHelpers
import utils.assertTrueWithRetry
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration
import java.util.*

private const val WAIT_TIME_GREATER_THAN_THIRTY_SECS = 31L

open class PrescriptionsSubmissionStepDefinitions : BaseStepDefinition() {

    @Steps
    lateinit var confirmRepeatPrescriptionOrderSteps: ConfirmRepeatPrescriptionOrderSteps
    @Steps
    lateinit var coursesStepDefinitions: CoursesStepDefinitions
    @Steps
    lateinit var prescriptionSteps: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest: PrescriptionSubmissionRequest? = null

    lateinit var prescriptionLoader: IPrescriptionLoader<*>

    lateinit var scenarioTitle: String

    var currentScenarioState: String = Scenario.STARTED

    lateinit var prescriptionPage: PrescriptionsPage

    private var initialHistoricPrescriptionsCount = 0

    val statusSubmitted = "Submitted"

    @Given("^I have an empty repeat prescription request")
    fun iHaveAnEmptyRepeatPrescriptionRequest() {
        prescriptionSubmissionRequest = null
    }

    @Given("^I have a repeat prescription request with (\\d+) courses")
    fun iHaveARepeatPrescriptionRequestWithXCourses(numOfCourses: Int) {
        currentPatient = Patient.getDefault("EMIS")

        val uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add(UUID.randomUUID().toString())
        }

        prescriptionSubmissionRequest = PrescriptionSubmissionRequest(uuids, "")
    }

    @Given("^(\\d+) invalid courses")
    fun xInvalidCourses(numOfCourses: Int) {
        val uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add("invalidCourse-$i")
        }

        prescriptionSubmissionRequest!!.courseIds.addAll(uuids)
    }

    @Given("^EMIS responds with an error indicating an included course has already been ordered in the last 30 days " +
            "when submitting the repeat prescription")
    fun emisRespondsWithErrorIndicatingAnIncludedCourseHasAlreadyBeenOrderedWhenSubmittingRepeatPrescription() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithAlreadyAPendingRequestInTheLast30Days()
        }
    }

    @Given("^Emis responds with an error indicating a course is invalid")
    fun emisRespondsWithAnErrorIndicatingACourseIsInvalid() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithBadRequestErrorIndicatingACourseIsInvalid()
        }
    }

    @Given("^EMIS responds with a Created success code when submitting the repeat prescription")
    fun emisRespondsWithACreatedSuccessCodeWhenSubmittingRepeatPrescription() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated()
        }
    }

    @Given("^EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat " +
            "prescription")
    fun emisRespondsWithAnErrorIndicatingPrescriptionsIsNotEnabled() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithPrescriptionsNotEnabled()
        }
    }

    @Given("^EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted")
    fun emisTakesTooLongToRespondWhenARepeatPrescriptionIsSubmitted() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated().delayedBy(Duration.ofSeconds(WAIT_TIME_GREATER_THAN_THIRTY_SECS))
        }
    }

    @Given("I select (\\d+) repeatable prescriptions to order")
    fun iSelectXRepeatablePrescriptionsToOrder(amount: Int) {
        prescriptionSubmissionWireMockAndDataSetup(amount, SerenityHelpers.getGpSupplier())
    }

    private fun prescriptionSubmissionWireMockAndDataSetup(amount: Int, gpSystem: String) {
        coursesStepDefinitions.thereAreXXRepeatablePrescriptionsAvailable(amount)
        coursesStepDefinitions.iSelectXRepeatablePrescriptions(amount)
        currentScenarioState = PrescriptionsFactory.getForSupplier(gpSystem)
                .setupWireMockAndDataSetup(
                        scenarioTitle,
                        currentScenarioState,
                        statusSubmitted,
                        initialHistoricPrescriptionsCount,
                        amount)

        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(null
            ).respondWithSuccess(NhsAzureSearchData.getOrganisationWithinRange(true))
        }
    }

        @Given("^the scenario is (.*)$")
        fun theScenarioIsX(title: String) {
            scenarioTitle = title
        }


        @Given("^I am using (.*) GP System to submit my prescription$")
        fun givenIHaveXPastRepeatPrescriptions(gpSystem: String) {
            SerenityHelpers.setGpSupplier(gpSystem)
            currentPatient = Patient.getDefault(gpSystem)
            SerenityHelpers.setPatient(currentPatient)
            CitizenIdSessionCreateJourney(mockingClient).createFor(currentPatient)
            SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(currentPatient)
            currentProvider = ProviderTypes.valueOf(gpSystem)
            currentPatient = Patient.getDefault(gpSystem)
            prescriptionLoader = PrescriptionsFactory.getForSupplier(gpSystem).getPrescriptionsLoader
            val emisPrescriptionMap = mutableMapOf<String, PrescriptionRequestsGetResponse>()
            Serenity.setSessionVariable("EmisPrescriptionsMap").to(emisPrescriptionMap)
        }

        @Given("^I have (\\d+) historic prescriptions in this scenario$")
        fun iHaveXHistoricPrescriptionsInThisScenario(amount: Int) {
            ViewSpinePdsStubs(mockingClient).generateSpineStubs()
            initialHistoricPrescriptionsCount = amount
            prescriptionLoader.loadData(amount, amount, amount)

            when (currentProvider) {
                ProviderTypes.EMIS -> {
                    val data = prescriptionLoader.data as PrescriptionRequestsGetResponse
                    mockingClient.forEmis {
                        prescriptions.prescriptionsRequest(currentPatient)
                                .respondWithSuccess(data)
                                .inScenario(scenarioTitle)
                                .whenScenarioStateIs(currentScenarioState)
                    }

                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")
                    map[Scenario.STARTED] = data
                }
                ProviderTypes.TPP -> {
                    mockingClient.forTpp {
                        prescriptions.listRepeatMedication(currentPatient)
                                .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                    }
                }
                ProviderTypes.VISION -> {
                    mockingClient.forVision {
                        prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
                                .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                    }
                }
            }
        }

        @When("I submit the repeat prescription")
        fun whenISubmitTheRepeatPrescription() {
            try {
                val response = Serenity
                        .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                        .prescriptions.postPrescriptionsConnection(prescriptionSubmissionRequest)
                SerenityHelpers.setHttpResponse(response)
            } catch (httpException: NhsoHttpException) {
                SerenityHelpers.setHttpException(httpException)
            }
        }

        @When("I click Confirm and order repeat prescription")
        fun iClickConfirmAndOrderRepeatPrescription() {
            confirmRepeatPrescriptionOrderSteps.confirmRepeatPrescriptionsOrderPage
                    .clickConfirmAndOrderRepeatPrescriptionButton()
        }

        @Then("EMIS responds with an unknown internal server error when a repeat prescription is submitted")
        fun emisRespondsWithAnUnknownInternalServerErrorWhenARepeatPrescriptionIsSubmitted() {
            mockingClient.forEmis {
                prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                        .respondWithGenericInternalServerError()
            }
        }


        @Then("I see a order successful message on the Repeat prescription page with (\\d+) prescriptions")
        fun iSeeAOrderSuccessfulMessageOnTheRequestPrescriptionPageWithXPrescriptions(amount: Int) {
            assertTrueWithRetry(prescriptionPage.isOrderSuccessfullTextVisible(),
                    "Expected order success text to be visible")

            when (currentProvider) {
                ProviderTypes.TPP -> {
                    prescriptionSteps.assertPrescriptionsMatch(
                            TppPrescriptionMapper.map(prescriptionLoader.data as ListRepeatMedicationReply),
                            amount,
                            false)
                }
                ProviderTypes.EMIS -> {
                    val map =
                            Serenity.sessionVariableCalled<MutableMap<String,
                                    PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")

                    prescriptionSteps.assertPrescriptionsMatch(EmisPrescriptionMapper.map(
                            map[currentScenarioState]!!), amount)
                }
                ProviderTypes.VISION -> {
                    prescriptionSteps.assertPrescriptionsMatch(VisionPrescriptionMapper
                            .map(prescriptionLoader.data as PrescriptionHistory), amount)
                }
            }
        }

        @Then("I see a message indicating there was an error sending my order")
        fun iSeeAMessageOrderNotSuccessful() {
            confirmRepeatPrescriptionOrderSteps.assertErrorSendingOrderShown()
        }

        @Then("I see a message indicating I've previously ordered " +
                "one of the selected medications within the last 30 days")
        fun iSeeAMessageIndicatingIvePreviouslyOrderedOneOfTheSelectedMedicationsWithinTheLast30days() {
            confirmRepeatPrescriptionOrderSteps.assertMedicationOrderedWithinTheLast30DaysErrorShown()
        }
    }
