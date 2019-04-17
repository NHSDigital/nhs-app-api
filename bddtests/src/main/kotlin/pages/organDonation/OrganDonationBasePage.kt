package pages.organDonation

import pages.HybridPageElement
import pages.HybridPageObject

abstract class OrganDonationBasePage: HybridPageObject() {

    abstract val titleText: String

    protected val title by lazy {
        HybridPageElement(
                "//h2",
                "//h2",
                null,
                null,
                this,
                helpfulName = "header").withText(titleText)
    }

    fun clickContinue() {
        clickOnButtonContainingText("Continue")
    }

    abstract fun assertDisplayed()

    protected fun assertPageFullyLoaded() {
        title.waitForElement()
        HybridPageElement(
                "//button",
                "//button",
                null,
                null,
                this
        ).withText("Back", false).waitForElement()
    }

    protected fun getLink(text: String): HybridPageElement {
        return HybridPageElement(
                "//a[normalize-space() = '$text']",
                page = this)
    }
}
