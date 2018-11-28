package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsBookingFactory.Companion.SymptomsToEnter
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentDateKey
import features.appointments.factories.AppointmentsFactory.Companion.TargetAppointmentTimeKey
import features.appointments.steps.AppointmentsConfirmationSteps
import features.appointments.steps.AvailableAppointmentFilterSteps
import features.appointments.steps.AvailableAppointmentsSteps
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Steps
import org.junit.Assert

class AppointmentsConfirmationStepDefinitions {

    @Steps
    lateinit var availableAppointmentsFilterSteps: AvailableAppointmentFilterSteps
    @Steps
    lateinit var availableAppointmentsSteps: AvailableAppointmentsSteps
    @Steps
    lateinit var appointmentsConfirmationSteps: AppointmentsConfirmationSteps

    @Given("^I have selected an appointment slot to book$")
    fun givenIHaveSelectedAnAppointmentSlotToBook() {
        availableAppointmentsFilterSteps.selectOptionsToRevealSlots()
        val date = sessionVariableCalled<String>(TargetAppointmentDateKey)
        val time = sessionVariableCalled<String>(TargetAppointmentTimeKey)
        availableAppointmentsSteps.availableAppointmentsPage.selectSlot(date, time)
    }
    @When("^I click the button to go back to my appointments$")
    fun whenIClickTheButtonToGoBackToMyAppointments() {
        appointmentsConfirmationSteps.goBackToMyAppointments()
    }

    @When("^I click the error page back button$")
    fun whenIClickTheErrorPageBackButton() {
        appointmentsConfirmationSteps.clickErrorPageBackButton()
    }

    @When("^I enter symptoms of (\\d+) characters$")
    fun whenIEnterSymptomsOfCharacter(length: Int) {
        val symptoms: String = getSymptomsOfLength(length)
        appointmentsConfirmationSteps.describeSymptoms(symptoms)
    }

    @When("^I enter symptoms$")
    fun whenIEnterSymptoms() {
        val symptoms = Serenity.sessionVariableCalled<String>(SymptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        appointmentsConfirmationSteps.describeSymptoms(symptoms)
    }

    @When("^I paste symptoms of (\\d+) characters$")
    fun whenIPasteSymptomsOfCharacters(length: Int) {
        val symptoms: String = getSymptomsOfLength(length)
        appointmentsConfirmationSteps.pasteSymptoms(symptoms)
    }

    @Then("^only the first (\\d+) characters will be displayed$")
    fun thenOnlyTheFirstCharactersWillBeDisplayed(length: Int) {
        appointmentsConfirmationSteps.checkSymptomsLength(length)
    }

    @Then("^I see appropriate information message when there is an error on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageWhenThereIsAnErrorOnAppointmentConfirmationPage() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message after 10 seconds when it times-out on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageAfterSecondsWhenItTimesOutOnAppointmentConfirmationPage() {
        appointmentsConfirmationSteps.checkTimeoutErrorMessage()
    }

    @Then("^I see appropriate information message " +
            "when there is an error sending data on appointment confirmation page$")
    fun thenISeeAppropriateInformationMessageWhenThereIsAnErrorSendingDataOnAppointmentConfirmationPage() {
        appointmentsConfirmationSteps.checkErrorSendingMessage()
    }

    @Then("^there should be a button to go back to my appointments$")
    fun thenThereShouldBeAButtonToGoBackToMyAppointments() {
        appointmentsConfirmationSteps.checkIfButtonIsVisible("Back to my appointments")
    }

    @Then("^an error is displayed that \"Describe your symptoms\" is mandatory$")
    fun thenAnErrorIsDisplayedThatIsMandatory() {
        appointmentsConfirmationSteps.checkValidationErrorMessage()
    }

    @Then("^I don't see option to type in booking reason$")
    fun thenIDontSeeOptionToTypeInBookingReason() {
        appointmentsConfirmationSteps.appointmentsConfirmation.symptomsFormDiv.assertElementNotPresent()
    }

    @Then("^a message is displayed indicating that user has reached maximum appointment limit$")
    fun aMessageIsDisplayedInformingTheAppointmentLimitReached() {
        appointmentsConfirmationSteps.verifyThatAppointmentLimitReachedErrorDisplayed()
    }

    private fun getSymptomsOfLength(length: Int): String {
        val symptoms = Serenity.sessionVariableCalled<String>(SymptomsToEnter)
        Assert.assertNotNull("Expected symptoms to be set, incorrect test setup", symptoms)
        Assert.assertEquals("Expected number of characters in symptoms, incorrect test setup", length, symptoms.length)
        return symptoms
    }
}
