package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.WebHeader

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/cancelling-success")
class AppointmentCancellationSuccessPage : HybridPageObject() {

    private lateinit var webHeader: WebHeader

    fun isLoaded(patientName: String) {
        webHeader.waitForPageHeaderText(patientName + "\'s GP appointment has been cancelled")
    }
}
