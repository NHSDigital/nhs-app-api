package features.appointments.steps

import mocking.MockingClient
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.*
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
        appointmentsConfirmation.backToMyAppointmentsButton.element.click()
    }

    @Step
    fun clickErrorPageBackButton() {
        errorPage.button.element.click()
    }

    @Step
    fun checkValidationErrorMessage() {
        val message = appointmentsConfirmation.inLineError.element.text
        assertEquals("Enter a reason for this appointment", message)
    }

    @Step
    fun describeSymptoms(symptoms: String) {
        appointmentsConfirmation.describeSymptoms(symptoms)
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
        val errorText = "Go back and try again. If the problem continues and you need to book or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111."
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

        assertEquals("expected Page Header text $expectedPageHeader but found ${errorPage.pageTitle.element.text}",
                expectedPageHeader, errorPage.pageTitle.element.text)
        assertEquals("expected error text $expectedHeader but found ${errorPage.heading.element.text}",
                expectedHeader, errorPage.heading.element.text)
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
