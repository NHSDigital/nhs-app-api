package pages.organDonation

import pages.HybridPageElement
import pages.HybridPageObject

class OrganDonationYourDecisionModule(private val page: HybridPageObject) {

    private val title = "Your decision"

    private val containerXPath = "//div[h2[text()=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    init {
        container.assertSingleElementPresent().assertIsVisible()
    }

    fun assertDecisionIsNo() {
        assertText("No, I do not want to donate my organs")
    }

    private fun assertText(expectedText: String) {
        HybridPageElement(
                "$containerXPath//span",
                page = page,
                helpfulName = "Explanatory text")
                .withText(expectedText)
                .assertSingleElementPresent()
                .assertIsVisible()
    }
}