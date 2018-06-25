package features.appointments.steps

import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import models.Slot
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import org.junit.Assert.*
import pages.appointments.CancelAppointmentPage

open class CancelAppointmentSteps {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var appointmentsSteps: AppointmentsSteps

    lateinit var cancelAppointmentPage: CancelAppointmentPage

    @Step
    fun verifyWeAreOnTheCancelAppointmentScreen() {
        assertEquals("Cancel appointment", cancelAppointmentPage.getPageHeaderText())
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
    fun verifyTheReasonIsAvailable(reason: String) {
        assertTrue("$reason is not available as a Cancellation Reason. ", cancelAppointmentPage.isReasonAvailable(reason))
    }

    @Step
    fun progressToAppointmentCancellationScreen() {
        appointmentsSteps.mockEMISMyAppointmentResponse()
        browser.goToApp()
        login.asDefault()
        navigation.select("appointments")
        appointmentsSteps.clickFirstCancelLink()
    }
}
