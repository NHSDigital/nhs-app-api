package pages.organDonation

import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

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
        waitForElement{title.assertIsVisible()}
        waitForElement{assertBackButton("Back")}
    }

    protected fun waitForElement(assertion : () -> Unit) {
        //These pages are throwing stale exceptions when interacting with them
        //By waiting for the back button, we ensure that the page is fully loaded
        var staleElement = true
        while (staleElement) {
            try {
                assertion.invoke()
                staleElement = false
            } catch (e: StaleElementReferenceException) {
                staleElement = true
            }
        }
    }

    private fun assertBackButton(text: String) {
        HybridPageElement(
                "//button",
                "//button",
                null,
                null,
                this
        ).withText(text, false).assertIsVisible()
    }

    protected fun getLink(text: String): HybridPageElement {
        return HybridPageElement(
                "//a[normalize-space() = '$text']",
                page = this)
    }
}
