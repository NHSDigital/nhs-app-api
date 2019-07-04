package pages.gpAtHand

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.TextBlockElement

class GpAtHandMedicalRecordPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertGpAtHandMedicalRecordPageVisible() {
        TextBlockElement.withH2Header("Sorry, you cannot access your medical record through the NHSApp", this)
                .assert("To view your medical record with Babylon GP at Hand, please use the Babylon app.")
                .assert("Please contact Babylon GP at Hand on 0330 808 2217 or gpathand@nhs.net " +
                        "if you have any problems.")
    }
}