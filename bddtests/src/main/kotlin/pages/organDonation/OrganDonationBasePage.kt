package pages.organDonation

import net.thucydides.core.annotations.NotImplementedException
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.avoidChromeWebDriverServiceCrash

const val RACE_CONDITION_WAIT: Long = 60

open class OrganDonationBasePage: HybridPageObject() {

    open val titleText: String = "Not Set"

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

    fun clickButton(buttonText:String) {
        //This wait has been added to ensure race condition does not occur on organ donation pages
        Thread.sleep(RACE_CONDITION_WAIT)
        clickOnButtonContainingText(buttonText)
    }

    fun assertButtonHasAttribute(buttonText: String, attributeName: String) {
        HybridPageElement(
            webDesktopLocator = "//button",
            page = this
        ).withText(buttonText, false)
            .assertIsVisible()
            .actOnTheElement {
                Assert.assertTrue(
                    "Expected attribute '$attributeName' not found on '$buttonText' button.",
                    !it.getAttribute(attributeName).isNullOrBlank()
                )
            }
    }

    open fun assertDisplayed() {
        throw NotImplementedException("assertDisplayed is not implemented on Organ Donation base page")
    }

    protected fun assertPageFullyLoaded() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        title.waitForElement()
    }

    protected fun getLink(text: String): HybridPageElement {
        return HybridPageElement(
                "//a[normalize-space() = '$text']",
                page = this)
    }
}
