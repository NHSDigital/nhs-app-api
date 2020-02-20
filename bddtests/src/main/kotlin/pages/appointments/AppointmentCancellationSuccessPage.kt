package pages.appointments

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/appointments/cancelling-success")
class AppointmentCancellationSuccessPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(patientName + "\'s GP appointment has been cancelled")
    }
}
