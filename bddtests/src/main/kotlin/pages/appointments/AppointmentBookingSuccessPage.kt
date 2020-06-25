package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-success")
class AppointmentBookingSuccessPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    val appointmentDetails = HybridPageElement(
            webDesktopLocator = "//*[@id='appointmentDetails']",
            androidLocator = null,
            page = this
    )
    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(patientName + "\'s GP appointment has been booked")
    }

    fun appointmentDetailsNotShown() {
        appointmentDetails.assertElementNotPresent()
    }
}
