package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/booking-success")
class AppointmentBookingSuccessPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(patientName + "\'s GP appointment has been booked")
    }
}
