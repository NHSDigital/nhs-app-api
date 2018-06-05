package features.prescriptionsSubmission.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.sharedStepDefinitions.backend.CommonSteps
import mocking.MockDefaults.Companion.patient
import mocking.MockingClient
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptionsSubmission.PrescriptionSubmissionRequest
import java.time.Duration
import java.util.*

open class PrescriptionsSubmissionStepDefinitions {

    val HTTP_EXCEPTION = "HttpException"
    val HTTP_RESPONSE = "HttpResponse"

    val mockingClient = MockingClient.instance

    var prescriptionSubmissionRequest : PrescriptionSubmissionRequest? = null

    private val commonSteps : CommonSteps = CommonSteps()

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
}
