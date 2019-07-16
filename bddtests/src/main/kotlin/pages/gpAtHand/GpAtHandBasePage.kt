package pages.gpAtHand

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.TextBlockElement

abstract class GpAtHandBasePage: HybridPageObject() {

    private lateinit var headerNative: HeaderNative
    abstract val headerContext: String
    abstract val paragraphContext: String


    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertGpAtHandPageVisible() {
        TextBlockElement.withH2Header("Sorry, you cannot $headerContext through the NHS App", this)
                .assert("To $paragraphContext with Babylon GP at Hand, please use the Babylon app.")
                .assert("Please contact Babylon GP at Hand on 0330 808 2217 or gpathand@nhs.net " +
                        "if you have any problems.")
    }
}