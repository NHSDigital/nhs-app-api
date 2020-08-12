package features.im1Appointments.stepDefinitions

import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.im1Appointments.steps.AppointmentSerenityHelpers
import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import features.im1Appointments.steps.AppointmentsConfirmationSteps
import features.im1Appointments.steps.AvailableAppointmentFilterSteps
import features.im1Appointments.steps.AvailableAppointmentsSteps
import mocking.data.appointments.AppointmentSessionVariableKeys
import mocking.data.appointments.FilterSlotDetails
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.ErrorDialogPage
import pages.assertDoesElementHaveFocus
import pages.assertElementNotPresent
import pages.assertSingleElementPresent
import pages.avoidChromeWebDriverServiceCrash
import utils.getOrFail

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps
    @Steps
    lateinit var availableAppointmentsFilterSteps: AvailableAppointmentFilterSteps
    @Steps
    lateinit var availableAppointmentsSteps: AvailableAppointmentsSteps

    private lateinit var errorDialogPage: ErrorDialogPage

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

    @Given("^I have selected a telephone appointment slot to book$")
    fun givenIHaveSelectedATelephoneAppointmentSlotToBook() {
        givenIHaveSelectedAnAppointmentSlotToBook()
    }

    @When("^I enter symptoms$")
    fun whenIEnterSymptoms() {
        val symptoms = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.symptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeSymptoms(symptoms)
    }

    @When("^I select a telephone number to book an appointment$")
    fun iSelectATelephoneNumberToBookAnAppointment() {
        val targetNumber = AppointmentSerenityHelpers.TELEPHONE_NUMBER_TO_BOOK_AGAINST.getOrFail<String>()
        appointmentsConfirmationSteps.appointmentsConfirmation
                .selectPhoneNumberRadioButtonByText(targetNumber)
    }

    @When("^I select the radio button for an alternative phone number to those stored$")
    fun iSelectAlternativePhoneNumberOption() {
        appointmentsConfirmationSteps.appointmentsConfirmation.selectRadioButtonForAlternativePhoneNumber()
        appointmentsConfirmationSteps.checkOnlyOnePhoneNumberRadioButtonIsSelected()
    }

    @When("^I enter a phone number for the appointment$")
    fun whenIEnterAPhoneNumberForTheAppointment() {
        val telephoneNumber = AppointmentSerenityHelpers.TELEPHONE_NUMBER_TO_BOOK_AGAINST.getOrFail<String>()
        Assert.assertNotNull("Expected telephone number to be set, incorrect test setup", telephoneNumber)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeTelephoneNumber(telephoneNumber)
    }

    @When("^I enter whitespace instead of a phone number for the appointment$")
    fun whenIEnterWhitespaceInsteadOfAPhoneNumberForTheAppointment() {
        val telephoneNumber = "  "
        Assert.assertNotNull("Expected telephone number to be set, incorrect test setup", telephoneNumber)
        appointmentsConfirmationSteps.appointmentsConfirmation.describeTelephoneNumber(telephoneNumber)
    }

    @Then("^I see appropriate information message " +
            "when there is an error sending data on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageWhenThereIsAnErrorSendingDataOnAppointmentConfirmationPage() {
        errorDialogPage.avoidChromeWebDriverServiceCrash()
        errorDialogPage.assertPageHeader(appointmentsConfirmationSteps.appointmentsConfirmation.problemHeader)
                .assertPageTitle(appointmentsConfirmationSteps.appointmentsConfirmation.problemTitle)
                .assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.goBackAndTryAgainProblem)
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
        errorDialogPage.assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.cannotBook)
                .assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.contactGp)
                .assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.cancelNoLongerNeeded)
                .assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.urgentAdvice)
                .assertPageTitle(appointmentsConfirmationSteps.appointmentsConfirmation.reachedLimitTitle)
                .assertPageHeader(appointmentsConfirmationSteps.appointmentsConfirmation.reachedLimitTitle)

    }

    @Then("^a message is displayed indicating that the slot has already been taken$")
    fun aMessageIsDisplayedInformingTheSlotHasAlreadyBeenTaken() {
        errorDialogPage.assertParagraphText(appointmentsConfirmationSteps.appointmentsConfirmation.chooseDifferent)
                .assertPageHeader(appointmentsConfirmationSteps.appointmentsConfirmation.notAvailableTitle)
                .assertPageTitle(appointmentsConfirmationSteps.appointmentsConfirmation.notAvailableTitle)
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
}
