package features.im1Appointments.steps

import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import pages.appointments.AppointmentsConfirmationPage
import pages.text

open class AppointmentsConfirmationSteps {

    lateinit var appointmentsConfirmation: AppointmentsConfirmationPage

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
}

