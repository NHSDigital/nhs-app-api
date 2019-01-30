package pages.organDonation

import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.BannerObject

private const val WAIT_FOR_PAGE = 1000L
abstract class OrganDonationBasePage: HybridPageObject() {

    val validationBanner by lazy { BannerObject.error(this) }

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
        title.assertIsVisible()
        waitForBackButton()
    }

    private fun waitForBackButton() {
        //These pages are throwing stale exceptions when interacting with them
        //By waiting for the back button, we ensure that the page is fully loaded
        Thread.sleep(WAIT_FOR_PAGE)
        var staleElement = true
        while (staleElement) {
            try {
                assertBackButton("Back")
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
}