package features.courses.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.authentication.steps.LoginSteps
import features.prescriptions.helpers.PrescriptionHelpers
import features.prescriptions.stepDefinitions.PrescriptionsSerenityHelpers
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsHistoryJourney
import mocking.emis.practices.NecessityOption
import mocking.emis.practices.SettingsResponseModel
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.hamcrest.CoreMatchers.containsString
import org.junit.Assert
import pages.PrescriptionsHubPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.nominatedPharmacy.NominatedPharmacyCheckPage
import pages.prescription.ConfirmRepeatPrescriptionsOrderPage
import pages.prescription.RepeatPrescriptionsPage
import utils.SerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set

private const val MILLISECONDS_TO_WAIT_FOR_CONTINUE: Long = 500
private const val SPECIAL_REQUEST_LIMIT = 450

open class CoursesStepDefinitions {

    private val isVisibleIndicator = "is"

    @Steps
    lateinit var login: LoginSteps

    private val mockingClient = MockingClient.instance

    private lateinit var prescriptionsHubPage : PrescriptionsHubPage
    private lateinit var repeatPrescriptions : RepeatPrescriptionsPage
    private lateinit var nominatedPharmacyCheckPage : NominatedPharmacyCheckPage
    private lateinit var confirmRepeatPrescriptionsOrderPage : ConfirmRepeatPrescriptionsOrderPage

    lateinit var coursesLoader: ICoursesLoader<*>

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
        var currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        if (currentProvider == null) {
            initialize()
            currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        }

        if (currentProvider != Supplier.TPP) {
            PrescriptionsHistoryJourney.getForSupplier(currentProvider!!).createFor(currentPatient)
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
        PrescriptionsSerenityHelpers.SELECTED_COURSES.set(coursesToSelect)
    }

    @Given("^there are (\\d*) repeatable prescriptions available$")
    fun thereAreXXRepeatablePrescriptionsAvailable(numberOfPrescriptionsToCreate: Int) {
        iHaveXAssignedPrescriptions(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionsAreOfTypeRepeat(numberOfPrescriptionsToCreate)
        xOfMyPrescriptionCanBeRequested(numberOfPrescriptionsToCreate)
    }

    @Given("^(\\d+) of my prescriptions are of type repeat$")
    fun xOfMyPrescriptionsAreOfTypeRepeat(numberOfRepeats: Int) {
        numOfRepeats = numberOfRepeats
    }

    @Given("^(\\d+) of my prescriptions can be requested$")
    fun xOfMyPrescriptionCanBeRequested(numberCanBeRequested: Int) {
        numCanBeRequested = numberCanBeRequested
        setupWiremockandCreateData()
    }

    @Given("^special request text has been enabled and is '(.*)'$")
    fun gpProviderHasEnabledSpecialRequestText(necessityString: String) {
        initialize()
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        if (currentProvider == Supplier.EMIS) {
            when(necessityString) {
                "Mandatory" -> setupEmisSpecialRequestConfigWithNecessityOptionOf(NecessityOption.MANDATORY)
                "Optional" -> setupEmisSpecialRequestConfigWithNecessityOptionOf(NecessityOption.OPTIONAL)
                else -> Assert.fail("Invalid necessity option provided.")
            }
        }
    }

    @Given("^special request text has been disabled$")
    fun gpProviderHasDisabledSpecialRequestText() {
        initialize()

        PrescriptionHelpers.setPrescriptionCommentsAllowed(false)
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        if (currentProvider == Supplier.EMIS) {
            setupEmisSpecialRequestConfigWithNecessityOptionOf(NecessityOption.NOT_ALLOWED)
        }
    }

    private fun setupEmisSpecialRequestConfigWithNecessityOptionOf(necessityOption: NecessityOption) {
        val response = SettingsResponseModel()
        response.inputRequirements.prescribingComment = necessityOption.text

        val currentPatient = SerenityHelpers.getPatient()
        mockingClient.forEmis.mock {
            practiceSettingsRequest(currentPatient)
                    .respondWithSuccess(response)
        }
    }

    @When("^I enter text \"(.*)\" for special request$")
    fun iEnterTextForSpecialRequest(text: String) {
        Serenity.setSessionVariable("specialRequestText")
                .to(repeatPrescriptions.typeTextIntoSpecialRequestTextArea(text))
    }

    @When("^I enter a html script tag as text for special request$")
    fun iEnterTextForSpecialRequest() {
        Serenity.setSessionVariable("specialRequestText")
                .to(repeatPrescriptions.typeTextIntoSpecialRequestTextArea("<script>"))
    }

    @Then("^I can see how many characters I have left for my special request$")
    fun iCanSeeHowManyCharactersIHaveLeftForMySpecialRequest() {
        val text = getSpecialRequestCharactersRemainingText()
        Assert.assertTrue(repeatPrescriptions.specialRequestCharactersRemainingTextIsVisible(text))
    }

    @When("^I click 'Order a new repeat prescription'$")
    fun iClickOrderARepeatPrescription() {
        prescriptionsHubPage.clickOrderARepeatPrescriptionButton()
    }

    @When("^I click Continue on the Order a repeat prescription page$")
    fun iClickContinueOnTheOrderARepeatPrescriptionsPage() {
        Thread.sleep(MILLISECONDS_TO_WAIT_FOR_CONTINUE)
        repeatPrescriptions.clickOnButtonContainingText("Continue")
    }

    @When("^I click 'Help with medical abbreviations'$")
    fun iClickMedicalAbbreviationsLinkFromTheOrderARepeatPrescriptionsPage() {
        repeatPrescriptions.clickMedicalAbbreviationsLabel()
    }

    @When("^I click 'Change' for the prescriptions on the Prescription confirmation page$")
    fun iClickChangeThisRepeatPrescriptionOnThePrescriptionConfirmationPage() {
        confirmRepeatPrescriptionsOrderPage.clickChangeThisPrescriptionButton()
    }

    @When("^I click 'Change' for the special request on the Prescription confirmation page$")
    fun iClickChangeThisSpecialRequestOnThePrescriptionConfirmationPage() {
        confirmRepeatPrescriptionsOrderPage.clickChangeThisSpecialRequest()
    }

    @When("^I select (\\d+) additional repeat prescriptions$")
    fun iSelectXAdditionalRepeatPrescriptions(numberOfAdditionalRepeatPrescriptionsToSelect: Int) {
        val courses = getAvailableCoursesFilteredSortedOrdered()
        val selectedCourses = PrescriptionsSerenityHelpers.SELECTED_COURSES
                .getOrFail<List<MedicationCourse>>()
        val coursesToSelect = courses.drop(selectedCourses.size)
                .take(numberOfAdditionalRepeatPrescriptionsToSelect)
        for (course in coursesToSelect) {
            repeatPrescriptions.selectRepeatPrescription(course)
        }
        PrescriptionsSerenityHelpers.SELECTED_COURSES.set(selectedCourses.plus(coursesToSelect))
    }

    @Then("^I see the medical abbreviations help link$")
    fun iSeeTheMedicalAbbreviationsHelpLink() {
        Assert.assertTrue(repeatPrescriptions.checkMedicalAbbreviationsLink())
    }

    @Then("^a message is displayed indicating that you don't have any medication available to order$")
    fun aMessageIsDisplayedIndicatingThatYouDontHaveAnyRepeatMedicationAvailableToOrder() {
        Assert.assertTrue(repeatPrescriptions.isNoMedicationAvailableToOrderMessageVisible())
    }

    @Then("^I see the available repeatable prescriptions$")
    fun iSeeTheAvailableRepeatablePrescriptions() {
        repeatPrescriptions.shouldBeDisplayed()
        val coursesToCheck = getAvailableCoursesFilteredSortedOrdered()
        repeatPrescriptions.verifyVisiblePrescriptions(coursesToCheck)
    }

    @Then("^I don't see the special request text on prescription confirmation$")
    fun iDontSeeTheSpecialRequestTextOnPrescriptionConfirmation() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertFalse(confirmRepeatPrescriptionsOrderPage.specialRequestElementIsVisible())
    }

    @Then("^I see the previously selected prescriptions on the Confirm repeat prescription page$")
    fun iSeeThePreviouslySelectedPrescriptionsOnTheConfirmRepeatPrescriptionPage() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        confirmRepeatPrescriptionsOrderPage
                .verifySelectedRepeatPrescriptions(PrescriptionsSerenityHelpers.SELECTED_COURSES.getOrFail())
    }

    @Then("^I see the entered special request text$")
    fun iSeeTheSpecialRequestText() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertThat(confirmRepeatPrescriptionsOrderPage.getSpecialRequest(),
                containsString(Serenity.sessionVariableCalled("specialRequestText")))
    }

    @Then("^I see the default special request text$")
    fun iSeeTheDefaultSpecialRequestText() {
        confirmRepeatPrescriptionsOrderPage.shouldBeDisplayed()
        Assert.assertThat(confirmRepeatPrescriptionsOrderPage.getSpecialRequest(),
                containsString("No notes added"))
    }

    @Then("^I see the special request text area$")
    fun iSeeTheSpecialRequestTextbox() {
        repeatPrescriptions.shouldBeDisplayed()
        Assert.assertTrue(repeatPrescriptions.specialRequestTextArea.elements.isNotEmpty())
        repeatPrescriptions.specialRequestTextArea.assertIsVisible()
    }

    @Then("^I don't see the special request text area$")
    fun iDontSeeTheSpecialRequestTextbox() {
        repeatPrescriptions.shouldBeDisplayed()
        Assert.assertFalse(repeatPrescriptions.specialRequestTextArea.elements.isNotEmpty())
        repeatPrescriptions.specialRequestTextArea.assertElementNotPresent()
    }

    @Then("^I see my previously selected repeat prescriptions selected$")
    fun iSeeMyPreviouslySelectedRepeatPrescriptionsSelected() {
        repeatPrescriptions.shouldBeDisplayed()
        for (course in PrescriptionsSerenityHelpers.SELECTED_COURSES.getOrFail<List<MedicationCourse>>()) {
            repeatPrescriptions.verifyPrescriptionIsSelected(course)
        }
    }

    @Then("^A validation message (.*) displayed indicating the user has not selected any repeat prescriptions$")
    fun aValidationMessageIsDisplayedIndicatingTheUserHasNotSelectedAnyRepeatPrescriptions(visibility: String) {
        Assert.assertTrue(
                repeatPrescriptions.isNoRepeatPrescriptionsSelectedMessageVisible() ==
                (visibility.toLowerCase() == isVisibleIndicator))
    }

    @Then("^A validation message (.*) displayed indicating the user has not entered special request text$")
    fun aValidationMessageIsDisplayedIndicatingTheUserHasNotEnteredSpecialRequestText(visibility: String) {
        Assert.assertTrue(
                repeatPrescriptions.isNoSpecialRequestTextEnteredMessageVisible() ==
                        (visibility.toLowerCase() == isVisibleIndicator))
    }

    private fun getSpecialRequestCharactersRemainingText(): String {
        val specialRequest = Serenity.sessionVariableCalled<String>("specialRequestText") ?: ""
        val charactersRemaining = SPECIAL_REQUEST_LIMIT - specialRequest.length
        val pluralisedCharacters = when(charactersRemaining) {
            1 -> "character"
            else -> "characters"
        }

        return "You have $charactersRemaining $pluralisedCharacters remaining."
    }

    private fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        return coursesLoader.getAvailableCoursesFilteredSortedOrdered()
    }

    private fun setupWiremockandCreateData() {
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()
        if (currentProvider == null) {
            initialize()
        }

        val prescriptionsFactory = PrescriptionsFactory.getForSupplier(currentProvider!!)

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

        coursesLoader = factory.getCoursesLoader
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        if (currentProvider == null) {
            PrescriptionsSerenityHelpers.PROVIDER.set(gpSystem)
        }
    }
}
