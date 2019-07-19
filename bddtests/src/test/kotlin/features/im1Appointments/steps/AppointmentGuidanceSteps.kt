package features.im1Appointments.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert.assertTrue
import pages.appointments.AppointmentGuidancePage
import pages.isDisplayed
import pages.navigation.HeaderNative

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Things to try before you book an appointment"

    lateinit var appointmentGuidancePage: AppointmentGuidancePage
    lateinit var headerNative: HeaderNative

    @Step
    fun checkThePageHeaderIsCorrect() {
        headerNative.waitForPageHeaderText(expectedPageHeader)
    }

    @Step
    fun checkTheContentIsCorrect() {
        assertTrue(appointmentGuidancePage.menuCheckSymptomsButton.isDisplayed
        && appointmentGuidancePage.bookButton.isDisplayed)
    }

    @Step
    fun clickBookAnAppointmentButton() {
        appointmentGuidancePage.bookButton.click()
    }
}
