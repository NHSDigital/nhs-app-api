package features.appointments.steps

import mocking.defaults.MockDefaults
import mocking.MockingClient
import mockingFacade.appointments.BookAppointmentSlotFacade
import net.serenitybdd.core.Serenity.setSessionVariable
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.appointments.AppointmentsConfirmationPage
import worker.models.appointments.BookAppointmentSlotRequest
import java.time.Duration

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance
    val patient = MockDefaults.patient

    @Step
    fun clickOnConfirmAndBookAppointmentButton() {
        appointmentsConfirmation.clickOnConfirmAndBookAppointmentButton()
    }

    @Step
    fun goBackToMyAppointments() {
        appointmentsConfirmation.backToMyAppointmentsButton.element.click()
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
        setSessionVariable("Symptoms").to(symptoms)
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
        assertTrue(
                "Sub-heading is incorrect. ",
                errorPage.hasSubHeading("Sorry, there's been a problem sending your request")
        )
        assertTrue(
                "First part of the message is incorrect. ",
                errorPage.hasDetailParagraphOne("Please go back and try again.")
        )
        assertTrue(
                "Second part of the message is incorrect. ",
                errorPage.hasDetailParagraphTwo("If the problem persists and you need to book or cancel an appointment now, contact your GP surgery directly.")
        )
    }

    @Step
    fun pasteSymptoms(length: Int) {
        appointmentsConfirmation.pasteSymptoms("x".repeat(length))
    }

    @Step
    fun checkIfButtonIsVisible(button: String) {
        val isVisible = appointmentsConfirmation.isButtonVisible(button)
        Assert.assertTrue(isVisible)
    }
}
