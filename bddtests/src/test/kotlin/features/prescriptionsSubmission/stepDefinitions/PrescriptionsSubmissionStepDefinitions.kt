package features.prescriptionsSubmission.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.And
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
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.vision.VisionMockDefaults
import mocking.vision.models.PrescriptionHistory
import models.Patient
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert.assertTrue
import pages.prescription.PrescriptionsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration
import java.util.*

open class PrescriptionsSubmissionStepDefinitions : BaseStepDefinition() {

    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest: PrescriptionSubmissionRequest? = null
    lateinit var prescriptionLoader: IPrescriptionLoader<*>

    @Steps
    lateinit var coursesStepDefinitions: CoursesStepDefinitions

    @Steps
    lateinit var confirmRepeatPrescriptionOrderSteps: ConfirmRepeatPrescriptionOrderSteps

    lateinit var scenarioTitle: String
    var currentScenarioState: String = Scenario.STARTED

    lateinit var prescriptionPage: PrescriptionsPage

    @Steps
    lateinit var prescriptionSteps: PrescriptionsSteps

    private val EMIS = "EMIS"
    private val TPP = "TPP"

    private var initialHistoricPrescriptionsCount = 0

    val StatusSubmitted = "Submitted"

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

    @And("^(\\d+) invalid courses")
    fun xInvalidCourses(numOfCourses: Int) {
        val uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add("invalidCourse-$i")
        }

        prescriptionSubmissionRequest!!.courseIds.addAll(uuids)
    }

    @And("^EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription")
    fun emisRespondsWithErrorIndicatingAnIncludedCourseHasAlreadyBeenOrderedInTheLastDaysWhenSubmittingRepeatPrescription() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithAlreadyAPendingRequestInTheLast30Days()
        }
    }

    @And("^Emis responds with an error indicating a course is invalid")
    fun emisRespondsWithAnErrorIndicatingACourseIsInvalid() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithBadRequestErrorIndicatingACourseIsInvalid()
        }
    }

    @And("^EMIS responds with a Created success code when submitting the repeat prescription")
    fun emisRespondsWithACreatedSuccessCodeWhenSubmittingRepeatPrescription() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated()
        }
    }

    @And("^EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat prescription")
    fun emisRespondsWithAnErrorIndicatingPrescriptionsIsNotEnabled() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithPrescriptionsNotEnabled()
        }
    }

    @When("I submit the repeat prescription")
    fun whenISubmitTheRepeatPrescription() {
        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.postPrescriptionsConnection(prescriptionSubmissionRequest)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @And("^EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted")
    fun emisTakesTooLongToRespondWhenARepeatPrescriptionIsSubmitted() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithCreated().delayedBy(Duration.ofSeconds(31))
        }
    }

    @And("EMIS responds with an unknown internal server error when a repeat prescription is submitted")
    fun emisRespondsWithAnUnknownInternalServerErrorWhenARepeatPrescriptionIsSubmitted() {
        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(currentPatient, prescriptionSubmissionRequest)
                    .respondWithGenericInternalServerError()
        }
    }

    @Given("I select (\\d+) (.*) repeatable prescriptions to order")
    fun iSelectXRepeatablePrescriptionsToOrder(amount: Int, gpSystem: String) {
        prescriptionSubmissionWireMockAndDataSetup(amount, gpSystem)
    }

    @And("^the scenario is (.*)$")
    fun theScenarioIsX(title: String) {
        scenarioTitle = title
    }


    @Given("^I am using (.*) GP System to submit my prescription$")
    fun givenIHaveXPastRepeatPrescriptions(gpSystem: String) {
        initialize(gpSystem)
    }

    private fun initialize(gpSystem: String) {
        currentProvider = ProviderTypes.valueOf(gpSystem)
        currentPatient = Patient.getDefault(gpSystem)
        prescriptionLoader = PrescriptionsFactory.getForSupplier(gpSystem).getPrescriptionsLoader
        val emisPrescriptionMap = mutableMapOf<String, PrescriptionRequestsGetResponse>()
        Serenity.setSessionVariable("EmisPrescriptionsMap").to(emisPrescriptionMap)
    }

    @And("^I have (\\d+) historic prescriptions in this scenario$")
    fun iHaveXHistoricPrescriptionsInThisScenario(amount: Int) {

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

                val map = Serenity.sessionVariableCalled<MutableMap<String, PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")
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
                    getPrescriptionHistoryRequest(VisionMockDefaults.visionUserSession)
                            .respondWithSuccess(prescriptionLoader.data as PrescriptionHistory)
                }
            }
        }
    }

    @When("I click Confirm and order repeat prescription")
    fun iClickConfirmAndOrderRepeatPrescription() {
        confirmRepeatPrescriptionOrderSteps.confirmRepeatPrescriptionsOrderPage.clickConfirmAndOrderRepeatPrescriptionButton()
    }


    @Then("I see a order successful message on the Repeat prescription page with (\\d+) prescriptions")
    fun iSeeAOrderSuccessfulMessageOnTheRequestPrescriptionPageWithXPrescriptions(amount: Int) {
        assertTrue(prescriptionPage.isOrderSuccessfullTextVisible())

        when (currentProvider) {
            ProviderTypes.TPP -> {
                prescriptionSteps.assertPrescriptionsMatch(
                        TppPrescriptionMapper.Map(prescriptionLoader.data as ListRepeatMedicationReply),
                        amount,
                        false)
            }
            ProviderTypes.EMIS -> {
                val map = Serenity.sessionVariableCalled<MutableMap<String, PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")

                prescriptionSteps.assertPrescriptionsMatch(EmisPrescriptionMapper.Map(
                        map[currentScenarioState]!!), amount)
            }
            ProviderTypes.VISION -> {
                prescriptionSteps.assertPrescriptionsMatch(VisionPrescriptionMapper.map(prescriptionLoader.data as PrescriptionHistory), amount)
            }
        }
    }

    private fun prescriptionSubmissionWireMockAndDataSetup(amount: Int, gpSystem: String) {
        coursesStepDefinitions.iSelectXRepeatablePrescriptions(amount, gpSystem, amount)
        currentScenarioState = PrescriptionsFactory.getForSupplier(gpSystem)
                .setupWireMockAndDataSetup(
                        scenarioTitle,
                        currentScenarioState,
                        StatusSubmitted,
                        initialHistoricPrescriptionsCount,
                        amount)
    }
}