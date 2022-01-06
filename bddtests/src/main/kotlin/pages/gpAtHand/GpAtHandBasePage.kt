package pages.gpAtHand

import pages.HybridPageObject
import pages.navigation.WebHeader
import pages.sharedElements.expectedPage.ExpectedPageStructure

abstract class GpAtHandBasePage: HybridPageObject() {

    private lateinit var webHeader: WebHeader
    abstract val headerContext: String
    abstract val paragraphContext: String


    fun isLoaded() {
        webHeader.waitForPageHeaderText("Service unavailable")
    }

    fun assertGpAtHandPageVisible() {
        val expected = ExpectedPageStructure()
                .h2("You cannot $headerContext through the NHS App")
                .paragraph("To $paragraphContext with Babylon GP at Hand, use the Babylon app.")
                .paragraph("Call Babylon GP at Hand on 0330 808 2217 if you have any problems.")
        expected.assert(this)
    }
}
