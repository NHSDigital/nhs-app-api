package features.appointments.steps

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
        val expectedSlot = Serenity.sessionVariableCalled<Slot>(Slot::class.java)
        assertEquals(expectedSlot.date, cancelAppointmentPage.getSelectedAppointmentDateText())
        assertEquals(expectedSlot.time, cancelAppointmentPage.getSelectedAppointmentTimeText())
        assertEquals(expectedSlot.session, cancelAppointmentPage.getSelectedAppointmentSessionNameText())
        assertEquals(expectedSlot.location, cancelAppointmentPage.getSelectedAppointmentLocationText())
        for (i in 0 until expectedSlot.clinician.size) {
            assertEquals(expectedSlot.clinician[i], cancelAppointmentPage.getSelectedAppointmentClinicianTextAtPosition(i + 1))
        }
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
}
