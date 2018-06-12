package features.courses.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.courses.CoursesData
import features.courses.steps.CourseSteps
import features.prescriptions.steps.PrescriptionsSteps
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.emis.models.*
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.courses.CourseListResponse


open class coursesStepDefinitions {

    val EXPECTED_COURSE_DATA_SIZE = "CourseDataSize"
    val HTTP_EXCEPTION = "HttpException"

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var prescriptionsSteps: PrescriptionsSteps
    @Steps
    lateinit var courseSteps: CourseSteps

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    lateinit var coursesData: MutableList<MedicationCourse>

    @Given("I have (\\d+) assigned prescriptions")
    fun iHaveXAssignedPrescriptions(numberOfCourses: Int){
        coursesData = CoursesData.getCourseData(numberOfCourses,0,0, mutableListOf())
        mockingClient.forEmis { coursesRequest(patient).respondWithSuccess(CourseRequestsGetResponse(coursesData)) }
    }

    @And("(\\d+) of my prescriptions are of type repeat")
    fun xOfMyPrescriptionsAreOfTypeRepeat(numOfRepeats: Int){
        if(coursesData == null){
            throw Exception("No courses have been provisioned")
        }

        if(numOfRepeats > coursesData.count()){
            throw Exception("Number of repeatable courses must be less than or equal to total number of courses")
        }

        for (course in 1..numOfRepeats) {
            coursesData[course - 1].prescriptionType = PrescriptionType.Repeat
        }

        mockingClient.forEmis { coursesRequest(patient).respondWithSuccess(CourseRequestsGetResponse(coursesData)) }
    }

    @And("(\\d+) of my prescriptions can be requested")
    fun xOfMyPrescriptionCanBeRequested(numCanBeRequested: Int){
        if(numCanBeRequested > coursesData.count()){
            throw Exception("Number of courses which can be requested must be less than or equal to total number of courses")
        }

        for (course in 1..numCanBeRequested) {
            coursesData[course - 1].canBeRequested = true
        }

        mockingClient.forEmis { coursesRequest(patient).respondWithSuccess(CourseRequestsGetResponse(coursesData)) }
        Serenity.setSessionVariable(EXPECTED_COURSE_DATA_SIZE).to(numCanBeRequested)
    }

    @When("I get the users courses with a valid cookie")
    fun whenIGetTheUsersCoursesWithAnValidCookie() {
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getCoursesConnection(null)

            Serenity.setSessionVariable(CourseListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive a list of (\\d+) repeating prescriptions that can be requested$")
    fun thenIReceiveAListOfXRepeatingPrescriptionsDateDescending(expectedNumberOfRepeatingPrescriptions: Int) {
        val result = Serenity.sessionVariableCalled<CourseListResponse>(CourseListResponse::class)
        Assert.assertNotNull("No courses in session", result)

        var actualNumberOfRepeatPrescriptions = result.response.courses.count()

        Assert.assertEquals(
                "Unexpected number of repeat prescriptions. Expected: "
                        + expectedNumberOfRepeatingPrescriptions
                        + ". Actual: "
                        +  actualNumberOfRepeatPrescriptions,
                expectedNumberOfRepeatingPrescriptions,
                result.response.courses.count())
    }

    @Given("^I have historic prescriptions$")
    fun iHaveHistoricPrescriptions() {

        val medicationCourses = mutableListOf<MedicationCourse>()
        val prescriptions = mutableListOf<PrescriptionRequest>()

        mockingClient.forEmis { prescriptionsRequest(patient)
                .respondWithSuccess(PrescriptionRequestsGetResponse(prescriptions, medicationCourses)) }
    }

    @When("I click 'Order a repeat prescription'")
    fun iClickOrderARepeatPrescription () {
        prescriptionsSteps.prescriptions.clickOrderARepeatPrescriptionButton()
    }

    @Then("I see the available repeatable prescriptions")
    fun iSeeTheAvailableRepeatablePrescriptions() {
        courseSteps.isLoaded()

        coursesData = coursesData.filter { medicationCourse -> medicationCourse.canBeRequested }.toMutableList()
        coursesData = coursesData.filter { medicationCourse -> medicationCourse.prescriptionType == PrescriptionType.Repeat }.toMutableList()
        coursesData = coursesData.sortedBy { medicationCourse -> medicationCourse.name }.toMutableList()
        coursesData = coursesData.take(100).toMutableList()
        courseSteps.assertCorrectRepeatPrescriptionsShown(coursesData)
    }
}
