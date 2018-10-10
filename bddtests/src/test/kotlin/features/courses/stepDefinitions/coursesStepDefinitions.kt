package features.courses.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.gpServiceBuilderInterfaces.Courses.ICoursesLoader
import mocking.data.prescriptions.courses.TppCoursesLoader
import features.courses.steps.ConfirmRepeatPrescriptionOrderSteps
import features.courses.steps.CourseSteps
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.BaseStepDefinition
import mocking.MockingClient
import mocking.emis.models.*
import mocking.tpp.models.ListRepeatMedicationReply
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.courses.CoursesListResponse
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import mocking.data.prescriptions.courses.VisionCoursesLoader
import mocking.defaults.MockDefaults
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsHistoryJourney
import mocking.vision.models.EligibleRepeats

open class CoursesStepDefinitions : BaseStepDefinition() {

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


    lateinit var coursesLoader: ICoursesLoader<*>

    lateinit var selectedCourses: List<MedicationCourse>

    var numOfCourses: Int = 0
    var numOfRepeats: Int = 0
    var numCanBeRequested: Int = 0
    var showQuantity: Boolean = true
    var showDosage: Boolean = true


    @Given("I have (\\d+) (.*) assigned prescriptions")
    fun iHaveXAssignedPrescriptions(numberOfCourses: Int, gpSystem: String) {
        numOfCourses = numberOfCourses
        initalize(gpSystem)
    }

    @Given("I have (\\d+) assigned prescriptions which have (.*)")
    fun iHaveXAssignedPrescriptionsWhichHasX(numberOfCourses: Int, content: String) {
        numOfCourses = numberOfCourses
        showDosage = content.toLowerCase().contains("dosage")
        showQuantity = content.toLowerCase().contains("quantity")
        setupWiremockandCreateData()
    }

    @And("(\\d+) of my prescriptions are of type repeat")
    fun xOfMyPrescriptionsAreOfTypeRepeat(numberOfRepeats: Int) {
        numOfRepeats = numberOfRepeats
    }

    @And("(\\d+) of my prescriptions can be requested")
    fun xOfMyPrescriptionCanBeRequested(numberCanBeRequested: Int) {
        numCanBeRequested = numberCanBeRequested

        setupWiremockandCreateData()

    }

    @When("I get the users courses with a valid cookie")
    fun whenIGetTheUsersCoursesWithAnValidCookie() {
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getCoursesConnection()

            Serenity.setSessionVariable(CoursesListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive a list of (\\d+) repeating prescriptions that can be requested$")
    fun thenIReceiveAListOfXRepeatingPrescriptionsDateDescending(expectedNumberOfRepeatingPrescriptions: Int) {
        val result = Serenity.sessionVariableCalled<CoursesListResponse>(CoursesListResponse::class)
        Assert.assertNotNull("No courses in session", result)

        val actualNumberOfRepeatPrescriptions = result.courses.count()

        Assert.assertEquals(
                "Unexpected number of repeat prescriptions. Expected: "
                        + expectedNumberOfRepeatingPrescriptions
                        + ". Actual: "
                        + actualNumberOfRepeatPrescriptions,
                expectedNumberOfRepeatingPrescriptions,
                result.courses.count())
    }

    @Given("^I have historic prescriptions$")
    fun iHaveHistoricPrescriptions() {
        configureWireMockForHistoricPrescriptions()
    }

    @When("I click 'Order a new repeat prescription'")
    fun iClickOrderARepeatPrescription() {
        prescriptionsSteps.prescriptions.clickOrderARepeatPrescriptionButton()
    }

    @Then("I see the available repeatable prescriptions")
    fun iSeeTheAvailableRepeatablePrescriptions() {
        courseSteps.isLoaded()
        val coursesToCheck = getAvailableCoursesFilteredSortedOrdered()
        courseSteps.assertCorrectRepeatPrescriptionsShown(coursesToCheck)
    }

    @And("a message is displayed indicating that you don't have any medication available to order")
    fun aMessageIsDisplayedIndicatingThatYouDontHaveAnyRepeatMedicationAvailableToOrder() {
        courseSteps.assertNoMedicationAvailableToOrderMessageShown()
    }

    @Given("I select (\\d+) (.*) repeatable prescriptions out of (\\d+) available")
    fun iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect: Int, gpSystem: String, numberOfPrescriptionsToCreate: Int) {
        iHaveXAssignedPrescriptions(numberOfPrescriptionsToCreate, gpSystem)
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

    private fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        return coursesLoader.getAvailableCoursesFilteredSortedOrdered()
    }

    private fun configureWireMockForHistoricPrescriptions() {
        if (currentProvider == null) {
            initalize(Serenity.sessionVariableCalled<String>(GLOBAL_PROVIDER_TYPE))
        }

        if (currentProvider == ProviderTypes.EMIS) {
            PrescriptionsHistoryJourney(mockingClient).createFor(currentPatient)

        }

        if (currentProvider == ProviderTypes.VISION) {
            PrescriptionsHistoryJourney(mockingClient).createFor(currentPatient)

        }
    }

    @Suppress("UNCHECKED_CAST")
    private fun setupWiremockandCreateData() {

        if (currentProvider == null) {
            initalize(Serenity.sessionVariableCalled<String>(GLOBAL_PROVIDER_TYPE))
        }

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                coursesLoader.loadData(numOfCourses, numOfRepeats, numCanBeRequested,
                        showDosage, showQuantity)

                mockingClient.forEmis {
                    coursesRequest(currentPatient)
                            .respondWithSuccess(CourseRequestsGetResponse(coursesLoader.data as List<MedicationCourse>))
                }
            }
            ProviderTypes.TPP -> {
                coursesLoader.loadData(numOfCourses, numOfRepeats, numCanBeRequested,
                        showDosage, showQuantity)

                mockingClient.forTpp {
                    listRepeatMedication(currentPatient)
                            .respondWithSuccess(coursesLoader.data as ListRepeatMedicationReply)
                }
            }
            ProviderTypes.VISION -> {
                coursesLoader.loadData(numOfCourses, numOfRepeats, numCanBeRequested,
                        showDosage, showQuantity)

                mockingClient.forVision {
                    getEligibleRepeatsRequest(MockDefaults.visionUserSession, MockDefaults.visionGetEligibleRepeats)
                            .respondWithSuccess(coursesLoader.data as EligibleRepeats)
                }
            }
        }
    }

    private fun initalize(gpSystem: String) {
        if (currentProvider == null) {
            currentProvider = ProviderTypes.valueOf(gpSystem)
        }

        when (currentProvider) {
            ProviderTypes.EMIS -> {
                coursesLoader = EmisCoursesLoader
                currentPatient = EMIS_PATIENT
            }
            ProviderTypes.TPP -> {
                coursesLoader = TppCoursesLoader
                currentPatient = TPP_PATIENT
            }
            ProviderTypes.VISION -> {
                coursesLoader = VisionCoursesLoader
                currentPatient = VISION_PATIENT
            }
        }
    }
}
