package features.appointments.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.appointments.AppointmentGuidancePage

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Check before you book"
    private val expectedContentHeader = "Want to avoid waiting?"
    private val expectedGuidanceLinesAndIfInBold = arrayListOf(
            Pair("If it isn't urgent, you can try three things before booking an appointment:", false),
            Pair("1. Self care", true),
            Pair("Many minor problems can be treated at home, for example through rest or appropriate over-the-counter medicines", false),
            Pair("2. Check your symptoms", true),
            Pair("Using trusted NHS online information", false),
            Pair("3. Get advice from a pharmacist", true),
            Pair("They're highly skilled healthcare professionals who can offer valuable advice", false)
    )

    lateinit var appointmentGuidancePage: AppointmentGuidancePage

    @Step
    fun checkThePageHeaderIsCorrect() {
        val actualHeader = appointmentGuidancePage.getPageHeaderText()
        assertEquals("Page header for the appointment guidance is not matching",
                expectedPageHeader, actualHeader)
    }

    @Step
    fun checkTheContentHeaderIsCorrect() {
        assertTrue("Appointment guidance content header $expectedContentHeader is not found",
                appointmentGuidancePage.isSubHeaderTextEqualTo(expectedContentHeader))
    }

    @Step
    fun checkGuidanceItemsHeadersAreCorrect() {
        val actualGuidanceItems = appointmentGuidancePage.getGuidanceBody()
        assertEquals("Expected appointment guidance items are not found",
                expectedGuidanceLinesAndIfInBold,
                actualGuidanceItems)
    }

    @Step
    fun clickBookAnAppointmentButton() {
        appointmentGuidancePage.bookButton.element.click()
    }

    @Step
    fun clickCheckSymptomsButton() {
        appointmentGuidancePage.checkSymptomsButton.element.click()
    }
}