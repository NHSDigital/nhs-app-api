package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/hospital-appointments/")
open class HospitalAppointmentsPage : HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this,
            helpfulName = "Hospital Appointments Title"
    ).withText("Hospital and other appointments", normalised = true)

    fun assertIsDisplayed() {
        pageTitle.assertIsVisible()
    }
}
