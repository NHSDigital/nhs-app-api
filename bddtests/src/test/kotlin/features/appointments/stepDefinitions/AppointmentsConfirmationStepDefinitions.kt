package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsBookingFactory
import features.appointments.factories.AppointmentsBookingFactory.Companion.symptomsToEnter
import features.appointments.factories.AppointmentsBookingFactory.Companion.telephoneNumberToEnter
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AvailableAppointmentFilterSteps
import features.appointments.steps.AvailableAppointmentsSteps
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.FilterSlotDetails
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.assertDoesElementHaveFocus
import pages.assertElementNotPresent
import pages.assertSingleElementPresent
import utils.SerenityHelpers

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps
    @Steps
    lateinit var availableAppointmentsFilterSteps: AvailableAppointmentFilterSteps
    @Steps
    lateinit var availableAppointmentsSteps: AvailableAppointmentsSteps

    @Given("^I have selected an appointment slot to book$")
    fun givenIHaveSelectedAnAppointmentSlotToBook() {
        availableAppointmentsFilterSteps.selectOptionsToRevealSlots()
        val targetSlotDetails = sessionVariableCalled<FilterSlotDetails>(
                AppointmentSessionVariableKeys.APPOINTMENT_TO_SELECT
        )
        availableAppointmentsSteps.availableAppointmentsPage.selectSlot(
                targetSlotDetails.dateAsUIString,
                targetSlotDetails.timeAsUIString,
                targetSlotDetails.sessionName
        )
    }

    @Given("^I wish to book a telephone appointment using my (\\w+) phone number$")
    fun iWishToBookATelephoneAppointmentUserMyPhoneNumberOfType(phoneNumberToSelect: String) {
        val patient = SerenityHelpers.getPatient()
        when (phoneNumberToSelect) {
            "first" ->
                SerenityHelpers.setSerenityVariableIfNotAlreadySet(AppointmentsConfirmationSteps.SerenityVariable
                        .TELEPHONE_NUMBER_TO_BOOK_AGAINST, patient.telephoneFirst)
            "second" ->
                SerenityHelpers.setSerenityVariableIfNotAlreadySet(AppointmentsConfirmationSteps.SerenityVariable
                        .TELEPHONE_NUMBER_TO_BOOK_AGAINST, patient.telephoneSecond)
            else -> Assert.fail("Invalid phone number type. ")
        }
    }

    @Given("^I will manually enter this phone number$")
    fun iWillManuallyEnterThisPhoneNumber() {
        val telephoneNumberToEnter = Serenity.sessionVariableCalled<String>(AppointmentsConfirmationSteps
                .SerenityVariable.TELEPHONE_NUMBER_TO_BOOK_AGAINST)
        Assert.assertNotNull(
                "No phone number has been referenced, so don't know what telephone number will be entered. ",
                telephoneNumberToEnter
        )
        Serenity.setSessionVariable(AppointmentsBookingFactory.telephoneNumberToEnter).to(telephoneNumberToEnter)
    }

    @Given("^I have selected a telephone appointment slot to book$")
    fun givenIHaveSelectedATelephoneAppointmentSlotToBook() {
        givenIHaveSelectedAnAppointmentSlotToBook()
    }

    @When("^I click the error page back button$")
    fun whenIClickTheErrorPageBackButton() {
        appointmentsConfirmationSteps.errorPage.button.click()
    }

    @When("^I enter symptoms$")
    fun whenIEnterSymptoms() {
        val symptoms = Serenity.sessionVariableCalled<String>(symptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeSymptoms(symptoms)
    }

    @When("^I select the (\\w+) number from available ones$")
    fun iSelectPhoneNumberFromAvailableOnes(phoneNumberToSelect: String) {
        val patient = SerenityHelpers.getPatient()
        when(phoneNumberToSelect) {
            "first" ->
                appointmentsConfirmationSteps.appointmentsConfirmation
                        .selectPhoneNumberRadioButtonByText(patient.telephoneFirst)
            "second" ->
                appointmentsConfirmationSteps.appointmentsConfirmation
                        .selectPhoneNumberRadioButtonByText(patient.telephoneSecond)
            else -> Assert.fail("Invalid phone number type. ")
        }

        appointmentsConfirmationSteps.checkOnlyOnePhoneNumberRadioButtonIsSelected()
    }

    @When("^I select the radio button for an alternative phone number to those stored$")
    fun iSelectAlternativePhoneNumberOption() {
        appointmentsConfirmationSteps.appointmentsConfirmation.selectRadioButtonForAlternativePhoneNumber()
        appointmentsConfirmationSteps.checkOnlyOnePhoneNumberRadioButtonIsSelected()
    }

    @When("^I alternate between the different number options from available ones$")
    fun iSelectAlternatePhoneNumberOptions(list: List<String>) {
        for (option in list) {
            when (option) {
                "first", "second" -> iSelectPhoneNumberFromAvailableOnes(option)
                "alternative" -> iSelectAlternativePhoneNumberOption()
                else -> Assert.fail("Invalid phone number option: $option")
            }

        }
    }

    @When("^I enter a phone number for the appointment$")
    fun whenIEnterAPhoneNumberForTheAppointment() {
        val telephoneNumber = Serenity.sessionVariableCalled<String>(telephoneNumberToEnter)
        Assert.assertNotNull("Expected telephone number to be set, incorrect test setup", telephoneNumber)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeTelephoneNumber(telephoneNumber)
    }

    @When("^I enter whitespace instead of a phone number for the appointment$")
    fun whenIEnterWhitespaceInsteadOfAPhoneNumberForTheAppointment() {
        val telephoneNumber = "  "
        Assert.assertNotNull("Expected telephone number to be set, incorrect test setup", telephoneNumber)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeTelephoneNumber(telephoneNumber)
    }

    @Then("^only the first (\\d+) characters will be displayed$")
    fun thenOnlyTheFirstCharactersWillBeDisplayed(length: Int) {
        appointmentsConfirmationSteps.checkSymptomsLength(length)
    }

    @Then("^I see appropriate information message after 10 seconds when it times-out on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageAfterSecondsWhenItTimesOutOnAppointmentConfirmationPage() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message " +
            "when there is an error sending data on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageWhenThereIsAnErrorSendingDataOnAppointmentConfirmationPage() {
        appointmentsConfirmationSteps.appointmentsConfirmation.locatorMethods.waitForNativeStepToComplete()
        appointmentsConfirmationSteps.checkErrorSendingMessage()
    }

    @Then("^there should be an action to go back to my appointments$")
    fun thenThereShouldBeAButtonToGoBackToMyAppointments() {
        appointmentsConfirmationSteps.checkIfActionIsVisible("Back to my appointments")
    }

    @Then("^an error is displayed that \"Describe your symptoms\" is mandatory$")
    fun thenAnErrorIsDisplayedThatIsMandatory() {
        appointmentsConfirmationSteps.checkValidationErrorMessage()
    }

    @Then("^I don't see option to type in booking reason$")
    fun thenIDontSeeOptionToTypeInBookingReason() {
        appointmentsConfirmationSteps.appointmentsConfirmation.reasonFormField.assertElementNotPresent()
    }

    @Then("^a message is displayed indicating that user has reached maximum appointment limit$")
    fun aMessageIsDisplayedInformingTheAppointmentLimitReached() {
        appointmentsConfirmationSteps.verifyThatAppointmentLimitReachedErrorDisplayed()
    }

    @Then("^I do not see a text input to enter phone number$")
    fun iDoNotSeeTextInputToEnterPhoneNumber() {
        appointmentsConfirmationSteps.appointmentsConfirmation.telephoneNumberDiv.assertElementNotPresent()
    }

    @Then("^I do not see any phone numbers to select$")
    fun iDoNotSeeAnyPhoneNumbersToSelect() {
        appointmentsConfirmationSteps.appointmentsConfirmation.radioButtons.assertElementNotPresent()
    }

    @Then("^I see a text input to enter phone number$")
    fun iSeeATextInputToEnterPhoneNumber() {
        appointmentsConfirmationSteps.appointmentsConfirmation.telephoneNumberDiv.assertSingleElementPresent()
    }

    @Then("^the radio button for an alternate phone number remains selected$")
    fun theRadioButtonForAlternatePhoneNumberRemainsSelected() {
        appointmentsConfirmationSteps.appointmentsConfirmation.assertRadioButtonForAlternativePhoneNumberIsSelected()
    }

    @Then("^the phone number text field is not displayed$")
    fun iDoNotSeeATextInputToEnterPhoneNumber() {
        appointmentsConfirmationSteps.appointmentsConfirmation.telephoneNumberDiv.assertElementNotPresent()
    }

    @Then("^the focus will go back to empty phone number input box$")
    fun theFocusWillGoBackToEmptyPhoneNumberInputbox() {
        appointmentsConfirmationSteps.appointmentsConfirmation.telephoneNumberDiv.assertDoesElementHaveFocus()
    }

    @Then("^the focus will go back to empty booking reason input box$")
    fun theFocusWillGoBackToEmptyBookingReasonInputbox() {
        appointmentsConfirmationSteps.appointmentsConfirmation.reasonFormField.assertDoesElementHaveFocus()
    }

    @Then("^a message is displayed indicating a phone number is required$")
    fun thenAMessageIsDisplayedIndicatingAPhoneNumberIsRequired() {
        appointmentsConfirmationSteps.checkTelephoneNumberRequiredErrorMessage()
    }

    @Then("^I see radio buttons to select the user's telephone numbers$")
    fun iSeeRadioButtonsToSelectUsersTelephoneNumbers() {
        val usersPhoneNumbers = ArrayList<String>()
        val patient = SerenityHelpers.getPatient()
        if (!patient.telephoneFirst.isNullOrEmpty())
            usersPhoneNumbers.add(patient.telephoneFirst)
        if (!patient.telephoneSecond.isNullOrEmpty())
            usersPhoneNumbers.add(patient.telephoneSecond)
        appointmentsConfirmationSteps.checkRadioButtonsDisplayedForPhoneNumbers(usersPhoneNumbers)
    }

    @Then("^I see a radio button to select an alternate number$")
    fun iSeeRadioButtonsToSelectAlternateNumber() {
        appointmentsConfirmationSteps.appointmentsConfirmation.assertRadioButtonDisplayedForAlternateNumber()
    }

    @Then("^none of available phone numbers are selected$")
    fun noneOfAvailablePhoneNumbersAreSelected() {
        appointmentsConfirmationSteps.checkNoPhoneNumberRadioButtonsAreSelected()
    }
}
