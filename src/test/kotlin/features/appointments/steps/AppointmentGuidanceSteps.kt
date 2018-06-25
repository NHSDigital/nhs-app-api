package features.appointments.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import pages.appointments.AppointmentGuidancePage

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Check before you book"
    private val expectedContentHeader = "Want to avoid waiting?"
    private val expectedListHeaders = arrayListOf(
            "Self care",
            "Check your symptoms",
            "Get advice from a pharmacist")

    lateinit var appointmentGuidancePage: AppointmentGuidancePage

    @Step
    fun checkThePageHeaderIsCorrect() {
        val actualHeader = appointmentGuidancePage.getPageHeaderText()
        assertEquals("Page header for the appointment guidance is not matching",
                expectedPageHeader, actualHeader)
    }

    @Step
    fun checkTheContentHeaderIsCorrect() {
        val actualContentHeader = appointmentGuidancePage.contentHeaderElement.text
        assertEquals("Appointment guidance content header $expectedContentHeader is not found",
                expectedContentHeader,
                actualContentHeader)
    }

    @Step
    fun checkGuidanceItemsHeadersAreCorrect() {
        val actualGuidanceItemsHeaders = appointmentGuidancePage.getListHeaders()
        assertEquals("Expected appointment guidance items are not found",
                expectedListHeaders,
                actualGuidanceItemsHeaders)
    }

    @Step
    fun clickBookAnAppointmentButton() {
        appointmentGuidancePage.clickBookButton()
    }

    @Step
    fun clickCheckSymptomsButton() {
        appointmentGuidancePage.clickSymptomsCheckerButton()
    }
}