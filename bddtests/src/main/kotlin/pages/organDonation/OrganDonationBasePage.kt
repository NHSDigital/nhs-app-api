package pages.organDonation

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

const val RACE_CONDITION_WAIT: Long = 50

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
