package features.appointments.steps

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.appointments.AppointmentGuidancePage
import pages.navigation.HeaderNative

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Check before you book"
    private val expectedContentHeader = "Want to avoid waiting?"
    private val expectedGuidanceLinesAndIfInBold = arrayListOf(
            Pair("Three things to try before you book an appointment:", false),
            Pair("1. Self care", true),
            Pair("Many minor problems can be treated at home, for example through rest or appropriate over-the-counter medicines", false),
            Pair("2. Check your symptoms", true),
            Pair("Using trusted NHS online information", false),
            Pair("3. Get advice from a pharmacist", true),
            Pair("They're highly skilled healthcare professionals who can offer valuable advice", false)
    )

    lateinit var appointmentGuidancePage: AppointmentGuidancePage
    lateinit var headerNative: HeaderNative

    @Step
    fun checkThePageHeaderIsCorrect() {
        headerNative.waitForPageHeaderText(expectedPageHeader)
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
        appointmentGuidancePage.bookButton.element.waitUntilVisible<WebElementFacade>()
        appointmentGuidancePage.bookButton.click()
    }

    @Step
    fun clickCheckSymptomsButton() {
        appointmentGuidancePage.checkSymptomsButton.click()
    }
}