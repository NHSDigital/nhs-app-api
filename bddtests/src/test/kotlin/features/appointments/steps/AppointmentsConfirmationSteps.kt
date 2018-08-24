package features.appointments.steps

import mocking.defaults.MockDefaults
import mocking.MockingClient
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.appointments.AppointmentsConfirmationPage
import java.time.Duration

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

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
    fun mockEmisSuccessResponse() {
        //accept all requests
        mockEmisSuccessResponseDelayedBy(0)
    }

    @Step
    fun mockEmisSuccessResponseDelayedBy(delayedBy: Long) {
        //accept all requests
        mockingClient
                .forEmis {
                    bookAppointmentSlotRequest(patient, BookAppointmentSlotFacade(patient.userPatientLinkToken, 123, "Reason"))
                            .respondWithSuccess()
                            .delayedBy(Duration.ofSeconds(delayedBy))
                }
    }


    @Step
    fun checkValidationErrorMessage() {
        val message = appointmentsConfirmation.getInlineValidationError()
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
        checkErrorSendingMessage()
    }

    @Step
    fun checkErrorSendingMessage() {
        val errorHeading = "Sorry, there's been a problem sending your request"
        val errorSubHeading = "Please go back and try again."
        val errorText = "If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly."
        assertEquals("Expected heading text $errorHeading but found ${errorPage.heading.element.text}",
                errorHeading, errorPage.heading.element.text )
        assertEquals("Expected sub-heading text $errorSubHeading but found ${errorPage.subHeading.element.text}",
                errorSubHeading, errorPage.subHeading.element.text )
        assertEquals("Expected heading text $errorText but found ${errorPage.errorText1.element.text}",
                errorText, errorPage.errorText1.element.text )
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
