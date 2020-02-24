package features.im1Appointments.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.appointments.AppointmentGuidancePage
import pages.navigation.HeaderNative
import pages.navigation.WebHeader

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Things to try before you book an appointment"

    lateinit var appointmentGuidancePage: AppointmentGuidancePage
    lateinit var headerNative: HeaderNative
    lateinit var webHeader: WebHeader

    @Step
    fun checkThePageHeaderIsCorrect() {
        webHeader.getPageTitle().withText(expectedPageHeader)
    }

    @Step
    fun checkGuidanceItemsAreCorrectOLCEnabled() {
        val hasGuidanceItems = appointmentGuidancePage.checkGuidanceBodyForOnlineConsultations()
        Assert.assertTrue("Expected appointment guidance items are not found", hasGuidanceItems)
    }
}
