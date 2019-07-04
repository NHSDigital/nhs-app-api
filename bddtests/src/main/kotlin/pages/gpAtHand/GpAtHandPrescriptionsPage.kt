package pages.gpAtHand

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.TextBlockElement

class GpAtHandPrescriptionsPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertGpAtHandPrescriptionsPageVisible() {
        TextBlockElement.withH2Header("Sorry, you cannot order prescriptions through the NHS App", this)
                .assert("To order prescriptions with Babylon GP at Hand, please use the Babylon app.")
                .assert("Please contact Babylon GP at Hand on 0330 808 2217 or gpathand@nhs.net " +
                        "if you have any problems.")
    }
}