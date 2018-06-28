package features.appointments.steps

import mocking.defaults.MockDefaults
import mocking.MockingClient
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import pages.appointments.AppointmentsConfirmationPage
import worker.models.appointments.BookAppointmentSlotRequest
import java.time.Duration

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    @Step
    fun clickOnConfirmAndBookAppointmentButton() {
        appointmentsConfirmation.clickOnConfirmAndBookAppointmentButton()
    }

    @Step
    fun clickOnButton(button: String) {
        appointmentsConfirmation.clickOnButton(button)
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
                .forEmis { bookAppointmentSlotRequest(patient, BookAppointmentSlotRequest(patient.userPatientLinkToken, 123, "Reason"))
                        .respondWithSuccess()
                        .delayedBy(Duration.ofSeconds(delayedBy))
                }
    }

    @Step
    fun mockEmisUnavailableResponse() {
        //accept all requests
        mockingClient
                .forEmis { bookAppointmentSlotRequest(patient, BookAppointmentSlotRequest(patient.userPatientLinkToken, 123, "Reason"))
                        .respondWithUnavailableException()
                }
    }

    @Step
    fun mockEmisConflictesponse() {
        //accept all requests
        mockingClient
                .forEmis { bookAppointmentSlotRequest(patient, BookAppointmentSlotRequest(patient.userPatientLinkToken, 123, "Reason"))
                        .respondWithConflictException()
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
        setSessionVariable("Symptoms").to(symptoms)
    }

    @Step
    fun checkSymptomsLength(expectedLength: Int) {
        Assert.assertTrue(appointmentsConfirmation.getSymptoms().length == expectedLength)
    }

    @Step
    fun checkTimeoutErrorMessage() {
        checkErrorSendingMessage()
    }

    @Step
    fun checkErrorSendingMessage() {
        val message = appointmentsConfirmation.getServerErrorElement()

        Assert.assertTrue(message.text.contains("Sorry, there's been a problem sending your request"))
        Assert.assertTrue(message.text.contains("Please go back and try again."))
        Assert.assertTrue(message.text.contains("If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly."))
    }

    @Step
    fun pasteSymptoms(length: Int)
    {
        appointmentsConfirmation.pasteSymptoms("x".repeat(length))
    }

    @Step
    fun checkIfButtonIsVisible(button: String)
    {
        val isVisible = appointmentsConfirmation.isButtonVisible(button)
        Assert.assertTrue(isVisible)
    }
}
