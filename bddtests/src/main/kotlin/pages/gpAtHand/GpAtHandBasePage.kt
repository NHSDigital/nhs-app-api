package pages.gpAtHand

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

abstract class GpAtHandBasePage: HybridPageObject() {

    private lateinit var headerNative: HeaderNative
    abstract val headerContext: String
    abstract val paragraphContext: String


    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertGpAtHandPageVisible() {
        val expected = ExpectedPageStructure()
                .h2("Sorry, you cannot $headerContext through the NHS App")
                .paragraph("To $paragraphContext with Babylon GP at Hand, please use the Babylon app.")
                .paragraph("Please contact Babylon GP at Hand on 0330 808 2217 or gpathand@nhs.net " +
                        "if you have any problems.")
        expected.assert(this)
    }
}