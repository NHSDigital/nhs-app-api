package features.courses.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.courses.CoursesData
import features.courses.steps.ConfirmRepeatPrescriptionOrderSteps
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
import worker.models.courses.CoursesResponseData


open class coursesStepDefinitions {

    val EXPECTED_COURSE_DATA_SIZE = "CourseDataSize"
    val HTTP_EXCEPTION = "HttpException"

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var prescriptionsSteps: PrescriptionsSteps
    @Steps
    lateinit var courseSteps: CourseSteps
    @Steps
    lateinit var confirmRepeatPrescriptionOrderSteps: ConfirmRepeatPrescriptionOrderSteps

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    lateinit var coursesData: MutableList<MedicationCourse>

    lateinit var selectedCourses: List<MedicationCourse>

    @Given("^I have (\\d+) assigned prescriptions$")
    fun iHaveXAssignedPrescriptions(numberOfCourses: Int) {
        coursesData = CoursesData.getCourseData(numberOfCourses,0,0, mutableListOf(), true, true)
        mockingClient.forEmis {
            coursesRequest(patient)
                    .respondWithSuccess(CourseRequestsGetResponse(coursesData))
        }
    }

    @Given("I have (\\d+) assigned prescriptions which have (.*)")
    fun iHaveXAssignedPrescriptionsWhichHasX(numberOfCourses: Int, content: String) {
        val showDosage = content.toLowerCase().contains("dosage")
        val showQuantity = content.toLowerCase().contains("quantity")

        coursesData = CoursesData.getCourseData(numberOfCourses,0,0, mutableListOf(), showDosage, showQuantity)
        mockingClient.forEmis {
            coursesRequest(patient)
                    .respondWithSuccess(CourseRequestsGetResponse(coursesData))
        }
    }

    @And("(\\d+) of my prescriptions are of type repeat")
    fun xOfMyPrescriptionsAreOfTypeRepeat(numOfRepeats: Int){
        if(coursesData == null) {
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
    fun xOfMyPrescriptionCanBeRequested(numCanBeRequested: Int) {
        if(coursesData == null) {
            throw Exception("No courses have been provisioned")
        }

        if(numCanBeRequested > coursesData.count()) {
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

            Serenity.setSessionVariable(CoursesResponseData::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive a list of (\\d+) repeating prescriptions that can be requested$")
    fun thenIReceiveAListOfXRepeatingPrescriptionsDateDescending(expectedNumberOfRepeatingPrescriptions: Int) {
        val result = Serenity.sessionVariableCalled<CoursesResponseData>(CoursesResponseData::class)
        Assert.assertNotNull("No courses in session", result)

        var actualNumberOfRepeatPrescriptions = result.courses.count()

        Assert.assertEquals(
                "Unexpected number of repeat prescriptions. Expected: "
                        + expectedNumberOfRepeatingPrescriptions
                        + ". Actual: "
                        +  actualNumberOfRepeatPrescriptions,
                expectedNumberOfRepeatingPrescriptions,
                result.courses.count())
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
        val coursesToCheck = getAvailableCoursesFilteredSortedOrdered()
        courseSteps.assertCorrectRepeatPrescriptionsShown(coursesToCheck)
    }

    @Given("^I select (\\d+) repeatable prescriptions out of (\\d+) available$")
    fun iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect: Int, numberOfPrescriptionsToCreate: Int) {
        iHaveXAssignedPrescriptions(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
        iClickOrderARepeatPrescription()

        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.take(numberOfPrescriptionsToSelect)
        courseSteps.selectRepeatPrescriptions(coursesToSelect)
        selectedCourses = coursesToSelect
    }

    @Given("^I select (\\d+) repeatable prescriptions out of (\\d+) available which have (.*)")
    fun iSelectXRepeatablePrescriptionsWhichHaveX(numberOfPrescriptionsToSelect: Int, numberOfPrescriptionsToCreate: Int, content: String) {
        iHaveXAssignedPrescriptionsWhichHasX(numberOfPrescriptionsToCreate, content)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
        iClickOrderARepeatPrescription()

        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.take(numberOfPrescriptionsToSelect)
        courseSteps.selectRepeatPrescriptions(coursesToSelect)
        selectedCourses = coursesToSelect
    }

    @And("I enter text \"(.*)\" for special request")
    fun iEnterTextForSpecialRequest(text: String) {
        courseSteps.repeatPrescriptions.typeTextIntoSpecialRequestTextArea(text)
    }

    @When("I click Continue on the Order a repeat prescription page")
    fun iClickContinueOnTheOrderARepeatPrescriptionsPage() {
        courseSteps.repeatPrescriptions.clickContinueButton()
    }

    @When("I click 'Change this repeat prescription' on the Prescription confirmation page")
    fun iClickChangeThisRepeatPrescriptionOnThePrescriptionConfirmationPage() {
        confirmRepeatPrescriptionOrderSteps.clickChangeThisPrescriptionButton()
    }

    @Then("I see the previously selected prescriptions on the Confirm repeat prescription page")
    fun iSeeThePreviouslySelectedPrescriptionsOnTheConfirmRepeatPrescriptionPage() {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.confirmRepeatPrescriptionsOrderPage.verifySelectedRepeatPrescriptions(selectedCourses)
    }

    @Then("I see the special request text \"(.*)\"")
    fun iSeeTheSpecialRequestText(value: String) {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.assertSpecialRequest(value)
    }

    @Then("I see my previously selected repeat prescriptions selected")
    fun iSeeMyPreviouslySelectedRepeatPrescriptionsSelected() {
        courseSteps.isLoaded()
        courseSteps.assertCorrectRepeatPrescriptionsSelected(selectedCourses)
    }

    @Then("A validation message is displayed indicating the user has not selected any repeat prescriptions")
    fun aValidationMessageIsDisplayedIndicatingTheUserhasNotSelectedAnyRepeatPrescriptions() {
        courseSteps.assertNoRepeatPrescriptionsSelectedMessageShown()
    }

    @When("I select (\\d+) additional repeat prescriptions")
    fun iSelectXAdditionalRepeatPrescriptions(numberOfAdditionalRepeatPrescriptionsToSelect: Int) {
        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.drop(selectedCourses.size).take(numberOfAdditionalRepeatPrescriptionsToSelect)
        courseSteps.selectRepeatPrescriptions(coursesToSelect)
        selectedCourses = selectedCourses.plus(coursesToSelect)
    }

    private fun getAvailableCoursesFilteredSortedOrdered() : List<MedicationCourse> {
        var coursesDataFiltered = coursesData.filter { medicationCourse -> medicationCourse.canBeRequested }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.filter { medicationCourse -> medicationCourse.prescriptionType == PrescriptionType.Repeat }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.sortedBy { medicationCourse -> medicationCourse.name }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.take(100).toMutableList()

        return coursesDataFiltered
    }
}
