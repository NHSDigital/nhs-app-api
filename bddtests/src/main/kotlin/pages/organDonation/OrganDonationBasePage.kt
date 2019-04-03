package pages.organDonation

import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.lang.AssertionError

private const val DEFAULT_WAIT_TIME = 500L
const val DEFAULT_RETRIES = 3

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
        waitForElement(title)
    }

    private fun waitForElement(element: HybridPageElement,
                               numberOfRetries: Int = DEFAULT_RETRIES,
                               waitTime: Long = DEFAULT_WAIT_TIME) {
        //These pages are throwing stale exceptions when interacting with them
        //By waiting for specific elements, we ensure that the page is fully loaded
        var retryCountdown = numberOfRetries
        var staleElement = true
        while (staleElement || retryCountdown > 0) {
            try {
                element.assertIsVisible()
                staleElement = false
                retryCountdown=0
            } catch (e: StaleElementReferenceException) {
                staleElement = true
            } catch (e: AssertionError) {
                Thread.sleep(waitTime)
                retryCountdown--
                if(retryCountdown==0)
                    throw e
            }
        }
    }

    protected fun getLink(text: String): HybridPageElement {
        return HybridPageElement(
                "//a[normalize-space() = '$text']",
                page = this)
    }
}
