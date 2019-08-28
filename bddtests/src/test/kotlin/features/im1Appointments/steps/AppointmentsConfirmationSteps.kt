package features.im1Appointments.steps

import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import mocking.MockingClient
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.ErrorPage
import pages.appointments.AppointmentsConfirmationPage
import pages.assertElementNotPresent
import pages.isActionVisible
import pages.text

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage
    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance

    @Step
    fun checkValidationErrorMessage() {
        val message = appointmentsConfirmation.reasonError.text
        assertEquals("Enter a reason for this appointment", message)
    }

    @Step
    fun checkTelephoneNumberRequiredErrorMessage() {
        val message = appointmentsConfirmation.telephoneError.text
        assertEquals("Enter a telephone number", message)
    }

    @Step
    fun checkAppointmentDetails() {
        val expectedSlot =  Serenity.sessionVariableCalled<Slot>(AppointmentsBookingFactory.selectedSlot)
                .copy(id = null)

        val areCliniciansExpected = expectedSlot.clinicians.isNotEmpty()
        val actualSlot = appointmentsConfirmation.getAppointmentSlot(areCliniciansExpected)
        assertEquals("Exact expected Appointment not found. ", expectedSlot, actualSlot)
    }

    @Step
    fun checkSymptomsLength(expectedLength: Int) {
        val expectedSymptoms = Serenity.sessionVariableCalled<String>(AppointmentsBookingFactory.symptomsToEnter)
                .slice(0 until expectedLength)
        assertEquals(
                "Displayed symptoms of unexpected length. ",
                appointmentsConfirmation.getSymptoms().length,
                expectedLength
        )
        assertEquals(
                "Symptoms displayed incorrectly. ",
                expectedSymptoms,
                appointmentsConfirmation
                        .getSymptoms()
        )
    }

    @Step
    fun checkTimeoutErrorMessage() {
        checkErrorSendingMessage()
    }

    @Step
    fun checkErrorSendingMessage() {
        val errorHeading = "There's been a problem sending your request"
        val errorText = "Go back and try again. If the problem continues and you need to book " +
                "or cancel an appointment now, contact your GP surgery directly. For urgent medical advice, call 111."
        assertEquals("Expected heading text $errorHeading but found ${errorPage.heading.text}",
                errorHeading, errorPage.heading.text)
        errorPage.subHeading.assertElementNotPresent()
        assertEquals("Expected error text $errorText but found ${errorPage.errorText1.text}",
                errorText, errorPage.errorText1.text)
    }

    @Step
    fun verifyThatAppointmentLimitReachedErrorDisplayed() {
        val expectedPageHeader = "Appointment limit reached"
        val expectedHeader = "You can't book any more appointments right now"
        errorPage.assertPageHeader(expectedPageHeader)
        errorPage.assertHeaderText(expectedHeader)
    }

    @Step
    fun checkIfActionIsVisible(action: String) {
        val isVisible = appointmentsConfirmation.isActionVisible(action)
        assertTrue(isVisible)
    }

    @Step
    fun checkRadioButtonsDisplayedForPhoneNumbers(usersPhoneNumbers: ArrayList<String>) {
        for (phoneNumber in usersPhoneNumbers) {
            appointmentsConfirmation.assertRadioButtonDisplayedForPhoneNumber(phoneNumber)
        }
    }

    @Step
    fun checkNoPhoneNumberRadioButtonsAreSelected() {
        checkNumberOfPhoneNumberRadioButtonsSelected(0)
    }

    @Step
    fun checkOnlyOnePhoneNumberRadioButtonIsSelected() {
        checkNumberOfPhoneNumberRadioButtonsSelected(1)
    }

    private fun checkNumberOfPhoneNumberRadioButtonsSelected(expectedNumberOfRadioButtonsSelected: Int) {
        assertEquals(
                "Incorrect number of radio buttons selected. ",
                expectedNumberOfRadioButtonsSelected,
                appointmentsConfirmation.getNumberOfSelectedPhoneNumberRadioButtons()
        )
    }

    enum class SerenityVariable {
        TELEPHONE_NUMBER_TO_BOOK_AGAINST
    }
}
