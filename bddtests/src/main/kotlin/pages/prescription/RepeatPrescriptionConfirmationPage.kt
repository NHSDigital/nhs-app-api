package pages.prescription

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.navigation.HeaderNative

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/order-success")
open class RepeatPrescriptionConfirmationPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(patientName + "\'s prescription has been ordered")
    }
}