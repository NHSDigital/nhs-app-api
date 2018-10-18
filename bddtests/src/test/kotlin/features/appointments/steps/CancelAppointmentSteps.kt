package features.appointments.steps

import features.appointments.factories.UpcomingAppointmentsFactory
import features.authentication.steps.LoginSteps
import features.sharedSteps.NavigationSteps
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.appointments.CancelAppointmentPage

open class CancelAppointmentSteps {

    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps

    lateinit var cancelAppointmentPage: CancelAppointmentPage

    @Step
    fun verifyWeAreOnTheCancelAppointmentScreen() {
        val expectedHeader = "Cancel appointment"
        cancelAppointmentPage.waitForPageHeaderText(expectedHeader)
        assertEquals("Check your appointment details before cancelling",
                cancelAppointmentPage.getCheckDetailsText())
    }

    @Step
    fun verifyTheCorrectAppointmentDetailsAreDisplayed() {
        val expectedSlot = retrieveSlotOfAppointmentToCancel()
        assertEquals("Date", expectedSlot.date, cancelAppointmentPage.selectedAppointmentDate.element.text)
        assertEquals("Time", expectedSlot.time, cancelAppointmentPage.selectedAppointmentTime.element.text)
        assertEquals("Session Name", expectedSlot.session,
                cancelAppointmentPage.selectedAppointmentSessionName.element.text)
        assertEquals("location", expectedSlot.location,
                cancelAppointmentPage.selectedAppointmentLocation.element.text)
        assertEquals(expectedSlot.clinicians, cancelAppointmentPage.getSelectedAppointmentClinicianText())
    }

    @Step
    fun verifyTheDropDownMenuLabel() {
        assertEquals("Reason for cancelling", cancelAppointmentPage.getReasonDropDownLabelText())
    }

    @Step
    fun verifyTheDropDownMenuLabelIsNotVisible() {
        assertFalse("Drop-Down menu is visible", cancelAppointmentPage.containsDropDownMenu())
    }

    @Step
    fun verifyTheValidationErrorSummary() {

        assertEquals("There's a problem", cancelAppointmentPage.errorBanner.subHeading)
        assertEquals("Select a reason for cancelling", cancelAppointmentPage.errorBanner.bodyElements[0])
    }

    @Step
    fun verifyTheInlineReasonValidationError() {
        assertEquals("Select a reason for cancelling", cancelAppointmentPage.inLineError.element.text)
    }

    @Step
    fun selectReason(reason: String) {
        assertTrue("$reason is not available as a Cancellation Reason. ", cancelAppointmentPage.selectReason(reason))
    }

    @Step
    fun retrieveSlotOfAppointmentToCancel(): Slot {
        return Serenity.sessionVariableCalled<List<Slot>>(
                UpcomingAppointmentsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_MY_UPCOMING_APPOINTMENTS
        ).first()
    }
}
