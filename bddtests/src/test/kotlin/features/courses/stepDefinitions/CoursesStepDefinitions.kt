package features.courses.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.authentication.steps.LoginSteps
import features.prescriptions.factories.PrescriptionsFactory
import features.prescriptions.helpers.PrescriptionHelpers
import features.prescriptions.stepDefinitions.PrescriptionsSerenityHelpers
import features.prescriptions.stepDefinitions.ProviderTypes
import features.prescriptions.steps.PrescriptionsSteps
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsHistoryJourney
import mocking.emis.practices.NecessityOption
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.hamcrest.CoreMatchers.containsString
import org.junit.Assert
import pages.nominatedPharmacy.NominatedPharmacyCheckPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.RepeatPrescriptionsPage
import utils.SerenityHelpers
import utils.getOrNull
import utils.set

open class CoursesStepDefinitions {

    private val isVisibleIndicator = "is"

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var prescriptionsSteps: PrescriptionsSteps

    val mockingClient = MockingClient.instance

    private lateinit var repeatPrescriptions : RepeatPrescriptionsPage
    private lateinit var nominatedPharmacyCheckPage : NominatedPharmacyCheckPage
    private lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage

    lateinit var coursesLoader: ICoursesLoader<*>

    lateinit var selectedCourses: List<MedicationCourse>

    var numOfCourses: Int = 0
    var numOfRepeats: Int = 0
    var numCanBeRequested: Int = 0
    var showQuantity: Boolean = true
    var showDosage: Boolean = true

    @Given("^I have (\\d+) assigned prescriptions$")
    fun iHaveXAssignedPrescriptions(numberOfCourses: Int) {
        numOfCourses = numberOfCourses
        initialize()
    }

    @Given("^I have (\\d+) assigned prescriptions which have (.*)$")
    fun iHaveXAssignedPrescriptionsWhichHasX(numberOfCourses: Int, content: String) {
        numOfCourses = numberOfCourses
        showDosage = content.toLowerCase().contains("dosage")
        showQuantity = content.toLowerCase().contains("quantity")
        setupWiremockandCreateData()
    }

    @Given("^I have historic prescriptions$")
    fun iHaveHistoricPrescriptions() {
        val currentPatient = SerenityHelpers.getPatient()
        var currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()
        if (currentProvider == null) {
            initialize()
            currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()
        }

        if (currentProvider == ProviderTypes.EMIS) {
            PrescriptionsHistoryJourney(mockingClient).createFor(currentPatient)
        }
        else if (currentProvider == ProviderTypes.VISION) {
            PrescriptionsHistoryJourney(mockingClient).createFor(currentPatient)
        }
        else if (currentProvider == ProviderTypes.MICROTEST) {
            PrescriptionsHistoryJourney(mockingClient).createFor(currentPatient)
        }
    }

    @Given("^I select (\\d+) repeatable prescriptions out of (\\d+) available$")
    fun iSelectXRepeatablePrescriptionsOutOf(numberOfPrescriptionsToSelect: Int, numberOfPrescriptions: Int) {
        iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect)

        repeatPrescriptions.verifyVisiblePrescriptionCount(numberOfPrescriptions)
    }

    @Given("^I have (\\d+) repeatable prescriptions available which have (.*)")
    fun iHaveXRepeatablePrescriptionsWhichHaveX(numberOfPrescriptionsToCreate: Int,
                                                  content: String) {
        iHaveXAssignedPrescriptionsWhichHasX(numberOfPrescriptionsToCreate, content)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
    }

    fun iSelectXRepeatablePrescriptions(numberOfPrescriptionsToSelect: Int) {
        iClickOrderARepeatPrescription()
        nominatedPharmacyCheckPage.continueButton.click()
        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.take(numberOfPrescriptionsToSelect)
        for (course in coursesToSelect) {
            repeatPrescriptions.selectRepeatPrescription(course)
        }
        selectedCourses = coursesToSelect
    }

    @Given("^there are (\\d*) repeatable prescriptions available$")
    fun thereAreXXRepeatablePrescriptionsAvailable(numberOfPrescriptionsToCreate: Int) {
        iHaveXAssignedPrescriptions(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
    }

    @Given("(\\d+) of my prescriptions are of type repeat")
    fun xOfMyPrescriptionsAreOfTypeRepeat(numberOfRepeats: Int) {
        numOfRepeats = numberOfRepeats
    }

    @Given("(\\d+) of my prescriptions can be requested")
    fun xOfMyPrescriptionCanBeRequested(numberCanBeRequested: Int) {
        numCanBeRequested = numberCanBeRequested
        setupWiremockandCreateData()
    }

    @Given("special request text has been enabled")
    fun gpProviderHasEnabledSpecialRequestText() {
        initialize()

        PrescriptionHelpers.setPrescriptionCommentsAllowed(true)
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()

        if (currentProvider == ProviderTypes.EMIS) {
            setupSpecialRequestConfigEmis()
        }
    }

    @Given("special request text has been disabled")
    fun gpProviderHasDisabledSpecialRequestText() {
        initialize()

        PrescriptionHelpers.setPrescriptionCommentsAllowed(false)
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()

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
        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis {
            practiceSettingsRequest(currentPatient)
                    .respondWithSuccess(response)
        }
    }

    @When("I enter text \"(.*)\" for special request")
    fun iEnterTextForSpecialRequest(text: String) {
        Serenity.setSessionVariable("specialRequestText")
                .to(repeatPrescriptions.typeTextIntoSpecialRequestTextArea(text))
    }

    @When("I click 'Order a new repeat prescription'")
    fun iClickOrderARepeatPrescription() {
        prescriptionsSteps.prescriptions.clickOrderARepeatPrescriptionButton()
    }

    @When("I click Continue on the Order a repeat prescription page")
    fun iClickContinueOnTheOrderARepeatPrescriptionsPage() {
        repeatPrescriptions.clickOnButtonContainingText("Continue")
    }

    @When("I click 'Change this repeat prescription' on the Prescription confirmation page")
    fun iClickChangeThisRepeatPrescriptionOnThePrescriptionConfirmationPage() {
        confirmRepeatPrescriptionsOrderPage.clickChangeThisPrescriptionButton()
    }

    @When("I select (\\d+) additional repeat prescriptions")
    fun iSelectXAdditionalRepeatPrescriptions(numberOfAdditionalRepeatPrescriptionsToSelect: Int) {
        val courses = getAvailableCoursesFilteredSortedOrdered()
        val coursesToSelect = courses.drop(selectedCourses.size)
                .take(numberOfAdditionalRepeatPrescriptionsToSelect)
        for (course in coursesToSelect) {
            repeatPrescriptions.selectRepeatPrescription(course)
        }
        selectedCourses = selectedCourses.plus(coursesToSelect)
    }

    @Then("a message is displayed indicating that you don't have any medication available to order")
    fun aMessageIsDisplayedIndicatingThatYouDontHaveAnyRepeatMedicationAvailableToOrder() {
        Assert.assertTrue(repeatPrescriptions.isNoMedicationAvailableToOrderMessageVisible())
    }

    @Then("I see the available repeatable prescriptions")
    fun iSeeTheAvailableRepeatablePrescriptions() {
        repeatPrescriptions.shouldBeDisplayed()
        val coursesToCheck = getAvailableCoursesFilteredSortedOrdered()
        repeatPrescriptions.verifyVisiblePrescriptions(coursesToCheck)
    }

    @Then("I don't see the special request text on prescription confirmation")
    fun iDontSeeTheSpecialRequestTextOnPrescriptionConfirmation() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertFalse(confirmRepeatPrescriptionsOrderPage.specialRequestElementIsVisible())
    }

    @Then("I see the previously selected prescriptions on the Confirm repeat prescription page")
    fun iSeeThePreviouslySelectedPrescriptionsOnTheConfirmRepeatPrescriptionPage() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        confirmRepeatPrescriptionsOrderPage
                .verifySelectedRepeatPrescriptions(selectedCourses)
    }

    @Then("I see the entered special request text")
    fun iSeeTheSpecialRequestText() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertThat(confirmRepeatPrescriptionsOrderPage.getSpecialRequest(),
                containsString(Serenity.sessionVariableCalled("specialRequestText")))
    }

    @Then("I see the default special request text")
    fun iSeeTheDefaultSpecialRequestText() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertThat(confirmRepeatPrescriptionsOrderPage.getSpecialRequest(), containsString("None"))
    }

    @Then("I see the special request text area")
    fun iSeeTheSpecialRequestTextbox() {
        repeatPrescriptions.shouldBeDisplayed()
        Assert.assertTrue(repeatPrescriptions.isSpecialRequestTextAreaVisible())
    }

    @Then("I don't see the special request text area")
    fun iDontSeeTheSpecialRequestTextbox() {
        repeatPrescriptions.shouldBeDisplayed()
        Assert.assertFalse(repeatPrescriptions.isSpecialRequestTextAreaVisible())
    }

    @Then("I see my previously selected repeat prescriptions selected")
    fun iSeeMyPreviouslySelectedRepeatPrescriptionsSelected() {
        repeatPrescriptions.shouldBeDisplayed()
        for (course in selectedCourses) {
            repeatPrescriptions.verifyPrescriptionIsSelected(course)
        }
    }

    @Then("A validation message (.*) displayed indicating the user has not selected any repeat prescriptions")
    fun aValidationMessageIsDisplayedIndicatingTheUserHasNotSelectedAnyRepeatPrescriptions(visibility: String) {
        Assert.assertTrue(
                repeatPrescriptions.isNoRepeatPrescriptionsSelectedMessageVisible() ==
                (visibility.toLowerCase() == isVisibleIndicator))
    }

    private fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        return coursesLoader.getAvailableCoursesFilteredSortedOrdered()
    }

    private fun setupWiremockandCreateData() {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()
        if (currentProvider == null) {
            initialize()
        }

        val prescriptionsFactory = PrescriptionsFactory.getForSupplier(currentProvider.toString())

        prescriptionsFactory.setupWireMockAndCreateData(
                numOfCourses,
                numOfRepeats,
                numCanBeRequested,
                showDosage,
                showQuantity)
    }

    private fun initialize() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val factory = PrescriptionsFactory.getForSupplier(gpSystem)

        SerenityHelpers.setPatient(factory.patient)
        coursesLoader = factory.getCoursesLoader
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<ProviderTypes>()

        if (currentProvider == null) {
            PrescriptionsSerenityHelpers.PROVIDER.set( ProviderTypes.valueOf(gpSystem))
        }
    }
}
