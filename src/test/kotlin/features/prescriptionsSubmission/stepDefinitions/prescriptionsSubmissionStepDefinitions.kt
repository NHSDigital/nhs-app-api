package features.prescriptionsSubmission.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.stepDefinitions.coursesStepDefinitions
import features.courses.steps.ConfirmRepeatPrescriptionOrderSteps
import features.prescriptions.PrescriptionsData
import features.prescriptions.stepDefinitions.PrescriptionsStepDefinitions
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.defaults.MockDefaults.Companion.patient
import mocking.MockingClient
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.prescription.PrescriptionsPage
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration
import java.time.OffsetDateTime
import java.util.*

open class PrescriptionsSubmissionStepDefinitions {

    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest : PrescriptionSubmissionRequest? = null

    @Steps
    lateinit var coursesStepDefinitions: coursesStepDefinitions

    @Steps
    lateinit var confirmRepeatPrescriptionOrderSteps: ConfirmRepeatPrescriptionOrderSteps

    private val commonSteps : CommonSteps = CommonSteps()

    var perscriptionMap = mutableMapOf<String, PrescriptionRequestsGetResponse>()

    lateinit var scenarioTitle: String
    var currentScenarioState: String = Scenario.STARTED

    lateinit var prescriptionPage : PrescriptionsPage

    @Steps
    lateinit var prescriptionSteps: PrescriptionsSteps

    @Steps
    lateinit var prescriptionStepDefinitions: PrescriptionsStepDefinitions

    @Given("^I have an empty repeat prescription request")
    fun iHaveAnEmptyRepeatPrescriptionRequest()
    {
        commonSteps.givenIHaveLoggedInAndHaveAValidSessionCookie()

        prescriptionSubmissionRequest = null
    }

    @Given("^I have a repeat prescription request with (\\d+) courses")
    fun iHaveARepeatPrescriptionRequestWithXCourses(numOfCourses: Int)
    {
        commonSteps.givenIHaveLoggedInAndHaveAValidSessionCookie()

        var uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add(UUID.randomUUID().toString())
        }

        prescriptionSubmissionRequest = PrescriptionSubmissionRequest(uuids, "")
    }

    @And("^(\\d+) invalid courses")
    fun xInvalidCourses(numOfCourses: Int)
    {
        var uuids: MutableList<String> = mutableListOf()

        for (i in 0 until numOfCourses) {
            uuids.add("invalidCourse-$i")
        }

        prescriptionSubmissionRequest!!.courseIds.addAll(uuids)
    }

    @And("^EMIS responds with an error indicating an included course has already been ordered in the last 30 days when submitting the repeat prescription")
    fun emisRespondsWithErrorIndicatingAnIncludedCourseHasAlreadyBeenOrderedInTheLastDaysWhenSubmittingRepeatPrescription()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithAlreadyAPendingRequestInTheLast30Days() }
    }

    @And("^Emis responds with an error indicating a course is invalid")
    fun emisRespondsWithAnErrorIndicatingACourseIsInvalid()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithBadRequestErrorIndicatingACourseIsInvalid() }
    }

    @And("^EMIS responds with a Created success code when submitting the repeat prescription")
    fun emisRespondsWithACreatedSuccessCodeWhenSubmittingRepeatPrescription()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithCreated() }
    }

    @And("^EMIS responds with an error indicating prescriptions is not enabled when submitting the repeat prescription")
    fun emisRespondsWithAnErrorIndicatingPrescriptionsIsNotEnabled()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithPrescriptionsNotEnabled() }
    }

    @When("I submit the repeat prescription")
    fun whenISubmitTheRepeatPrescription()
    {
        try {
            val response = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postPrescriptionsConnection(prescriptionSubmissionRequest, null)

            Serenity.setSessionVariable(HTTP_RESPONSE).to(response)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @And("^EMIS takes longer than 30 seconds to respond when a repeat prescription is submitted")
    fun emisTakesTooLongToRespondWhenARepeatPrescriptionIsSubmitted()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithCreated().delayedBy(Duration.ofSeconds(31)) }
    }

    @And("EMIS responds with an unknown internal server error when a repeat prescription is submitted")
    fun emisRespondsWithAnUnknownInternalServerErrorWhenARepeatPrescriptionIsSubmitted()
    {
        mockingClient.forEmis { repeatPrescriptionSubmissionRequest(patient, prescriptionSubmissionRequest).respondWithGenericInternalServerError() }
    }

    @Given("I select (\\d+) repeatable prescriptions to order")
    fun iSelectXRepeatablePrescriptionsToOrder(amount: Int) {

        coursesStepDefinitions.iSelectXRepeatablePrescriptions(amount, amount)

        //redefine mock for scenario value...
        mockingClient.forEmis {
            coursesRequest(patient)
                    .respondWithSuccess(CourseRequestsGetResponse(coursesStepDefinitions.coursesData))
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }
        val submitted = "SUBMITTED"
        mockingClient.forEmis {
            repeatPrescriptionSubmissionRequest(patient)
                    .respondWithCreated()
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
                    .willSetStateTo(submitted)
        }

        currentScenarioState = submitted

        buildNewPrescriptionData(coursesStepDefinitions.coursesData)

        mockingClient.forEmis {
            prescriptionsRequest(patient)
                    .respondWithSuccess(perscriptionMap[currentScenarioState]!!)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }
    }

    private fun buildNewPrescriptionData(orderedCourses: MutableList<MedicationCourse>) {

        //1 create new prescription object and add ordered courses to it..
        val cr = mutableListOf<RequestedMedicationCourse>()
        orderedCourses.forEach {
            c -> cr.add(
                RequestedMedicationCourse(
                        c.medicationCourseGuid, RequestedMedicationCourseStatus.Requested))
        }
        val oldPrescriptions = perscriptionMap[Scenario.STARTED]
        val prs = mutableListOf<PrescriptionRequest>()
        prs.add(PrescriptionRequest(OffsetDateTime.now().toString(), cr))

        oldPrescriptions!!.prescriptionRequests.forEach {
            pr -> prs.add(pr)
        }

        //2. update course list
        val cs = mutableSetOf<MedicationCourse>()
        oldPrescriptions.medicationCourses.forEach {
            c -> cs.add(c)
        }

        orderedCourses.forEach {
            c -> cs.add(MedicationCourse(
                c.medicationCourseGuid,
                c.name,
                c.dosage,
                c.quantityRepresentation,
                c.prescriptionType,
                c.constituents,
                c.canBeRequested))
        }

        perscriptionMap[currentScenarioState] = PrescriptionRequestsGetResponse(prs, cs.toList())
    }

    @And("^the scenario is (.*)$")
    fun theScenarioIsX(title: String) {
        scenarioTitle = title
    }

    @And("^I have (\\d+) historic prescriptions in this scenario$")
    fun iHaveXHistoricPrescriptionsInThisScenario(amount: Int) {
        val pr = PrescriptionsData.loadPrescriptionsData(amount, amount, amount)
        mockingClient.forEmis {
            prescriptionsRequest(patient)
                    .respondWithSuccess(pr)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }

        perscriptionMap[Scenario.STARTED] = pr

    }

    @When("I click Confirm and order repeat prescription")
    fun iClickConfirmAndOrderRepeatPrescription() {

        confirmRepeatPrescriptionOrderSteps.confirmRepeatPrescriptionsOrderPage.clickConfirmAndOrderRepeatPrescriptionButton()

    }


    @Then("I see a order successful message on the Repeat prescription page with (\\d+) prescriptions")
    fun iSeeAOrderSuccessfulMessageOnTheRequestPrescriptionPageWithXPrescriptions(amount: Int) {

        Assert.assertTrue(prescriptionPage.isOrdeSuccessfulTextVisible())

        prescriptionSteps.assertPrescriptionsMatch(prescriptionStepDefinitions.getExpectedNumPrescriptions(
                perscriptionMap[currentScenarioState]!!), amount)
    }


}
