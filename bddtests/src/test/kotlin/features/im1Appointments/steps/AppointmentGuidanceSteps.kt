package features.im1Appointments.steps

import net.thucydides.core.annotations.Step
import pages.appointments.AppointmentGuidancePage
import pages.navigation.WebHeader

open class AppointmentGuidanceSteps {
    private val expectedPageHeader = "Things to try before you book an appointment"

    lateinit var appointmentGuidancePage: AppointmentGuidancePage
    lateinit var webHeader: WebHeader

    @Step
    fun checkThePageHeaderIsCorrect() {
        webHeader.getPageTitle().withText(expectedPageHeader)
    }

    @Step
    fun checkGuidanceItemsAreCorrectOLCEnabled() {
        appointmentGuidancePage.checkGuidanceBodyForOnlineConsultations()
    }
}
