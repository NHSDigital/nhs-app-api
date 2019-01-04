package features.appointments.steps

import mocking.MockingClient
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.appointments.AppointmentsConfirmationPage

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance

    @Step
    fun clickOnConfirmAndBookAppointmentButton() {
        appointmentsConfirmation.clickOnConfirmAndBookAppointmentButton()
        appointmentsConfirmation.waitForSpinnerToDisappear()
    }

    @Step
    fun goBackToMyAppointments() {
        appointmentsConfirmation.backToMyAppointmentsButton.click()
    }

    @Step
    fun clickErrorPageBackButton() {
        errorPage.button.click()
    }

    @Step
    fun checkValidationErrorMessage() {
        val message = appointmentsConfirmation.reasonError.element.text
        assertEquals("Enter a reason for this appointment", message)
    }

    @Step
    fun checkTelephoneNumberRequiredErrorMessage() {
        val message = appointmentsConfirmation.telephoneError.element.text
        assertEquals("Enter a telephone number", message)
    }

    @Step
    fun describeSymptoms(symptoms: String) {
        appointmentsConfirmation.describeSymptoms(symptoms)
    }

    @Step
    fun describeTelephoneNumber(telephoneNumber: String) {
        appointmentsConfirmation.describeTelephoneNumber(telephoneNumber)
    }

    @Step
    fun checkSymptomsLength(expectedLength: Int) {
        assertTrue(appointmentsConfirmation.getSymptoms().length == expectedLength)
    }

    @Step
    fun checkTimeoutErrorMessage() {
        errorPage.waitForSpinnerToDisappear()
        checkErrorSendingMessage()
    }

    @Step
    fun checkErrorSendingMessage() {
        val errorHeading = "There's been a problem sending your request"
        val errorText = "Go back and try again. If the problem continues and you need to book " +
                "or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111."
        assertEquals("Expected heading text $errorHeading but found ${errorPage.heading.element.text}",
                errorHeading, errorPage.heading.element.text)
        errorPage.subHeading.assertElementNotPresent()
        assertEquals("Expected error text $errorText but found ${errorPage.errorText1.element.text}",
                errorText, errorPage.errorText1.element.text)
    }

    @Step
    fun verifyThatAppointmentLimitReachedErrorDisplayed() {
        val expectedPageHeader = "Appointment limit reached"
        val expectedHeader = "You can't book any more appointments right now"
        errorPage.assertPageHeader(expectedPageHeader)
        errorPage.assertHeaderText(expectedHeader)
    }

    @Step
    fun pasteSymptoms(symptoms: String) {
        appointmentsConfirmation.pasteSymptoms(symptoms)
    }

    @Step
    fun checkIfButtonIsVisible(button: String) {
        val isVisible = appointmentsConfirmation.isButtonVisible(button)
        Assert.assertTrue(isVisible)
    }
}
