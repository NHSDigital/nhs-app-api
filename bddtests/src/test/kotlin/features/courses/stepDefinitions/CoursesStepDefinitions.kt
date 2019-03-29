package features.courses.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.courses.steps.ConfirmRepeatPrescriptionOrderSteps
import features.courses.steps.CourseSteps
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.helpers.PrescriptionHelpers
import features.prescriptions.steps.PrescriptionsSteps
import features.sharedStepDefinitions.BaseStepDefinition
import features.sharedStepDefinitions.BaseStepDefinition.Companion.ProviderTypes
import features.sharedStepDefinitions.GLOBAL_PROVIDER_TYPE
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsHistoryJourney
import mocking.emis.practices.NecessityOption
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.prescription.RepeatPrescriptionsPage
import utils.SerenityHelpers
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.courses.CoursesListResponse

open class CoursesStepDefinitions : BaseStepDefinition() {

    val isVisibleIndicator = "is"

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var prescriptionsSteps: PrescriptionsSteps
    @Steps
    lateinit var courseSteps: CourseSteps

    lateinit var repeatPrescriptions : RepeatPrescriptionsPage

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
                    .prescriptions.getCoursesConnection()

            Serenity.setSessionVariable(CoursesListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
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

    @And("(.*) has enabled special request text")
    fun gpProviderHasEnabledSpecialRequestText(gpSystem: String) {
        initalize(gpSystem)

        PrescriptionHelpers.setPrescriptionCommentsAllowed(true)

        if (currentProvider == ProviderTypes.EMIS) {
            setupSpecialRequestConfigEmis()
        }
    }

    @And("(.*) has disabled special request text")
    fun gpProviderHasDisabledSpecialRequestText(gpSystem: String) {
        initalize(gpSystem)

        PrescriptionHelpers.setPrescriptionCommentsAllowed(false)

        if (currentProvider == ProviderTypes.EMIS) {
            setupSpecialRequestConfigEmis()
        }
    }

    private fun setupSpecialRequestConfigEmis() {
        val response = SettingsResponseModel()

        if (PrescriptionHelpers.getPrescriptionCommentsAllowed()) {
            response.inputRequirements.prescribingComment = NecessityOption.OPTIONAL.text
        } else {
            response.inputRequirements.prescribingComment = NecessityOption.NOT_ALLOWED.text
        }
        mockingClient.forEmis {
            practiceSettingsRequest(currentPatient)
                    .respondWithSuccess(response)
        }
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

    @Given("I select (\\d+) repeatable prescriptions out of (\\d+) available$")
    fun iSelectXRepeatablePrescriptionsOutOf(numberOfPrescriptionsToSelect: Int, numberOfPrescriptions: Int) {
        iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect)

        repeatPrescriptions.verifyVisiblePrescriptionCount(numberOfPrescriptions)
    }

    fun iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect: Int) {
        iClickOrderARepeatPrescription()

        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.take(numberOfPrescriptionsToSelect)
        courseSteps.selectRepeatPrescriptions(coursesToSelect)
        selectedCourses = coursesToSelect
    }

    @Given("^there are (\\d*) (.*) repeatable prescriptions available$")
    fun thereAreXXRepeatablePrescriptionsAvailable(numberOfPrescriptionsToCreate: Int, gpSystem: String) {
        iHaveXAssignedPrescriptions(numberOfPrescriptionsToCreate, gpSystem)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
    }

    @Given("^I select (\\d+) repeatable prescriptions out of (\\d+) available which have (.*)")
    fun iSelectXRepeatablePrescriptionsWhichHaveX(numberOfPrescriptionsToSelect: Int,
                                                  numberOfPrescriptionsToCreate: Int,
                                                  content: String) {
        iHaveXAssignedPrescriptionsWhichHasX(numberOfPrescriptionsToCreate, content)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)

        iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect)
    }

    @And("I enter text \"(.*)\" for special request")
    fun iEnterTextForSpecialRequest(text: String) {
        Serenity.setSessionVariable("specialRequestText")
                .to(courseSteps.repeatPrescriptions.typeTextIntoSpecialRequestTextArea(text))
    }

    @When("I click Continue on the Order a repeat prescription page")
    fun iClickContinueOnTheOrderARepeatPrescriptionsPage() {
        courseSteps.repeatPrescriptions.orderRepeatPrescriptionButton.click()
    }

    @When("I click 'Change this repeat prescription' on the Prescription confirmation page")
    fun iClickChangeThisRepeatPrescriptionOnThePrescriptionConfirmationPage() {
        confirmRepeatPrescriptionOrderSteps.clickChangeThisPrescriptionButton()
    }

    @Then("I don't see the special request text on prescription confirmation")
    fun iDontSeeTheSpecialRequestTextOnPrescriptionConfirmation() {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.assertSpecialRequestNotShown()
    }

    @Then("I see the previously selected prescriptions on the Confirm repeat prescription page")
    fun iSeeThePreviouslySelectedPrescriptionsOnTheConfirmRepeatPrescriptionPage() {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.confirmRepeatPrescriptionsOrderPage
                .verifySelectedRepeatPrescriptions(selectedCourses)
    }

    @Then("I see the entered special request text")
    fun iSeeTheSpecialRequestText() {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.assertSpecialRequest(Serenity.sessionVariableCalled("specialRequestText"))
    }

    @Then("I see the default special request text")
    fun iSeeTheDefaultSpecialRequestText() {
        confirmRepeatPrescriptionOrderSteps.isLoaded()
        confirmRepeatPrescriptionOrderSteps.assertSpecialRequest("None")
    }

    @Then("I see the special request text area")
    fun iSeeTheSpecialRequestTextbox() {
        courseSteps.isLoaded()
        courseSteps.assertSpecialRequestTextAreaShown()
    }

    @Then("I don't see the special request text area")
    fun iDontSeeTheSpecialRequestTextbox() {
        courseSteps.isLoaded()
        courseSteps.assertSpecialRequestTextAreaNotShown()
    }

    @Then("I see my previously selected repeat prescriptions selected")
    fun iSeeMyPreviouslySelectedRepeatPrescriptionsSelected() {
        courseSteps.isLoaded()
        courseSteps.assertCorrectRepeatPrescriptionsSelected(selectedCourses)
    }

    @Then("A validation message (.*) displayed indicating the user has not selected any repeat prescriptions")
    fun aValidationMessageIsDisplayedIndicatingTheUserhasNotSelectedAnyRepeatPrescriptions(visibility: String) {
        courseSteps.assertNoRepeatPrescriptionsSelectedMessageVisibility(visibility.toLowerCase() == isVisibleIndicator)
    }

    @When("I select (\\d+) additional repeat prescriptions")
    fun iSelectXAdditionalRepeatPrescriptions(numberOfAdditionalRepeatPrescriptionsToSelect: Int) {
        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.drop(selectedCourses.size).take(numberOfAdditionalRepeatPrescriptionsToSelect)
        courseSteps.selectRepeatPrescriptions(coursesToSelect)
        selectedCourses = selectedCourses.plus(coursesToSelect)
    }

    @Then("I see a message indicating there was an error sending my order")
    fun iSeeAMessageOrderNotSuccessful() {
        confirmRepeatPrescriptionOrderSteps.assertErrorSendingOrderShown()
    }

    @Then("I see a message indicating I've previously ordered one of the selected medications within the last 30 days")
    fun iSeeAMessageIndicatingIvePreviouslyOrderedOneOfTheSelectedMedicationsWithinTheLast30days() {
        confirmRepeatPrescriptionOrderSteps.assertMedicationOrderedWithinTheLast30DaysErrorShown()
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

    private fun setupWiremockandCreateData() {

        if (currentProvider == null) {
            initalize(Serenity.sessionVariableCalled<String>(GLOBAL_PROVIDER_TYPE))
        }

        val prescriptionsFactory = PrescriptionsFactory.getForSupplier(currentProvider.toString())

        prescriptionsFactory.setupWireMockAndCreateData(
                numOfCourses,
                numOfRepeats,
                numCanBeRequested,
                showDosage,
                showQuantity)
    }

    private fun initalize(gpSystem: String) {

        val factory = PrescriptionsFactory.getForSupplier(gpSystem)

        currentPatient = factory.patient
        coursesLoader = factory.getCoursesLoader

        if (currentProvider == null) {
            currentProvider = ProviderTypes.valueOf(gpSystem)
        }
    }
}
