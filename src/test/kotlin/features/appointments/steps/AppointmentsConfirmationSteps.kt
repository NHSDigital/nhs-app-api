package features.appointments.steps

import mocking.defaults.MockDefaults
import mocking.MockingClient
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.AppointmentsConfirmationPage
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
    fun checkValidationErrorMessage() {
        val message = appointmentsConfirmation.getValidationErrorMessage()
        Assert.assertTrue(message.containsText("Please describe your symptoms"))
    }

    @Step
    fun describeSymptoms(symptoms: String) {
        appointmentsConfirmation.describeSymptoms(symptoms)
    }

    @Step
    fun checkSymptomsLength(expectedLength: Int) {
        Assert.assertTrue(appointmentsConfirmation.getSymptoms().length == expectedLength)
    }

    @Step
    fun checkTimeoutErrorMessage() {
        val message = appointmentsConfirmation.getServerErrorElement()
        Assert.assertTrue(message.text.contains("Sorry, we're experiencing technical difficulties"))

    }

    @Step
    fun pasteSymptoms(length: Int)
    {
        appointmentsConfirmation.pasteSymptoms("x".repeat(length))
    }
}
